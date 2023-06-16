using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NotFluffy.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Editor.DebuggingWindow
{
    public class DebuggerWindow : EditorWindow
    {
        // private const string ContainerIcon = "d_PrefabModel On Icon";
        private static string AssemblyDirectory => Path.GetDirectoryName(typeof(DebuggerWindow).GetAssemblyDefinitionPath());
        private static string ContainerIconPath =>  AssemblyDirectory + "/Resources/DiContainerIcon.png";
        private static string CodeIconPath => AssemblyDirectory + "/Resources/CodeIcon.png";
        private const string ObjectIcon = "curvekeyframeselected";
        private const string ResolverIcon = "P4_Updating";
        
        private const float WINDOW_BORDER = 10;
        private const float HORIZONTAL_SPACING = 5;

        private readonly Lazy<Texture> containerIcon = new(() => LoadIconFromPath(ContainerIconPath));
        private readonly Lazy<Texture> codeIcon = new(() => LoadIconFromPath(CodeIconPath));


        [NonSerialized] private bool _isInitialized;

        // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] private TreeViewState _treeViewState;
        [SerializeField] private MultiColumnHeaderState _multiColumnHeaderState;

        private SearchField _searchField;
        private int _id = -1;

        private MultiColumnTreeView TreeView { get; set; }
        
        private static Rect RefreshButtonRect
            => new(
                WINDOW_BORDER,
                HORIZONTAL_SPACING,
                32f,
                EditorGUIUtility.singleLineHeight
            );
        
        private Rect SearchBarRect
        {
            get
            {
                var x = RefreshButtonRect.xMax +  HORIZONTAL_SPACING;
                return new Rect( 
                    x,
                    HORIZONTAL_SPACING,
                    position.width - x - WINDOW_BORDER,
                    EditorGUIUtility.singleLineHeight
                );
            }
        }


        private Rect MultiColumnTreeViewRect
        {
            get
            {
                var refreshBut = RefreshButtonRect;
                
                var x = WINDOW_BORDER;
                var y = refreshBut.y + refreshBut.height + HORIZONTAL_SPACING;
                var width = position.width - 2 * WINDOW_BORDER;
                var height = position.height - y - WINDOW_BORDER;
                return new Rect(x, y, width, height);
            }
        }

        [MenuItem(MenuItems.MenuItemsRoot + "/Debugger %e")]
        public static void GetWindow()
        {
            var window = GetWindow<DebuggerWindow>();
            window.titleContent = new GUIContent("NoFluffDI Debugger");
            window.Focus();
            window.Repaint();
        }

        private void InitIfNeeded()
        {
            if (!_isInitialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                _treeViewState ??= new TreeViewState();

                var firstInit = _multiColumnHeaderState == null;
                var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState();
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(_multiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(_multiColumnHeaderState, headerState);
                _multiColumnHeaderState = headerState;

                var multiColumnHeader = new MultiColumnHeader(headerState)
                {
                    canSort = false,
                    height = MultiColumnHeader.DefaultGUI.minimumHeight
                };

                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<DebuggerWindowTreeElement>(GetData());

                TreeView = new MultiColumnTreeView(_treeViewState, multiColumnHeader, treeModel);
                TreeView.ExpandAll();

                _searchField = new SearchField();
                _searchField.downOrUpArrowKeyPressed += TreeView.SetFocusAndEnsureSelectedItem;

                _isInitialized = true;
            }
        }
        
        private static Texture LoadIconFromPath(string path) 
            => EditorGUIUtility.Load(path) as Texture;
        
        private void BuildDataRecursively(TreeElement parent, IReadOnlyContainer container)
        {
            if (container == null)
                return;

            var child = new DebuggerWindowTreeElement(container.Context.ToString(), parent.Depth + 1, ++_id, containerIcon.Value, container, null);
            parent.Children.Add(child);
            child.Parent = parent;

            foreach (var (id, resolver) in GetResolvers(container))
            {
                //var v = resolver.Resolve(new ResolutionContext(resolver, container, container));
                //var rText = $"{resolver.GetType().Name}<{string.Join(',', new[] { id.Type.Name, id.Id }.Where(o => o != null) )}> → {v.GetType().Name}";
                //var r = new DebuggerWindowTreeElement(rText, child.Depth + 1, ++_id,  GetIcon(v), container, resolver);
                //child.Children.Add(r);
                //r.Parent = child;
            }

            // foreach (var c in container.Children) 
            //     BuildDataRecursively(child, c);
        }
        
        private static Dictionary<ResolverID, IResolver> GetResolvers(IReadOnlyContainer container)
        {
            //const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            return new();
            //return (Dictionary<ResolverID, IResolver>)typeof(ContainerBuilder.Container).GetField("resolvers", flags).GetValue(container);
        }
        
        private IList<DebuggerWindowTreeElement> GetData()
        {
            var root = new DebuggerWindowTreeElement("Root", -1, ++_id, EditorGUIUtility.IconContent(ContainerIconPath).image, null, null);

            if(ProjectContext.Instance != null)
                BuildDataRecursively(root, ProjectContext.Instance);

            var list = new List<DebuggerWindowTreeElement>();
            TreeElementUtility.TreeToList(root, list);
            return list;
        }

        private void OnGUI()
        {
            Repaint();
            InitIfNeeded();
            SearchBar(SearchBarRect);
            DoTreeView(MultiColumnTreeViewRect);

            if (GUI.Button(RefreshButtonRect, EditorGUIUtility.IconContent("Refresh")))
            {
                _isInitialized = false;
                InitIfNeeded();
            }
        }

        private void SearchBar(Rect rect)
        {
            TreeView.searchString = _searchField.OnGUI(rect, TreeView.searchString);
        }

        private void DoTreeView(Rect rect)
        {
            TreeView.OnGUI(rect);
        }

        private Texture GetIcon(object obj)
        {
            return obj switch
            {
                IReadOnlyContainer container => containerIcon.Value,
                UnityEngine.Object uObj => EditorGUIUtility.ObjectContent(uObj, uObj.GetType()).image,
                _ => codeIcon.Value
            };
        }
    }
}