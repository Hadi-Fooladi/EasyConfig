using System.Windows;
using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal partial class PrimitiveEditor : IEditor
	{
		public PrimitiveEditor(object Value, IAttributeType TypeConverter)
		{
			InitializeComponent();
			this.TypeConverter = TypeConverter;

			// ReSharper disable once AssignmentInConditionalExpression
			if (isBool = TypeConverter is BoolAttr)
			{
				TB.Visibility = Visibility.Collapsed;
				gYesNo.Visibility = Visibility.Visible;

				((bool)Value ? rbYes : rbNo).IsChecked = true;
			}
			else
				TB.Text = TypeConverter.ToString(Value);
		}

		private readonly IAttributeType TypeConverter;

		private readonly bool isBool;

		#region IEditor Members
		public Control Control => this;

		public object Value
			=> isBool ?
				rbYes.IsChecked.isTrue() :
				TypeConverter.FromString(TB.Text);
		#endregion
	}
}
