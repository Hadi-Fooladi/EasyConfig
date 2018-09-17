using System.Windows.Controls;

namespace EasyConfig.Editor
{
	internal partial class PrimitiveEditor : IEditor
	{
		public PrimitiveEditor(object Value, IAttributeType TypeConverter)
		{
			InitializeComponent();
			this.TypeConverter = TypeConverter;
			TB.Text = TypeConverter.ToString(Value);
		}

		private readonly IAttributeType TypeConverter;

		#region IEditor Members
		public Control Control => this;

		public object Value => TypeConverter.FromString(TB.Text);
		#endregion
	}
}
