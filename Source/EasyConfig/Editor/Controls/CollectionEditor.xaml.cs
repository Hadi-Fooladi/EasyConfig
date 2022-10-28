using System;
using System.Windows;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	partial class CollectionEditor : IEditor
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

		private ListItem SelectedItem => _listbox.SelectedItem as ListItem;

		public void ClearSelection() { _listbox.SelectedIndex = -1; }

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

		public double? RequestedWidth => null;

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

		public IEditor SelectedItemEditor => SelectedItem?.Editor;
		
		public event EventHandler SelectedItemChanged;
		#endregion

		#region Nested Class
		private class ListItem : ListBoxItem
		{
			public readonly IEditor Editor;
			private readonly object _value;
			private readonly string _propertyName;
			private readonly PropertyInfo _propertyInfo;

			public ListItem(Type type, object value)
			{
				Content = "Item";
				Editor = type.CreateEditor(_value = value);

				_propertyName = type.GetCustomAttribute<TextPropertyNameAttribute>()?.Name;
				if (_propertyName == null) return;
				if (!(value is INotifyPropertyChanged npc)) return;

				_propertyInfo = type.GetProperty(_propertyName);
				npc.PropertyChanged += OnPropertyChanged;

				UpdateContent();
			}

			private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == _propertyName)
					UpdateContent();
			}

			private void UpdateContent() { Content = _propertyInfo.GetValue(_value); }
		}
		#endregion

		#region Event Handlers
		private void bAdd_OnClick(object sender, RoutedEventArgs e) => _listbox.Items.Add(new ListItem(_elementType, Activator.CreateInstance(_elementType)));

		private void bDel_OnClick(object sender, RoutedEventArgs e) => _listbox.Items.Remove(_listbox.SelectedItem);

		private void LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedItemChanged?.Invoke(this, EventArgs.Empty);
		}

		private void MoveUp_OnClick(object sender, RoutedEventArgs e)
		{
			var ndx = _listbox.SelectedIndex;
			if (ndx < 1) return;

			var items = _listbox.Items;
			var item = items[ndx];

			items.RemoveAt(ndx);
			items.Insert(ndx - 1, item);

			_listbox.SelectedItem = item;
		}

		private void MoveDown_OnClick(object sender, RoutedEventArgs e)
		{
			var items = _listbox.Items;
			var ndx = _listbox.SelectedIndex;
			if (ndx >= items.Count - 1) return;

			var item = items[ndx];
			items.RemoveAt(ndx);
			items.Insert(ndx + 1, item);

			_listbox.SelectedItem = item;
		}
		#endregion

	}
}
