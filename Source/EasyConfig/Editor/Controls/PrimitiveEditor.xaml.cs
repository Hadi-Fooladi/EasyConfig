using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		public Control Control => this;
	}
}
