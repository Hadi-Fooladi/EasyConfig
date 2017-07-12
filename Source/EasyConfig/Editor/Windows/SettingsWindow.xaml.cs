using System.Windows;

namespace Editor
{
	using Properties;

	internal partial class SettingsWindow
	{
		public SettingsWindow()
		{
			InitializeComponent();

			cbShowChangeLogBeforeSave.IsChecked = Settings.Default.ChangeLog_ShowNextTime;
		}

		#region Event Handlers
		private void bCancel_OnClick(object sender, RoutedEventArgs e) => Close();

		private void bOK_OnClick(object sender, RoutedEventArgs e)
		{
			Settings.Default.ChangeLog_ShowNextTime = cbShowChangeLogBeforeSave.IsChecked.isTrue();
			Settings.Default.Save();
			Close();
		}
		#endregion
	}
}
