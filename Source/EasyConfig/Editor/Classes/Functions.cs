using System;
using System.Windows.Input;
using System.Collections.Generic;

namespace Editor
{
	internal static class Fn
	{
		public static Uri GetLocalUri(string ResourceRelativePath) => new Uri("pack://application:,,,/Editor;component/" + ResourceRelativePath);

		public static bool isCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

		public static bool isEqual<T>(HashSet<T> A, HashSet<T> B)
		{
			if (A.Count != B.Count) return false;

			foreach (var X in A)
				if (!B.Contains(X))
					return false;

			return true;
		}
	}
}
