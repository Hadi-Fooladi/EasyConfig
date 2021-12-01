using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;

using EasyConfig.Editor;

namespace Editor_Test
{
	partial class BloodEditor : IEditor
	{
		public BloodEditor()
		{
			InitializeComponent();
		}

		#region IEditor
		public Control Control => this;

		public object Value
		{
			get
			{
				foreach (RadioButton rb in _g.Children)
					if (rb.IsChecked == true)
						return Enum.Parse(typeof(eBloodType), rb.Content.ToString());

				return eBloodType.Unknown;
			}

			set { }
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
			a.Value = Value.ToString();
			Node.Attributes.Append(a);
		}

		public void SetValueBy(XmlAttribute attribute)
		{
			var value = attribute?.Value;
			if (value == null) return;

			foreach (RadioButton rb in _g.Children)
				if (rb.Content.ToString() == value)
				{
					rb.IsChecked = true;
					return;
				}
		}

		public void SetValueBy(XmlNode containerNode, string name)
		{
			SetValueBy(containerNode.Attributes[name]);
		}
		#endregion
	}
}
