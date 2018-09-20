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

		public PrimitiveEditor(object Value, IAttributeType TypeConverter, object Default)
			: this(TypeConverter, Default)
		{
			if (Value == null)
			{
				cbIgnore.IsChecked = true;
				return;
			}

			if (isBool)
				((bool)Value ? rbYes : rbNo).IsChecked = true;
			else
				TB.Text = TypeConverter.ToString(Value);
		}

		public PrimitiveEditor(IAttributeType TypeConverter, XmlAttribute Attr, object Default)
			: this(TypeConverter, Default)
		{
			if (Attr == null)
			{
				cbIgnore.IsChecked = true;
				return;
			}

			if (isBool)
				((bool)TypeConverter.FromString(Attr.Value) ? rbYes : rbNo).IsChecked = true;
			else
				TB.Text = Attr.Value;
		}
		#endregion

		private readonly IAttributeType TypeConverter;

		private readonly bool isBool;

		private object GetValue()
		{
			if (isBool)
			{
				bool
					No = rbNo.IsChecked.isTrue(),
					Yes = rbYes.IsChecked.isTrue();

				if (!Yes && !No)
					throw new Exception("Value not set");

				return Yes;
			}

			return TypeConverter.FromString(TB.Text);
		}

		#region IEditor Members
		public Control Control => this;

		public object Value => GetValue();

		public bool Ignored => cbIgnore.IsChecked.isTrue();

		public void Validate() => GetValue();

		public void ShowItem(object Item) { }

		public void SaveToXmlNode(XmlNode Node, string Name) => Node.AddAttr(Name, TypeConverter.ToString(Value));
		#endregion
	}
}
