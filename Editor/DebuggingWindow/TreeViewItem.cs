using UnityEditor.IMGUI.Controls;

namespace NotFluffy.NoFluffDI.Editor.DebuggingWindow
{
    public class TreeViewItem<T> : TreeViewItem where T : TreeElement
    {
        public T data { get; }
    
        public TreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            this.data = data;
        }
    }
}