using System;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	internal partial class CollectionEditor : IEditor
	{
		public CollectionEditor(Type CollectionType, object Value)
		{
			InitializeComponent();

			ElementType = CollectionType.GetCollectionElementType();
			ListType = typeof(List<>).MakeGenericType(ElementType);

			if (Value == null) return;

			var C = (ICollection)Value;
			foreach (var X in C)
				LB.Items.Add(new ListItem(ElementType, X));
		}

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
		}
		#endregion

		#region Nested Class
		private class ListItem
		{
			private readonly Type T;
			public readonly IEditor Editor;

			public ListItem(Type ValueType, object Value)
			{
				T = ValueType;
				Editor = new CompoundEditor(ValueType, Value);
			}

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
		#endregion
	}
}
