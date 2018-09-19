using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal partial class PrimitiveEditor : IEditor
	{
		#region Constructors
		public PrimitiveEditor(IAttributeType TypeConverter)
		{
			InitializeComponent();
			this.TypeConverter = TypeConverter;

			// ReSharper disable once AssignmentInConditionalExpression
			if (isBool = TypeConverter is BoolAttr)
			{
				TB.Visibility = Visibility.Collapsed;
				gYesNo.Visibility = Visibility.Visible;
			}
		}

		public PrimitiveEditor(object Value, IAttributeType TypeConverter) : this(TypeConverter)
		{
			if (isBool)
				((bool)Value ? rbYes : rbNo).IsChecked = true;
			else
				TB.Text = TypeConverter.ToString(Value);
		}

		public PrimitiveEditor(IAttributeType TypeConverter, XmlAttribute Attr) : this(TypeConverter)
		{
			if (Attr == null) return;

			if (isBool)
				((bool)TypeConverter.FromString(Attr.Value) ? rbYes : rbNo).IsChecked = true;
			else
				TB.Text = Attr.Value;
		}
		#endregion

		private readonly IAttributeType TypeConverter;

		private readonly bool isBool;

		#region IEditor Members
		public Control Control => this;

		public object Value
			=> isBool ?
				rbYes.IsChecked.isTrue() :
				TypeConverter.FromString(TB.Text);

		public void Validate()
		{
			if (isBool) return;

			try
			{
				TypeConverter.FromString(TB.Text);
			}
			catch (Exception E)
			{
				throw new ValidationException(this, null, E);
			}
		}

		public void ShowItem(object Item) { }
		#endregion
	}
}
