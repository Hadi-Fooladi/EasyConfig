using System;
using EasyConfig;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Editor
{
	internal class TreeNode
	{
		#region Constructors
		public TreeNode(Node N, TreeNode Container)
		{
			DT = N;
			Name = N.Name;
			Tag = N.TagName;
			Multiple = N.Multiple;
			this.Container = Container;

			Init();
		}

		public TreeNode(Field F, TreeNode Container)
		{
			Name = F.Name;
			Tag = F.TagName;
			Multiple = F.Multiple;
			this.Container = Container;
			DT = Global.DataTypeMap[F.Type];

			Init();
		}
		#endregion

		public readonly List<TreeNode> Nodes = new List<TreeNode>();
		public readonly List<AttributeValue> Attributes = new List<AttributeValue>();

		public readonly DataType DT;
		public readonly bool Multiple;
		public readonly string Name, Tag;
		public readonly TreeNode Container;
		public readonly TreeViewItem TreeViewItem = new TreeViewItem();

		private readonly ContextMenu CM = new ContextMenu();

		private void Init()
		{
			AddAttributes();
			Container?.Nodes.Add(this);

			foreach (var F in DT.Fields)
			{
				var MI = new MenuItem { Header = "Add " + F.Name };
				MI.Click += (_, __) => new TreeNode(F, this);
				CM.Items.Add(MI);
			}

			if (DT is Node N)
				foreach (var X in N.Nodes)
				{
					var MI = new MenuItem { Header = "Add " + X.Name };
					MI.Click += (_, __) => new TreeNode(X, this);
					CM.Items.Add(MI);
				}

			if (Container != null)
			{
				CM.Items.Add(new Separator());

				var MI = new MenuItem { Header = "Remove" };
				MI.Click += (_, __) =>
				{
					Container.Nodes.Remove(this);
					Container.TreeViewItem.Items.Remove(TreeViewItem);
				};
				CM.Items.Add(MI);
			}

			TreeViewItem.Tag = this;
			TreeViewItem.Header = Name;
			TreeViewItem.ContextMenu = CM;

			Container?.TreeViewItem.Items.Add(TreeViewItem);
		}

		private void AddAttributes()
		{
			foreach (var A in DT.Attributes)
				Attributes.Add(new AttributeValue(A));
		}
	}
}
