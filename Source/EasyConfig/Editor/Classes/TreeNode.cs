using System;
using System.Xml;
using System.Text;
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
			DT = F.DataType;
			Name = Tag = F.Tag;
			Multiple = F.Multiple;
			this.Container = Container;

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

		public void Validate(StringBuilder sbWarnings)
		{
			// Checking Attributes Value
			foreach (var A in Attributes)
				if (!A.Validate())
					throw NewAttributeValidationException(A, $"Attribute '{A.Name}' value is incorrect.");

			foreach (var TN in Nodes)
				TN.Validate(sbWarnings);

			// Checking Nodes Count
			var Dic = new Dictionary<string, int>(); // Map Tag => Count
			foreach (var TN in Nodes)
			{
				if (Dic.ContainsKey(TN.Tag))
					Dic[TN.Tag]++;
				else
					Dic[TN.Tag] = 1;
			}

			if (DT is Node N)
				foreach (var X in N.Nodes)
				{
					ValidateNodeCount(X);
					Dic.Remove(X.Tag);
				}

			foreach (var F in DT.AllFields)
			{
				ValidateFieldCount(F);
				Dic.Remove(F.Tag);
			}

			foreach (var Key in Dic.Keys)
				sbWarnings.AppendLine($"Unknown '{Key}' member in '{Path}'");

			#region Local Functions
			void ValidateNodeCount(Node Node) => ValidateCount(Node.Multiple, Node.Tag);
			void ValidateFieldCount(Field Field) => ValidateCount(Field.Multiple, Field.Tag);
			void ValidateCount(bool Multiple, string Tag)
			{
				if (Multiple) return;

				if (!Dic.ContainsKey(Tag))
				{
					sbWarnings.AppendLine($"'{Tag}' in '{Path}' is missing");
					return;
				}

				if (Dic[Tag] != 1)
					throw NewValidationException($"Only one instance of '{Tag}' is acceptable");
			}
			#endregion
		}

		/// <summary>
		/// Used to expand all parents tree nodes to make this node visible
		/// </summary>
		public void Reveal()
		{
			var T = this;
			for (;;)
			{
				T = T.Container;
				if (T == null) return;
				T.TreeViewItem.IsExpanded = true;
			}
		}

		public void RevealAndSelect()
		{
			Reveal();
			TreeViewItem.IsSelected = true;
		}

		#region Private Methods
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
			foreach (var A in DataType.AllAttributes)
				Attributes.Add(new AttributeValue(A));
		}

		private void AddFieldsMenu(DataType DataType)
		{
			foreach (var F in DataType.AllFields)
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

		private ValidationException NewValidationException(string Message) => new ValidationException(this, Message);
		private AttributeValidationException NewAttributeValidationException(AttributeValue A, string Message) => new AttributeValidationException(this, A, Message);
		#endregion

		public class ValidationException : Exception
		{
			public new readonly TreeNode Source;
			public ValidationException(TreeNode Source, string Message) : base(Message) { this.Source = Source; }
		}

		public class AttributeValidationException : ValidationException
		{
			public readonly AttributeValue AttrVal;
			public AttributeValidationException(TreeNode Source, AttributeValue AttrVal, string Message) : base(Source, Message) { this.AttrVal = AttrVal; }

		}
	}
}
