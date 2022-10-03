using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;

using XmlExt;

namespace EasyConfig.Editor
{
	internal partial class PrimitiveEditor : IEditor
	{
		#region Constructors
		public PrimitiveEditor(object value, IAttributeType typeConverter, object @default)
		{
			InitializeComponent();
			_typeConverter = typeConverter;

			// ReSharper disable once AssignmentInConditionalExpression
			if (_isBool = typeConverter is BoolAttr)
			{
				TB.Visibility = Visibility.Collapsed;
				gYesNo.Visibility = Visibility.Visible;
			}

			lblType.Text = typeConverter.TypeName;

			if (@default == null)
				DefaultPanel.Visibility = Visibility.Collapsed;
			else
				lblDefault.Text = _isBool ? typeConverter.ToString(@default) : @default.ToString();

			Value = value;
		}
		#endregion

		private readonly IAttributeType _typeConverter;

		private readonly bool _isBool;

		private object GetValue()
		{
			if (_isBool)
			{
				bool
					no = rbNo.IsChecked == true,
					yes = rbYes.IsChecked == true;

				if (!yes && !no)
					throw new Exception("Value not set");

				return yes;
			}

			return _typeConverter.FromString(TB.Text);
		}

		#region IEditor Members
		public Control Control => this;

		public object Value
		{
			get => GetValue();

			set
			{
				if (value == null)
				{
					cbIgnore.IsChecked = true;
					return;
				}

				if (_isBool)
					((bool)value ? rbYes : rbNo).IsChecked = true;
				else
					TB.Text = _typeConverter.ToString(value);
			}
		}

		public bool Ignored
		{
			get => cbIgnore.IsChecked == true;
			set => cbIgnore.IsChecked = value;
		}

		public void Validate() => GetValue();

		public void ShowItem(object Item) { }
		#endregion
	}
}
