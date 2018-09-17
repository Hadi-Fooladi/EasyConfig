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
	}
}
