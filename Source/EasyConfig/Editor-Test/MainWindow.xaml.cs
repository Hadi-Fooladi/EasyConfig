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

			object C;
			if (File.Exists(FILENAME))
				C = EasyConfig.Load<Config>(FILENAME);
			else
				C = new Config
				{
					Num = 5,
					Text = "OK",
					Version = new Version(5, 4),
					Oct = eOctal.Five
				};

			CC.Content = EC = new EditorControl(C);
		}

		private readonly EditorControl EC;
		private readonly EasyConfig.EasyConfig EasyConfig = new EasyConfig.EasyConfig();

		private void bSave_OnClick(object sender, RoutedEventArgs e)
		{
			EasyConfig.Save(EC.Value, FILENAME, "Config");
			MessageBox.Show("Save Completed!");
		}
	}
}
