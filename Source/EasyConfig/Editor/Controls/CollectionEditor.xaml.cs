using System;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	internal partial class CollectionEditor : IEditor
	{
		#region Constructors
		public CollectionEditor(Type collectionType, object value)
		{
			InitializeComponent();

			_elementType = collectionType.GetCollectionElementType();
			_listType = typeof(List<>).MakeGenericType(_elementType);

			if (value == null) return;

			var items = _listbox.Items;
			var collection = (ICollection)value;
			foreach (var element in collection)
				items.Add(new ListItem(_elementType, element));

			_listbox.SelectedIndex = 0;
		}
		#endregion

		private readonly Type _elementType, _listType;

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get
			{
				var list = (IList)Activator.CreateInstance(_listType);

				foreach (ListItem Item in _listbox.Items)
					list.Add(Item.Editor.Value);

				return list;
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
			foreach (ListItem item in _listbox.Items)
				try
				{
					item.Editor.Validate();
				}
				catch (Exception ex)
				{
					throw new ValidationException(this, item, ex);
				}
		}

		public void ShowItem(object item) => _listbox.SelectedItem = item;
		#endregion

		#region Nested Class
		private class ListItem
		{
			public readonly IEditor Editor;

			public ListItem(Type ValueType, object Value)
			{
				Editor = ValueType.GetCostumEditor();
				if (Editor != null)
					Editor.Value = Value;
				else
					Editor = new CompoundEditor(ValueType, Value);
			}

			public override string ToString() => "Item";
		}
		#endregion

		#region Event Handlers
		private void bAdd_OnClick(object sender, RoutedEventArgs e) => _listbox.Items.Add(new ListItem(_elementType, Activator.CreateInstance(_elementType)));

		private void bDel_OnClick(object sender, RoutedEventArgs e) => _listbox.Items.Remove(_listbox.SelectedItem);

		private void LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = _listbox.SelectedItem as ListItem;
			_fieldEditorContainer.Content = item?.Editor.Control;
		}
		#endregion
	}
}
