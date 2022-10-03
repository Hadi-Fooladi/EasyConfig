using System.Windows;

namespace EasyConfig.Editor
{
	public partial class EditorWindow
	{
		public delegate void dlg(EditorWindow EW);
		//public delegate void dlg<in T>(EditorWindow EW, T Data);

		#region Constructors
		public EditorWindow() => InitializeComponent();

		public EditorWindow(object Value) : this()
		{
			CC.Content = EC = new EditorControl(Value);
		}
		#endregion

		private readonly EditorControl EC;

		#region Public Methods
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
