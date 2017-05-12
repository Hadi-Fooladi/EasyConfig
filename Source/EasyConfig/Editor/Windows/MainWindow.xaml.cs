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

			var Root = Get(Schema.Root, null);
			TV.Items.Add(Root.TreeViewItem);

			//}
			//catch (Exception E)
			//{
			//	Msg.Error(E.Message);
			//	Application.Current.Shutdown();
			//}

			TreeNode Get(Node N, TreeNode Container)
			{
				var TN = new TreeNode(N, Container);

				foreach (var X in N.Nodes)
					Get(X, TN);

				foreach (var F in N.Fields)
					new TreeNode(F, TN);

				return TN;
			}
		}

		#region Event Handlers
		private void miExit_OnClick(object sender, RoutedEventArgs e) => Close();

		private void TV_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var TVI = (TreeViewItem)e.NewValue;
			var TN = (TreeNode)TVI.Tag;

			DG.ItemsSource = TN.Attributes;
		}
		#endregion
	}
}
