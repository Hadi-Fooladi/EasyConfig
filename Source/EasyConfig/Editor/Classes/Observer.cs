namespace Editor
{
	internal class Observer<T>
	{
		public delegate void dlg(Observer<T> Obj);

		public Observer() { }
		public Observer(T V) { Val = V; }

		private T Val;

		public T Value
		{
			get => Val;
			set
			{
				if (Equals(Val, value)) return;

				Val = value;
				OnChanged?.Invoke(this);
			}
		}

		public event dlg OnChanged;
	}
}
