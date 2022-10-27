using System;
using System.Windows.Controls;

using EasyConfig.Editor;

namespace Editor_Test
{
	partial class ListItemEditor : IEditor
	{
		public ListItemEditor()
		{
			InitializeComponent();
		}

		public Control Control => this;

		private object _value;

		public object Value
		{
			get => _value;
			set => DataContext = _value = value;
		}

		public bool Ignored { get; set; }
		public double? RequestedWidth => null;
		public void Validate() { }
		public void ShowItem(object item) { }
		public IEditor SelectedItemEditor => null;
		public event EventHandler SelectedItemChanged
		{
			add { }
			remove { }
		}
	}
}
