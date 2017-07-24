using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Editor
{
	internal class GrayShader : ShaderEffect
	{
		private static PixelShader PS;

		public GrayShader()
		{
			if (PS == null)
				PS = new PixelShader { UriSource = Fn.GetLocalUri("Resources/Shaders/Gray.ps") };

			PixelShader = PS;
			UpdateShaderValue(InputProperty);
		}

		public static readonly DependencyProperty InputProperty =
			RegisterPixelShaderSamplerProperty("Input", typeof(GrayShader), 0);

		public Brush Input
		{
			get => (Brush)GetValue(InputProperty);
			set => SetValue(InputProperty, value);
		}
	}
}
