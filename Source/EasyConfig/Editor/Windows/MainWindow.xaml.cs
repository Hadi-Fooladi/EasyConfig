using System;
using System.IO;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using EasyConfig;
using Microsoft.Win32;
using XmlExt;

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

			Root = Get(Schema.Root, null);
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

		private readonly TreeNode Root;

		#region Event Handlers
		private void miExit_OnClick(object sender, RoutedEventArgs e) => Close();

		private void TV_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var TVI = (TreeViewItem)e.NewValue;
			var TN = (TreeNode)TVI.Tag;

			DG.ItemsSource = TN.Attributes;
			lblPath.SetBinding(TextBlock.TextProperty, new Binding("Path") { Source = TN });
		}

		private void miSave_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var Doc = new XmlDocument();
				Root.FillXmlNode(Doc.AppendNode(Root.Name));

				var SFD = new SaveFileDialog
				{
					FileName = "Config",
					DefaultExt = "*.xml",
					Filter = "XML Files|*.xml|All Files|*.*"
				};

				if (SFD.ShowDialog() ?? false)
				{
					var XWS = new XmlWriterSettings
					{
						Indent = true,
						IndentChars = "\t",
						OmitXmlDeclaration = true
					};

					using (var Stream = File.Open(SFD.FileName, FileMode.Create, FileAccess.Write))
					{
						var XW = XmlWriter.Create(Stream, XWS);
						Doc.Save(XW);
						XW.Close();
					}
				}
			}
			catch (Exception E) { Msg.Error(E.Message); }
		}
		#endregion
	}
}
