using System;
using System.Windows;

namespace Editor
{
	internal partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Length != 1)
			{
				Shutdown();
				return;
			}

			Global.args = e.Args;
			StartupUri = new Uri("Windows/MainWindow.xaml", UriKind.Relative);
		}
	}
}
