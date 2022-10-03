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

		private CompoundEditor CE;

		#region Public Members
		public object Value
		{
			get => CE.Value;
			set => SV.Content = CE = new CompoundEditor(value.GetType(), value);
		}

		public bool IsValid(bool showError)
		{
			try
			{
				CE.Validate();
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
				CE.Validate();
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
