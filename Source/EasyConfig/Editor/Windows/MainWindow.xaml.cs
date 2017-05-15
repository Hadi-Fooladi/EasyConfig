using System;
using System.IO;
using System.Xml;
using System.Windows;
using Microsoft.Win32;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Controls;

using XmlExt;
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
			
			Global.Schema = Schema = new Schema(Global.args[0]);
			Global.DataTypeMap = new DataTypeMap(Schema);

			// Check Easy-Config Version
			Version
				Ver = Schema.Version,
				AppVer = Assembly.GetExecutingAssembly().GetName().Version;

			if (Ver.Major != AppVer.Major && Ver.Minor > AppVer.Minor)
				throw new Exception("Version Mismatch");

			Root = Get(Schema.Root, null);
			TV.Items.Add(Root.TreeViewItem);
			Root.TreeViewItem.IsSelected = true;

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

				foreach (var F in N.AllFields)
					new TreeNode(F, TN);

				return TN;
			}
		}

		private TreeNode Root;
		private readonly Schema Schema;

		private static bool IsTrue(bool? B) => B ?? false;
		private static bool IsFalse(bool? B) => !(B ?? true);

		private static XmlNode Select(XmlNode Node, string LocalName) => Node.SelectSingleNode($"*[local-name()='{LocalName}']");
		private static XmlNodeList SelectAll(XmlNode Node, string LocalName) => Node.SelectNodes($"*[local-name()='{LocalName}']");

		#region Event Handlers
		private void miExit_OnClick(object sender, RoutedEventArgs e) => Close();

		private void TV_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var TVI = e.NewValue as TreeViewItem;
			if (TVI == null) return;

			var TN = (TreeNode)TVI.Tag;

			DG.ItemsSource = TN.Attributes;
			lblPath.SetBinding(TextBlock.TextProperty, new Binding("Path") { Source = TN });
		}

		private void miSave_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var Doc = new XmlDocument();
				Root.FillXmlNode(Doc.AppendNode(Root.Tag));

				var SFD = new SaveFileDialog
				{
					FileName = "Config",
					DefaultExt = "*.xml",
					Filter = "XML Files|*.xml|All Files|*.*"
				};

				if (IsTrue(SFD.ShowDialog()))
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

		private void miOpen_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var OFD = new OpenFileDialog { Filter = "(XML, EasyConfig) Files|*.xml;*.EasyConfig|All Files|*.*" };

				if (IsFalse(OFD.ShowDialog())) return;

				var Doc = new XmlDocument();
				Doc.Load(OFD.FileName);

				Root = CreateTreeNode(Schema.Root, null, Doc.SelectSingleNode(Schema.Root.Tag));

				TV.Items.Clear();
				TV.Items.Add(Root.TreeViewItem);
				Root.TreeViewItem.IsSelected = true;
			}
			catch (Exception E) { Msg.Error(E.Message); }

			#region Local Functions
			TreeNode CreateTreeNode(Node N, TreeNode Container, XmlNode XN)
			{
				var TN = new TreeNode(N, Container, XN);

				CreateAllFields(N, TN, XN);

				foreach (var X in N.Nodes)
					foreach (XmlNode Node in SelectAll(XN, X.Tag))
						CreateTreeNode(X, TN, Node);

				return TN;
			}

			void CreateTreeNodeByField(Field F, TreeNode Container, XmlNode XN)
			{
				var TN = new TreeNode(F, Container, XN);
				CreateAllFields(Global.DataTypeMap[F.Type], TN, XN);
			}

			void CreateAllFields(DataType DT, TreeNode Container, XmlNode XN)
			{
				foreach (var Field in DT.AllFields)
					foreach (XmlNode Node in SelectAll(XN, Field.Tag))
						CreateTreeNodeByField(Field, Container, Node);
			}
			#endregion
		}
	}
}
