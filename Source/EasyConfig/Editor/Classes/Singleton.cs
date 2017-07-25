using System;

namespace Editor
{
	internal class Singleton<T> where T : class
	{
		public Singleton(Func<T> Constructor) { this.Constructor = Constructor; }

		private T ins;
		private readonly Func<T> Constructor;

		public T Instance => ins ?? (ins = Constructor());
	}
}
