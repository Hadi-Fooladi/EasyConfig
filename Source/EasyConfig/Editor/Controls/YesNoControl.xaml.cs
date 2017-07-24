using System;
using System.Windows;
using System.Windows.Controls;

namespace Editor
{
	internal partial class YesNoControl
	{
		public YesNoControl()
		{
			InitializeComponent();
			AttributeValue.OnValueChanged += AttributeValue_OnValueChanged;
		}

		private AttributeValue AV;

		public void BindAttributeValue(AttributeValue AV)
		{
			this.AV = AV;
			if (AV != null)
				AttributeValue_OnValueChanged(AV);
		}

		private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
		{
			if (AV == null) return;

			var RB = (RadioButton)sender;
			AV.Value = RB.Tag?.ToString();
		}

		private void AttributeValue_OnValueChanged(AttributeValue Sender)
		{
			if (Sender != AV) return;

			switch (AV.Value?.ToLower())
			{
			case "yes":
				rbYes.IsChecked = true;
				rbNo.IsChecked =
				rbNotSet.IsChecked = false;
				break;
			case "no":
				rbNo.IsChecked = true;
				rbYes.IsChecked =
				rbNotSet.IsChecked = false;
				break;
			default:
				rbNotSet.IsChecked = true;
				rbNo.IsChecked =
				rbYes.IsChecked = false;
				break;
			}
		}
	}
}
