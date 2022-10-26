using System.Windows;

namespace EasyConfig.Editor
{
	public partial class EditorWindow
	{
		public delegate void dlg(EditorWindow EW);
		//public delegate void dlg<in T>(EditorWindow EW, T Data);

		#region Constructors
		public EditorWindow(object value)
		{
			InitializeComponent();
			_ec.Value = value;
		}
		#endregion

		#region Public Methods
		public object Value => _ec.Value;
		#endregion

		#region Event Handlers
		public event dlg OnSaveRequested;
		private void fireSaveRequested() => OnSaveRequested?.Invoke(this);
		#endregion

		#region Event Handlers
		private void bValidate_OnClick(object sender, RoutedEventArgs e) => _ec.Validate();

		private void bSave_OnClick(object sender, RoutedEventArgs e) => fireSaveRequested();
		#endregion
	}
}
