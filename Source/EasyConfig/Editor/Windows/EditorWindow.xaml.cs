using System;
using System.Xml;
using System.Windows;

namespace EasyConfig.Editor
{
	public partial class EditorWindow
	{
		#region Constructors
		public EditorWindow(string XmlPath, Type T)
		{
			InitializeComponent();

			this.XmlPath = XmlPath;
			CC.Content = EC = new EditorControl(XmlPath, T);
		}

		public static EditorWindow New<T>(string XmlPath) => new EditorWindow(XmlPath, typeof(T));
		#endregion

		private readonly string XmlPath;
		private readonly EditorControl EC;

		#region Event Handlers
		private void bSave_OnClick(object sender, RoutedEventArgs e)
		{
			XmlDocument Doc;
			try
			{
				Doc = EC.GetXml("Config");
			}
			catch
			{
				Msg.Error($"Something went wrong.{Environment.NewLine}Please validate.");
				return;
			}

			try
			{
				Doc.Save(XmlPath);
				MessageBox.Show("Save Completed!");
			}
			catch (Exception Ex)
			{
				Msg.Error($"Saving failed. Reason: {Ex.Message}");
			}
		}

		private void bValidate_OnClick(object sender, RoutedEventArgs e) => EC.Validate();
		#endregion
	}
}
