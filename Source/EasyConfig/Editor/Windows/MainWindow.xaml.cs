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
using System.Collections.Generic;
using System.Windows.Media.Imaging;

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

			AttributeValue.OnValueChanged += AttributeValue_OnValueChanged;
			AttributeValue.OnOverrideDefaultChanged += AttributeValue_OnValueChanged;
		}

		#region Fields
		private TreeNode m_Root;

		private Schema Schema;
		private readonly Version AppVer;

		private string m_SchemaFilename, m_ConfigFilename;

		private readonly HashSet<AttributeValue> ChangeSet = new HashSet<AttributeValue>();
		#endregion

		#region Properties
		private TreeNode Root
		{
			get => m_Root;
			set
			{
				m_Root = value;
				Root.ResetPrevValues();

				var TVI = value.TreeViewItem;

				TV.Items.Clear();
				TV.Items.Add(TVI);
				TVI.IsSelected = true;
			}
		}

		private static bool isCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

		private bool SaveOn
		{
			set
			{
				bSave.isEnable =
				miSave.IsEnabled = value;
				string Gray = value ? "" : "-Gray";
				((Image)miSave.Icon).Source = new BitmapImage(Fn.GetLocalUri($"Resources/Save16{Gray}.png"));
			}
		}

		private string SchemaFilename
		{
			get => m_SchemaFilename;
			set
			{
				m_SchemaFilename = value;
				UpdateFilenames();
			}
		}

		private string ConfigFilename
		{
			get => m_ConfigFilename;
			set
			{
				m_ConfigFilename = value;

				ClearChanges();
				UpdateFilenames();
			}
		}
		#endregion

		#region Methods
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

				ConfigFilename = null;
				SchemaFilename = Filename;
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

				// Tree Node Root
				var TNR = CreateTreeNode(Schema.Root, null, RootNode);
				TNR.Attributes.Insert(0, new AttributeValue("Version", "Version") { Value = RootNode.Attr("Version", "") });

				Root = TNR;
				ConfigFilename = FileName;
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
			// Tree Node Root
			var TNR = CreateTreeNode(Schema.Root, null);
			TNR.Attributes.Insert(0, new AttributeValue("Version", "Version") { Value = Schema.Root.Version.ToString() });
			Root = TNR;

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

		private void UpdateFilenames()
		{
			lblFilenames.Text = $"Config: {GetFullPath(ConfigFilename)}{Environment.NewLine}Schema: {GetFullPath(SchemaFilename)}";

			string GetFullPath(string P) => string.IsNullOrEmpty(P) ? "N/A" : Path.GetFullPath(P);
		}

		private void Save(string Filename)
		{
			try
			{
				var Doc = new XmlDocument();
				var RootNode = Doc.AppendNode(Root.Tag);
				RootNode.AddAttr("Version", Schema.Version);

				Root.FillXmlNode(RootNode);

				var XWS = new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "\t",
					OmitXmlDeclaration = true
				};

				using (var Stream = File.Open(Filename, FileMode.Create, FileAccess.Write))
				{
					var XW = XmlWriter.Create(Stream, XWS);
					Doc.Save(XW);
					XW.Close();
				}

				Root.ResetPrevValues();

				ConfigFilename = Filename;
				Msg.Info("Saving completed successfully");
			}
			catch (Exception E) { Msg.Error(E.Message); }
		}

		private void ClearChanges()
		{
			SaveOn = false;
			ChangeSet.Clear();
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
			bool DoSave;
			if (Properties.Settings.Default.ChangeLog_ShowNextTime)
			{
				var CLW = new ChangeLogWindow(Root)
				{
					bOK = { Content = "Save" }
				};

				DoSave = CLW.ShowDialog().isTrue();
			}
			else DoSave = true;

			if (DoSave) Save(ConfigFilename);
		}

		private void miSaveAs_OnClick(object sender, RoutedEventArgs e)
		{
			var SFD = new SaveFileDialog
			{
				FileName = "Config",
				DefaultExt = "*.xml",
				Filter = "XML Files|*.xml|All Files|*.*"
			};

			if (SFD.ShowDialog().isTrue())
				Save(SFD.FileName);
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
				YNC.Visibility = Visibility.Visible;
				cbValue.Visibility =
				tbValue.Visibility = Visibility.Hidden;
				YNC.BindAttributeValue(A);
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
				YNC.Visibility =
				tbValue.Visibility = Visibility.Hidden;
				cbValue.Visibility = Visibility.Visible;

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
			YNC.Visibility =
			cbValue.Visibility = Visibility.Hidden;
			tbValue.Visibility = Visibility.Visible;
			tbValue.SetBinding(TextBox.TextProperty, NewBinding("Value"));

			Binding NewBinding(string PropertyName) => new Binding(PropertyName)
			{
				Source = A,
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
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

		private void miChangeLog_OnClick(object sender, RoutedEventArgs e)
		{
			new ChangeLogWindow(Root)
			{
				bCancel = { Content = "Close" },
				bOK = { Visibility = Visibility.Collapsed },
				cbNextTime = {Visibility = Visibility.Hidden }
			}.ShowDialog();
		}

		private void AttributeValue_OnValueChanged(AttributeValue AV)
		{
			if (AV.Changed) ChangeSet.Add(AV);
			else ChangeSet.Remove(AV);

			if (ConfigFilename != null)
				SaveOn = ChangeSet.Count > 0;
		}

		private void miSettings_OnClick(object sender, RoutedEventArgs e) => new SettingsWindow().ShowDialog();
		#endregion
	}
}
