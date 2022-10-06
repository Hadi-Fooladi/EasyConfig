namespace EasyConfig.Editor
{
	public partial class EditorControl
	{
		#region Constructors
		public EditorControl(object obj)
		{
			InitializeComponent();
			Value = obj;
		}
		#endregion

		private IEditor _editor;

		#region Public Members
		public object Value
		{
			get => _editor.Value;
			set => SV.Content = _editor = value.GetType().CreateEditor(value);
		}

		public bool IsValid(bool showError)
		{
			try
			{
				_editor.Validate();
				return true;
			}
			catch (ValidationException ve)
			{
				if (!showError) return false;

				for (; ; )
				{
					ve.ShowItemInEditor();

					var ie = ve.InnerException;
					if (ie is ValidationException ex)
					{
						ve = ex;
						continue;
					}

					Msg.Error(ie.Message);
					break;
				}
			}

			return false;
		}

		public void Validate()
		{
			try
			{
				_editor.Validate();
				Msg.Info("Validation Succeeded");
			}
			catch (ValidationException VE)
			{
				for (;;)
				{
					VE.ShowItemInEditor();

					var E = VE.InnerException;
					if (E is ValidationException Ex)
						VE = Ex;
					else
					{
						Msg.Error(E.Message);
						break;
					}
				}
			}
		}
		#endregion
	}
}
