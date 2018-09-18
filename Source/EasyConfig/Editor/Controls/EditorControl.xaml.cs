namespace EasyConfig.Editor
{
	public partial class EditorControl
	{
		public EditorControl(object Obj)
		{
			InitializeComponent();

			SV.Content = CE = new CompoundEditor(Obj.GetType(), Obj);
		}

		private readonly CompoundEditor CE;

		public object Value => CE.Value;

		public void Validate()
		{
			try
			{
				CE.Validate();
				Msg.Info("Validation Succeeded");
			}
			catch (ValidationException VE)
			{
				for(;;)
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
	}
}
