using System;
using System.Windows.Input;

namespace Editor
{
	internal static class Fn
	{
		public static Uri GetLocalUri(string ResourceRelativePath) => new Uri("pack://application:,,,/Editor;component/" + ResourceRelativePath);

		public static bool isCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
	}
}
