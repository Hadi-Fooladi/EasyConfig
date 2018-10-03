using System;
using System.Xml;
using System.Windows;

namespace EasyConfig.Editor
{
	public partial class EditorWindow
	{
		public delegate void dlg(EditorWindow EW);
		//public delegate void dlg<in T>(EditorWindow EW, T Data);

		#region Constructors
		public EditorWindow() => InitializeComponent();

		public EditorWindow(string XmlPath, Type T) : this()
		{
			CC.Content = EC = new EditorControl(XmlPath, T);
		}

		public EditorWindow(object Value) : this()
		{
			CC.Content = EC = new EditorControl(Value);
		}

		public static EditorWindow New<T>(string XmlPath) => new EditorWindow(XmlPath, typeof(T));
		#endregion

		private readonly EditorControl EC;

		#region Public Methods
		public void SaveXml(string XmlPath, string RootTagName)
		{
			XmlDocument Doc;

			try
			{
				Doc = EC.GetXml(RootTagName);
			}
			catch (Exception Ex)
			{
				throw new Exception($"Something went wrong.{Environment.NewLine}Please validate.", Ex);
			}

			try
			{
				Doc.Save(XmlPath);
			}
			catch (Exception Ex)
			{
				throw new Exception($"Saving failed.{Environment.NewLine}Reason: {Ex.Message}", Ex);
			}
		}

		public object Value => EC.Value;
		#endregion

		#region Event Handlers
		public event dlg OnSaveRequested;
		private void fireSaveRequested() => OnSaveRequested?.Invoke(this);
		#endregion

		#region Event Handlers
		private void bValidate_OnClick(object sender, RoutedEventArgs e) => EC.Validate();

		private void bSave_OnClick(object sender, RoutedEventArgs e) => fireSaveRequested();
		#endregion
	}
}
