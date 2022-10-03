using System;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using EasyConfig.Attributes;

namespace EasyConfig.Editor
{
	internal partial class CompoundEditor : IEditor
	{
		private readonly Type _type;

		#region Constructors
		public CompoundEditor(Type type, object value)
		{
			_type = type;
			InitializeComponent();

			if (value == null)
				Delete();
			else New(value);
		}
		#endregion

		#region Constants
		private static readonly IReadOnlyDictionary<Type, IAttributeType> s_attributeByType = new Dictionary<Type, IAttributeType>
		{
			{ typeof(bool), new BoolAttr() },
			{ typeof(char), new CharAttr() },
			{ typeof(byte), new ByteAttr() },
			{ typeof(short), new Int16Attr() },
			{ typeof(ushort), new UInt16Attr() },
			{ typeof(int), new IntAttr() },
			{ typeof(long), new Int64Attr() },
			{ typeof(float), new SingleAttr() },
			{ typeof(double), new DoubleAttr() },
			{ typeof(string), new StringAttr() },
			{ typeof(Version), new VersionAttr() }
		};
		#endregion

		#region Private Methods
		private void Delete()
		{
			bNewDel.Content = "New";
			_listbox.Visibility = Visibility.Hidden;
			_fieldEditorContainer.Content = null;
		}

		private void New()
		{
			bNewDel.Content = "Delete";
			_listbox.Visibility = Visibility.Visible;
		}

		private void New(object Value)
		{
			New();
			PopulateFields(Value);
		}

		private void PopulateFields(object obj)
		{
			var items = _listbox.Items;

			items.Clear();
			foreach (var mi in _type.GetConfigMembers())
				items.Add(new MemberItem(mi, mi.GetValue(obj)));

			_listbox.SelectedIndex = 0;
		}
		#endregion

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get
			{
				if (Ignored) return null;

				var result = Activator.CreateInstance(_type);

				foreach (MemberItem FI in _listbox.Items)
					FI.Save(result);

				return result;
			}

			set => throw new NotSupportedException();
		}

		public bool Ignored
		{
			get => _listbox.Visibility != Visibility.Visible;
			set => throw new NotSupportedException();
		}

		public void Validate()
		{
			foreach (MemberItem mi in _listbox.Items)
				try
				{
					var editor = mi.Editor;

					if (editor.Ignored)
					{
						if (mi.Necessary)
							throw new NecessaryFieldIgnoredException();
					}
					else
						editor.Validate();
				}
				catch (Exception ex)
				{
					throw new ValidationException(this, mi, ex);
				}
		}

		public void ShowItem(object item) => _listbox.SelectedItem = item;
		#endregion

		#region Nested Class
		private class MemberItem
		{
			private readonly MemberInfo _mi;

			public readonly bool Necessary;
			public readonly IEditor Editor;

			public string Name => _mi.Name;
			public string  ConfigName => _mi.GetConfigName();

			public Brush Color => Necessary ? Brushes.DarkRed : Brushes.Black;
			public FontWeight FontWeight => Necessary ? FontWeights.SemiBold : FontWeights.Normal;

			private object Default => _mi.GetCustomAttribute<DefaultAttribute>()?.Value;

			public string Desc { get; }

			#region Constructors
			public MemberItem(MemberInfo mi, object Value)
			{
				_mi = mi;
				Necessary = mi.IsNecessary();
				Desc = mi.GetCustomAttribute<DescriptionAttribute>()?.Desc;

				var Type = mi.GetMemberType();

				{
					var editor = mi.GetCostumEditor();
					if (editor != null)
					{
						editor.Value = Value;
						Editor = editor;
						return;
					}
				}

				// T? => T
				Type = Nullable.GetUnderlyingType(Type) ?? Type;

				if (Type.IsEnum)
				{
					Editor = new EnumEditor(Type, Value, Default);
					return;
				}

				var attributeType = s_attributeByType.GetValueOrNull(Type);
				if (attributeType != null)
				{
					Editor = new PrimitiveEditor(Value, attributeType, Default);
					return;
				}

				if (Type.IsCollection())
					Editor = new CollectionEditor(Type, Value);
				else
					Editor = new CompoundEditor(Type, Value);
			}
			#endregion

			public void Save(object Obj)
			{
				if (!Editor.Ignored)
					_mi.SetValue(Obj, Editor.Value);
				else
				{
					var d = Default;
					if (d != null)
						_mi.SetValue(Obj, d);
				}
			}
		}
		#endregion

		#region Event Handlers
		private void bNewDel_OnClick(object sender, RoutedEventArgs e)
		{
			if (Ignored)
				New(Activator.CreateInstance(_type));
			else
				Delete();
		}

		private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var mi = _listbox.SelectedItem as MemberItem;
			_fieldEditorContainer.Content = mi?.Editor.Control;

			var desc = mi?.Desc;
			if (desc == null)
				_descLabel.Visibility = Visibility.Collapsed;
			else
			{
				_descLabel.Text = mi.Desc;
				_descLabel.Visibility = Visibility.Visible;
			}
		}
		#endregion
	}
}
