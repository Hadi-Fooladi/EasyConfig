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

			Init();
		}

		public TreeNode(Field F, TreeNode Container)
		{
			DT = F.DataType;
			Name = Tag = F.Tag;
			Multiple = F.Multiple;
			this.Container = Container;

			Init();
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

		public string Path { get; private set; }
		public bool Removable => Container != null && (Multiple || !Container.CheckNodeMaxCount(Name, 1));

		#region Public Methods
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
			if (Container == null) return;

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

		public void ResetPrevValues()
		{
			foreach (var N in Nodes) N.ResetPrevValues();
			foreach (var A in Attributes) A.ResetPrevValues();
		}

		public void FillContextMenu()
		{
			var CM = Global.CM;
			CM.Items.Clear();

			bool ShowAll = Fn.isCtrlDown;

			foreach (var F in DT.AllFields)
				if (ShowAll || F.Multiple || !CheckNodeNameExist(F.Tag))
					AddMenu("Add " + F.Tag, () => new TreeNode(F, this));

			if (DT is Node N)
				foreach (var X in N.Nodes)
					if (ShowAll || X.Multiple || !CheckNodeNameExist(X.Tag))
						AddMenu("Add " + X.Name, () => new TreeNode(X, this));

			// Remove
			if (ShowAll || Removable)
			{
				if (CM.Items.Count > 0)
					CM.Items.Add(new Separator());

				AddMenu("Remove", Remove);
			}
		}
		#endregion

		#region Private Methods
		private void Init()
		{
			AddAttributes(DT);

			if (DT is Node N)
				foreach (var X in N.Nodes)
					if (X.Multiple)
						AddMenu("Add " + X.Name, () => new TreeNode(X, this));

			TreeViewItem.Tag = this;
			TreeViewItem.Header = Name;

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
			Global.CM.Items.Add(MI);
		}

		private void AddAttributes(DataType DataType)
		{
			foreach (var A in DataType.AllAttributes)
				Attributes.Add(new AttributeValue(A));
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

		private bool CheckNodeNameExist(string Name)
		{
			foreach (var X in Nodes)
				if (X.Name == Name)
					return true;

			return false;
		}

		/// <returns>false if there are nodes with this <paramref name="Name"/> more than <paramref name="MaxCount"/>; otherwise true.</returns>
		private bool CheckNodeMaxCount(string Name, int MaxCount)
		{
			int Count = 0;
			foreach (var X in Nodes)
				if (X.Name == Name)
				{
					Count++;
					if (Count > MaxCount)
						return false;
				}

			return true;
		}
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
