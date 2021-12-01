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
		private PrimitiveEditor(IAttributeType TypeConverter, object Default)
		{
			InitializeComponent();
			this.TypeConverter = TypeConverter;

			// ReSharper disable once AssignmentInConditionalExpression
			if (isBool = TypeConverter is BoolAttr)
			{
				TB.Visibility = Visibility.Collapsed;
				gYesNo.Visibility = Visibility.Visible;
			}

			lblType.Text = TypeConverter.TypeName;

			if (Default == null)
				DefaultPanel.Visibility = Visibility.Collapsed;
			else
				lblDefault.Text = isBool ? TypeConverter.ToString(Default) : Default.ToString();
		}

		public PrimitiveEditor(object value, IAttributeType TypeConverter, object Default)
			: this(TypeConverter, Default)
		{
			Value = value;
		}

		public PrimitiveEditor(IAttributeType TypeConverter, XmlAttribute Attr, object Default)
			: this(TypeConverter, Default)
		{
			SetValueBy(Attr);
		}
		#endregion

		private readonly IAttributeType TypeConverter;

		private readonly bool isBool;

		private object GetValue()
		{
			if (isBool)
			{
				bool
					No = rbNo.IsChecked == true,
					Yes = rbYes.IsChecked == true;

				if (!Yes && !No)
					throw new Exception("Value not set");

				return Yes;
			}

			return TypeConverter.FromString(TB.Text);
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

				if (isBool)
					((bool)value ? rbYes : rbNo).IsChecked = true;
				else
					TB.Text = TypeConverter.ToString(value);
			}
		}

		public bool Ignored
		{
			get => cbIgnore.IsChecked == true;
			set => cbIgnore.IsChecked = value;
		}

		public void Validate() => GetValue();

		public void ShowItem(object Item) { }

		public void SaveToXmlNode(XmlNode Node, string Name) => Node.AddAttr(Name, TypeConverter.ToString(Value));

		public void SetValueBy(XmlAttribute attr)
		{
			if (attr == null)
			{
				cbIgnore.IsChecked = true;
				return;
			}

			if (isBool)
				((bool)TypeConverter.FromString(attr.Value) ? rbYes : rbNo).IsChecked = true;
			else
				TB.Text = attr.Value;
		}
		#endregion
	}
}
