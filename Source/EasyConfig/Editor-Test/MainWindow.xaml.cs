using System;
using System.IO;
using System.Windows;

using EasyConfig.Editor;

namespace Editor_Test
{
	internal partial class MainWindow
	{
		private const string FILENAME = "Config.xml";

		public MainWindow()
		{
			InitializeComponent();

			CC.Content = EC = EditorControl.New<Config>(FILENAME);

			//object C;
			//if (File.Exists(FILENAME))
			//	C = EasyConfig.Load<Config>(FILENAME);
			//else
			//	C = new Config
			//	{
			//		Num = 5,
			//		Text = "OK",
			//		Version = new Version(5, 4),
			//		Oct = eOctal.Five
			//	};

			//CC.Content = EC = new EditorControl(C);
		}

		private readonly EditorControl EC;
		private readonly EasyConfig.EasyConfig EasyConfig = new EasyConfig.EasyConfig();

		#region Event Handlers
		private void bSave_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				//EasyConfig.Save(EC.Value, FILENAME, "Config");
				EC.GetXml("Config").Save(FILENAME);
				MessageBox.Show("Save Completed!");
			}
			catch
			{
				Msg.Error($"Something went wrong.{Environment.NewLine}Please validate.");
			}
		}

		private void bValidate_OnClick(object sender, RoutedEventArgs e)
		{
			EC.Validate();
		}
		#endregion
	}
}
