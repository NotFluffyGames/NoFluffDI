using System;
using UnityEditor;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Editor.DebuggingWindow
{
    [Serializable]
    internal class DebuggerWindowTreeElement : TreeElement
    {
        public readonly IResolver Resolver;
        public readonly IReadOnlyContainer Container;
        
        public Texture Icon { get; }

        public DebuggerWindowTreeElement(string name, int depth, int id, Texture icon, IReadOnlyContainer container, IResolver resolver) : base(name, depth, id)
        {
            Icon = icon;
            Container = container;
            Resolver = resolver;
        }
    }
}