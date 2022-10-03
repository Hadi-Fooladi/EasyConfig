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

			Options.UseFields =
			Options.UseProperties = true;

			var ec = new EasyConfig.EasyConfig
			{
				UseFields = true,
				UseProperties = true
			};

			//var wnd = new EditorWindow(new Config());
			var wnd = new EditorWindow(ec.Load<Config>(FILENAME));
			wnd.OnSaveRequested += _ =>
			{
				try
				{
					Msg.Info(wnd.Value);
					ec.GetXmlDocument(wnd.Value).Save(FILENAME);
				}
				catch (Exception Ex)
				{
					Msg.Error(Ex.Message);
				}
			};

			App.Run(wnd);
		}
	}
}
