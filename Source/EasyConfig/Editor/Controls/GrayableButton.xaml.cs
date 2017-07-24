using System.Windows;
using System.Windows.Media;

namespace Editor
{
	internal partial class GrayableButton
	{
		public GrayableButton()
		{
			InitializeComponent();
		}

		private readonly GrayShader Sh = new GrayShader();

		public static readonly DependencyProperty isEnableProperty =
			DPH<bool, GrayableButton>.Register("isEnable", true, OnValueChanged);

		public ImageSource Source
		{
			get => img.Source;
			set
			{
				img.Source = value;
				Sh.Input = new ImageBrush(value);
			}
		}

		public bool isEnable
		{
			get => (bool)GetValue(isEnableProperty);
			set => SetValue(isEnableProperty, value);
		}

		private static void OnValueChanged(GrayableButton B, bool V)
		{
			B.IsEnabled = V;
			B.img.Effect = V ? null : B.Sh;
		}
	}
}
