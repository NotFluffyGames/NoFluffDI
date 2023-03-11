using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Editor.DebuggingWindow
{
    internal class TreeViewWithTreeModel<T> : TreeView where T : TreeElement
    {
        private TreeModel<T> _treeModel;

        public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model) : base(state, multiColumnHeader)
        {
            Init(model);
        }

        private void Init(TreeModel<T> model)
        {
            _treeModel = model;
            _treeModel.OnModelChanged += Reload;
        }

        protected override TreeViewItem BuildRoot()
            => _treeModel.Root.ToTreeItem();

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (_treeModel.Root == null) 
                Debug.LogError("tree model root is null. did you call SetData()?");

            List<TreeViewItem> rows = new ();

            if (!string.IsNullOrWhiteSpace(searchString))
                Search(_treeModel.Root, searchString, rows);
            else if (_treeModel.Root.HasChildren())
                AddChildrenRecursive(_treeModel.Root, rows);

            // We still need to setup the child parent information for the rows since this 
            // information is used by the TreeView internal logic (navigation, dragging etc)
            SetupParentsAndChildrenFromDepths(root, rows);

            return rows;
        }

        private void AddChildrenRecursive(T parent, IList<TreeViewItem> newRows)
        {
            foreach (var treeElement in parent.Children)
            {
                var child = (T)treeElement;
                var item = child.ToTreeItem();
                newRows.Add(item);

                if (child.HasChildren())
                {
                    if (IsExpanded(child.Id))
                        AddChildrenRecursive(child, newRows);
                    else
                        item.children = CreateChildListForCollapsedParent();
                }
            }
        }

        private static void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Invalid search: cannot be null or empty", nameof(search));

            var stack = new Stack<T>();
            foreach (var element in searchFromThis.Children) 
                stack.Push((T)element);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                // Matches search?
                if (current.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) 
                    result.Add(current.ToTreeItem());

                if (current.Children is { Count: > 0 })
                    foreach (var element in current.Children)
                        stack.Push((T)element);
            }

            SortSearchResult(result);
        }

        private static void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
        }

        protected override IList<int> GetAncestors(int id) 
            => _treeModel.GetAncestors(id);

        protected override IList<int> GetDescendantsThatHaveChildren(int id) 
            => _treeModel.GetDescendantsThatHaveChildren(id);
    }

    public static class TreeViewExt
    {
        public static TreeViewItem<T> ToTreeItem<T>(this T data) where T : TreeElement =>
            new(data.Id, data.Depth() - 1, data.Name, data);
    }
}