﻿using System;
using System.Xml;
using System.Windows.Controls;
using System.Collections.Generic;

using XmlExt;
using EasyConfig;

namespace Editor
{
	internal class TreeNode
	{
		#region Constructors
		public TreeNode(Node N, TreeNode Container)
		{
			DT = N;
			Name = Tag = N.Tag;
			Multiple = N.Multiple;
			this.Container = Container;

			Init(false);
		}

		public TreeNode(Field F, TreeNode Container)
		{
			Name = Tag = F.Tag;
			Multiple = F.Multiple;
			this.Container = Container;
			DT = Global.DataTypeMap[F.Type];

			Init(true);
		}

		public TreeNode(Node N, TreeNode Container, XmlNode XN) : this(N, Container) { AssignAttributeValues(XN); }
		public TreeNode(Field F, TreeNode Container, XmlNode XN) : this(F, Container) { AssignAttributeValues(XN); }
		#endregion

		public readonly string Name, Tag;
		public readonly TreeNode Container;
		public readonly TreeViewItem TreeViewItem = new TreeViewItem();

		public readonly List<TreeNode> Nodes = new List<TreeNode>();
		public readonly List<AttributeValue> Attributes = new List<AttributeValue>();

		private readonly DataType DT;
		private readonly bool Multiple;
		private readonly ContextMenu CM = new ContextMenu();

		public string Path { get; private set; }
		public bool Removable { get; private set; }

		public void FillXmlNode(XmlNode Node)
		{
			foreach (var A in Attributes)
				if (!A.HasDefault || A.OverrideDefault)
					Node.AddAttr(A.Name, A.Value);

			foreach (var N in Nodes)
				N.FillXmlNode(Node.AppendNode(N.Tag));
		}

		public void Remove()
		{
			if (!Removable) return;

			Container.Nodes.Remove(this);
			Container.TreeViewItem.Items.Remove(TreeViewItem);
		}

		private void Init(bool isField)
		{
			AddFieldsMenu(DT);
			AddAttributes(DT);

			if (DT is Node N)
				foreach (var X in N.Nodes)
					if (X.Multiple)
						AddMenu("Add " + X.Name, () => new TreeNode(X, this));

			Removable = isField || Multiple;
			if (Removable)
			{
				if (CM.Items.Count > 0)
					CM.Items.Add(new Separator());

				AddMenu("Remove", Remove);
			}

			TreeViewItem.Tag = this;
			TreeViewItem.Header = Name;

			if (CM.Items.Count > 0)
			{
				TreeViewItem.ContextMenu = CM;
				CM.Opened += (_, __) => TreeViewItem.IsSelected = true;
			}

			if (Container != null)
			{
				Container.Nodes.Add(this);
				Container.TreeViewItem.Items.Add(TreeViewItem);

				Path = Container.Path + "/" + Name;
			}
			else Path = Name;
		}

		private void AddMenu(string Header, Action A)
		{
			var MI = new MenuItem { Header = Header };
			MI.Click += (_, __) => A();
			CM.Items.Add(MI);
		}

		private void AddAttributes(DataType DataType)
		{
			if (DataType.Inherit != null)
				AddAttributes(Global.DataTypeMap[DataType.Inherit]);

			foreach (var A in DataType.Attributes)
				Attributes.Add(new AttributeValue(A));
		}

		private void AddFieldsMenu(DataType DataType)
		{
			if (DataType.Inherit != null)
				AddFieldsMenu(Global.DataTypeMap[DataType.Inherit]);

			foreach (var F in DataType.Fields)
				AddMenu("Add " + F.Tag, () => new TreeNode(F, this));
		}

		private void AssignAttributeValues(XmlNode Node)
		{
			foreach (var A in Attributes)
			{
				A.Value = Node.Attr(A.Name, null);
				A.OverrideDefault = A.HasDefault && A.Value != null;
			}
		}
	}
}
