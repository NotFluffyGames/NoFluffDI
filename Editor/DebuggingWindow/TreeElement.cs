using System;
using System.Collections.Generic;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Editor.DebuggingWindow
{
	[Serializable]
	public class TreeElement : ITreeNode<TreeElement>
	{
		[SerializeField] private int _id;
		[SerializeField] private string _name;
		[SerializeField] private int _depth;
		[NonSerialized] private TreeElement _parent;
		[NonSerialized] private List<TreeElement> _children = new();

		public int Depth
		{
			get { return _depth; }
			set { _depth = value; }
		}

		public TreeElement Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		IReadOnlyList<TreeElement> ITreeNode<TreeElement>.Children => Children;
		public List<TreeElement> Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public string Name
		{
			get { return _name; } set { _name = value; }
		}

		public int Id
		{
			get { return _id; } set { _id = value; }
		}

		public TreeElement ()
		{
		}

		public TreeElement (string name, int depth, int id)
		{
			_name = name;
			_id = id;
			_depth = depth;
		}
	}
}