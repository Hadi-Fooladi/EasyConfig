using System;
using System.Xml;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Generic;

using XmlExt;

namespace EasyConfig.Editor
{
	internal partial class CollectionEditor : IEditor
	{
		#region Constructors
		private CollectionEditor(Type CollectionType)
		{
			InitializeComponent();

			ElementType = CollectionType.GetCollectionElementType();
			ListType = typeof(List<>).MakeGenericType(ElementType);
		}

		public CollectionEditor(Type CollectionType, object Value) : this(CollectionType)
		{
			if (Value == null) return;

			var C = (ICollection)Value;
			foreach (var X in C)
				LB.Items.Add(new ListItem(ElementType, X));

			LB.SelectedIndex = 0;
		}

		public CollectionEditor(Type CollectionType, XmlNodeList Nodes) : this(CollectionType)
		{
			foreach (XmlNode Node in Nodes)
				LB.Items.Add(new ListItem(ElementType, Node));

			LB.SelectedIndex = 0;
		}
		#endregion

		private readonly Type ElementType, ListType;

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get
			{
				var L = (IList)Activator.CreateInstance(ListType);

				foreach (ListItem Item in LB.Items)
					L.Add(Item.Editor.Value);

				return L;
			}

			set => throw new NotSupportedException();
		}

		public bool Ignored
		{
			get => false;
			set { }
		}

		public void Validate()
		{
			foreach (ListItem Item in LB.Items)
				try
				{
					Item.Editor.Validate();
				}
				catch (Exception E)
				{
					throw new ValidationException(this, Item, E);
				}
		}

		public void ShowItem(object Item) => LB.SelectedItem = Item;

		public void SaveToXmlNode(XmlNode Node, string Name)
		{
			foreach (ListItem Item in LB.Items)
			{
				var ChildNode = Node.AppendNode(Name);
				Item.Editor.SaveToXmlNode(ChildNode, null);
			}
		}
		#endregion

		#region Nested Class
		private class ListItem
		{
			public readonly IEditor Editor;

			public ListItem(Type ValueType, object Value) => Editor = new CompoundEditor(ValueType, Value);
			public ListItem(Type ValueType, XmlNode Node) => Editor = new CompoundEditor(ValueType, Node);

			public override string ToString() => "Item";
		}
		#endregion

		#region Event Handlers
		private void bAdd_OnClick(object sender, RoutedEventArgs e) => LB.Items.Add(new ListItem(ElementType, Activator.CreateInstance(ElementType)));

		private void bDel_OnClick(object sender, RoutedEventArgs e) => LB.Items.Remove(LB.SelectedItem);

		private void LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var LI = LB.SelectedItem as ListItem;
			FieldEditorContainer.Content = LI?.Editor.Control;
		}

		public void SetValueBy(XmlAttribute attribute)
		{
			throw new NotSupportedException();
		}

		public void SetValueBy(XmlNode containerNode, string name)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}
