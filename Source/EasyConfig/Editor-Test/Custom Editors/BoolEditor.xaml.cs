using System.Xml;
using System.Windows;
using System.Windows.Controls;

using EasyConfig.Editor;

namespace Editor_Test
{
	partial class BoolEditor : IEditor
	{
		public BoolEditor()
		{
			InitializeComponent();
		}

		private void bYes_OnClick(object sender, RoutedEventArgs e)
		{
			_tb.Text = "Yes";
		}

		private void bNo_OnClick(object sender, RoutedEventArgs e)
		{
			_tb.Text = "No";
		}

		#region IEditor
		public Control Control => this;

		public object Value
		{
			get => _tb.Text == "Yes";
			set => _tb.Text = value?.ToString();
		}

		public bool Ignored
		{
			get => false;
			set { }
		}

		public void Validate() { }

		public void ShowItem(object Item) { }

		public void SaveToXmlNode(XmlNode Node, string Name)
		{
			var a = Node.OwnerDocument.CreateAttribute(Name);
			a.Value = _tb.Text;
			Node.Attributes.Append(a);
		}

		public void SetValueBy(XmlAttribute attribute)
		{
			_tb.Text = attribute?.Value;
		}

		public void SetValueBy(XmlNode containerNode, string name)
		{
			SetValueBy(containerNode.Attributes[name]);
		}
		#endregion
	}
}
