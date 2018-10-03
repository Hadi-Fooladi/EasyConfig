﻿using System;
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

			var EW = EditorWindow.New<Config>(FILENAME);
			EW.OnSaveRequested += OnSaveRequested;

			App.Run(EW);
		}

		private static void OnSaveRequested(EditorWindow EW)
		{
			try
			{
				Msg.Info(EW.Value);
				EW.SaveXml(FILENAME, "Config");
			}
			catch (Exception Ex)
			{
				Msg.Error(Ex.Message);
			}
		}
	}
}
