using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;

using XmlExt;
using EasyConfig;

using Enum = EasyConfig.Enum;

namespace Editor
{
	internal partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			AppVer = Assembly.GetExecutingAssembly().GetName().Version;

			Title += $" (v{AppVer.ToString(3)})";

			int n = Global.args.Length;
			if (n >= 1)
				if (LoadSchema(Global.args[0]))
					if (n >= 2)
						Open(Global.args[1]);
					else
						GenerateSchemaTree();
		}

		private TreeNode m_Root;

		private Schema Schema;
		private readonly Version AppVer;

		private bool SettingRadioButtonsManually;

		#region Properties
		private TreeNode Root
		{
			get => m_Root;
			set
			{
				m_Root = value;
				var TVI = value.TreeViewItem;

				TV.Items.Clear();
				TV.Items.Add(TVI);
				TVI.IsSelected = true;
			}
		}

		private bool isCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
		#endregion

		#region Methods
		private static XmlNode Select(XmlNode Node, string LocalName) => Node.SelectSingleNode($"*[local-name()='{LocalName}']");
		private static XmlNodeList SelectAll(XmlNode Node, string LocalName) => Node.SelectNodes($"*[local-name()='{LocalName}']");

		private bool LoadSchema(string Filename)
		{
			try
			{
				Global.Schema = Schema = new Schema(Filename);
				Global.DataTypeMap = new DataTypeMap(Schema);

				// Check Easy-Config Version
				Version Ver = Schema.Version;
				if (Ver.Major != AppVer.Major && Ver.Minor > AppVer.Minor)
					throw new Exception("Version Mismatch");

				return true;
			}
			catch (Exception E) { Msg.Error(E.Message, "Loading Schema Failed"); }

			return false;
		}

		private void Open(string FileName)
		{
			try
			{
				var Doc = new XmlDocument();
				Doc.Load(FileName);

				var RootNode = Doc.DocumentElement;

				var Root = CreateTreeNode(Schema.Root, null, RootNode);
				Root.Attributes.Insert(0, new AttributeValue("Version", "Version") { Value = RootNode.Attr("Version", "") });

				this.Root = Root;
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
				CreateAllFields(F.DataType, TN, XN);
			}

			void CreateAllFields(DataType DT, TreeNode Container, XmlNode XN)
			{
				foreach (var Field in DT.AllFields)
					foreach (XmlNode Node in SelectAll(XN, Field.Tag))
						CreateTreeNodeByField(Field, Container, Node);
			}
			#endregion
		}

		private void GenerateSchemaTree()
		{
			var Root = CreateTreeNode(Schema.Root, null);
			Root.Attributes.Insert(0, new AttributeValue("Version", "Version") { Value = Schema.Root.Version.ToString() });
			this.Root = Root;

			TreeNode CreateTreeNode(Node N, TreeNode Container)
			{
				var TN = new TreeNode(N, Container);

				foreach (var X in N.Nodes)
					CreateTreeNode(X, TN);

				foreach (var F in N.AllFields)
					new TreeNode(F, TN);

				return TN;
			}
		}
		#endregion

		#region Event Handlers
		private void miExit_OnClick(object sender, RoutedEventArgs e) => Close();

		private void TV_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var TVI = e.NewValue as TreeViewItem;
			if (TVI == null) return;

			var TN = (TreeNode)TVI.Tag;

			LB.Items.Clear();
			foreach (var A in TN.Attributes)
				LB.Items.Add(A);

			LB.SelectedIndex = 0;

			lblPath.SetBinding(TextBlock.TextProperty, new Binding("Path") { Source = TN });
		}

		private void miSave_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var Doc = new XmlDocument();
				var RootNode = Doc.AppendNode(Root.Tag);
				RootNode.AddAttr("Version", Schema.Version);

				Root.FillXmlNode(RootNode);

				var SFD = new SaveFileDialog
				{
					FileName = "Config",
					DefaultExt = "*.xml",
					Filter = "XML Files|*.xml|All Files|*.*"
				};

				if (SFD.ShowDialog().isTrue())
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

		private void miOpen_OnClick(object sender, RoutedEventArgs e)
		{
			var OFD = new OpenFileDialog { Filter = "(XML, EasyConfig) Files|*.xml;*.EasyConfig|All Files|*.*" };

			if (OFD.ShowDialog().isTrue())
				Open(OFD.FileName);
		}

		private void miOpenSchema_OnClick(object sender, RoutedEventArgs e)
		{
			var OFD = new OpenFileDialog { Filter = "EasyConfig Files|*.EasyConfig|All Files|*.*" };

			if (OFD.ShowDialog().isTrue())
				if (LoadSchema(OFD.FileName))
					GenerateSchemaTree();
		}

		private void TV_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (!isCtrlDown) return;
			if (e.Key != Key.Up && e.Key != Key.Down) return;

			var TVI = TV.SelectedItem as TreeViewItem;
			if (TVI == null) return;

			TreeNode
				Child = (TreeNode)TVI.Tag,
				Container = Child.Container;

			if (Container == null) return;

			var Nodes = Container.Nodes;
			var Items = Container.TreeViewItem.Items;

			int ndx = Nodes.IndexOf(Child);

			switch (e.Key)
			{
			case Key.Up:
				if (ndx > 0)
				{
					Nodes.Swap(ndx - 1, ndx);

					Items.RemoveAt(ndx);
					Items.RemoveAt(ndx - 1);

					Items.Insert(ndx - 1, Nodes[ndx - 1].TreeViewItem);
					Items.Insert(ndx, Nodes[ndx].TreeViewItem);
				}
				break;

			case Key.Down:
				if (ndx < Nodes.Count - 1)
				{
					Nodes.Swap(ndx + 1, ndx);

					Items.RemoveAt(ndx + 1);
					Items.RemoveAt(ndx);

					Items.Insert(ndx, Nodes[ndx].TreeViewItem);
					Items.Insert(ndx + 1, Nodes[ndx + 1].TreeViewItem);
				}
				break;
			}

			e.Handled =
			TVI.IsSelected = true;
		}

		private void TV_OnPreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Delete) return;

			var TVI = TV.SelectedItem as TreeViewItem;
			if (TVI == null) return;

			var Child = (TreeNode)TVI.Tag;
			if (!Child.Removable) return;

			var Container = Child.Container;

			int ndx = Container.Nodes.IndexOf(Child);

			Child.Remove();

			int n = Container.Nodes.Count;
			TreeNode Selected = n == 0 ? Container : Container.Nodes[Math.Min(ndx, n - 1)];

			Selected.TreeViewItem.IsSelected = true;

			TV.Focus();
			e.Handled = true;
		}

		private void LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var A = LB.SelectedValue as AttributeValue;
			if (A == null)
			{
				dpAttribute.Visibility = Visibility.Hidden;
				return;
			}

			dpAttribute.Visibility = Visibility.Visible;

			lblType.Text = A.Type;
			lblDefault.Text = A.Default;
			lblDesc.Text = A.Desc ?? "N/A";

			cbOverrideDefault.IsEnabled = A.HasDefault;
			cbOverrideDefault.SetBinding(CheckBox.IsCheckedProperty, NewBinding("OverrideDefault"));

			// Showing YesNoGrid if Type is yn
			if (A.Type == "yn")
			{
				cbValue.Visibility =
				tbValue.Visibility = Visibility.Hidden;
				YesNoGrid.Visibility = Visibility.Visible;

				SettingRadioButtonsManually = true;
				if (string.IsNullOrEmpty(A.Value))
				{
					rbNo.IsChecked =
					rbYes.IsChecked = false;
					rbNotSet.IsChecked = true;
				}
				else
				{
					rbNotSet.IsChecked = false;
					var Yes = A.Value.Equals("Yes", StringComparison.OrdinalIgnoreCase);
					rbNo.IsChecked = !Yes;
					rbYes.IsChecked = Yes;
				}
				SettingRadioButtonsManually = false;
				return;
			}

			#region Checking Type is Enum
			Enum Enum = null;
			foreach (var E in Schema.Enums)
				if (A.Type == E.Name)
				{
					Enum = E;
					break;
				}

			// Showing ComboBox instead of TextBox if Type is Enum
			if (Enum != null)
			{
				cbValue.Visibility = Visibility.Visible;
				tbValue.Visibility =
				YesNoGrid.Visibility = Visibility.Hidden;

				BindingOperations.ClearBinding(cbValue, ComboBox.TextProperty);

				cbValue.Items.Clear();
				//cbValue.Items.Add("");
				foreach (var Member in Enum.MembersArray)
					cbValue.Items.Add(Member);

				cbValue.SetBinding(ComboBox.TextProperty, NewBinding("Value"));
				return;
			}
			#endregion

			// Showing TextBox
			tbValue.Visibility = Visibility.Visible;
			cbValue.Visibility =
			YesNoGrid.Visibility = Visibility.Hidden;
			tbValue.SetBinding(TextBox.TextProperty, NewBinding("Value"));

			Binding NewBinding(string PropertyName) => new Binding(PropertyName)
			{
				Source = A,
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
		}

		private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
		{
			if (SettingRadioButtonsManually) return;

			var A = LB.SelectedValue as AttributeValue;
			if (A == null) return;

			var RB = (RadioButton)sender;
			A.Value = RB.Tag?.ToString();
		}

		private void miValidate_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (Schema == null) throw new Exception("No Schema");

				var sbWarnings = new StringBuilder();
				Root.Validate(sbWarnings);

				var Warnings = sbWarnings.ToString();
				if (!string.IsNullOrEmpty(Warnings))
					Msg.Warning(Warnings);
				else
					Msg.Info("Validation succeeded");
				
			}
			catch (TreeNode.AttributeValidationException E)
			{
				E.Source.RevealAndSelect();
				LB.SelectedItem = E.AttrVal;

				ShowError(E);
			}
			catch (TreeNode.ValidationException E)
			{
				E.Source.RevealAndSelect();
				ShowError(E);
			}
			catch (Exception E) { ShowError(E); }

			void ShowError(Exception E) => Msg.Error(E.Message, "Validation failed");
		}
		#endregion
	}
}
