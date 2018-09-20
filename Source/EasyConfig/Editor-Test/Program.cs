using System;
using System.Windows;
using EasyConfig.Editor;

namespace Editor_Test
{
	internal static class Program
	{
		private const string FILENAME = "Config.xml";

		[STAThread]
		internal static void Main()
		{
			var App = new Application();
			App.Run(EditorWindow.New<Config>(FILENAME));
		}
	}
}
