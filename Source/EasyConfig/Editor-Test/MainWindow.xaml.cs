using System;
using System.Collections.Generic;
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

using EasyConfig.Editor;

namespace Editor_Test
{
	internal partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			var C = new Config
			{
				Num = 5,
				Text = "OK",
				Version = new Version(5, 4)
			};

			G.Children.Add(new EditorControl(C));
		}
	}
}
