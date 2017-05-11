using System;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;

using EasyConfig;

namespace Editor
{
	internal partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			//try
			//{
			var Schema = new Schema(Global.args[0]);
			Global.Schema = Schema;
			Global.DataTypeMap = new DataTypeMap(Schema);

			// Check Easy-Config Version
			Version
				Ver = Schema.Version,
				AppVer = Assembly.GetExecutingAssembly().GetName().Version;

			if (Ver.Major != AppVer.Major && Ver.Minor > AppVer.Minor)
				throw new Exception("Version Mismatch");

			var TN = new TreeNode(Schema.Root);
			TV.Items.Add(TN.TreeViewItem);



			//}
			//catch (Exception E)
			//{
			//	Msg.Error(E.Message);
			//	Application.Current.Shutdown();
			//}
		}

		private void miExit_OnClick(object sender, RoutedEventArgs e) => Close();
	}
}
