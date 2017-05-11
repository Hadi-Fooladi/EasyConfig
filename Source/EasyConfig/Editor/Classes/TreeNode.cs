using System;
using EasyConfig;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Editor
{
	internal class TreeNode
	{
		public readonly List<TreeNode> Nodes = new List<TreeNode>();
		public readonly List<AttributeValue> Attributes = new List<AttributeValue>();

		public readonly DataType DT;

		public TreeNode(DataType DT, bool CreateFields = true)
		{
			this.DT = DT;
			foreach (var A in DT.Attributes)
				Attributes.Add(new AttributeValue(A));

			if (DT is Node N)
				foreach (var X in N.Nodes)
					Nodes.Add(new TreeNode(X));

			if (CreateFields)
				foreach (var F in DT.Fields)
					Nodes.Add(new TreeNode(Global.DataTypeMap[F.Type], false));
		}

		public TreeViewItem TreeViewItem
		{
			get
			{
				var TVI = new TreeViewItem { Header = DT.Name };

				foreach (var N in Nodes)
					TVI.Items.Add(N.TreeViewItem);

				return TVI;
			}
		}
	}
}
