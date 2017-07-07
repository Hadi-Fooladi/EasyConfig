using System;

namespace Editor
{
	internal static class Fn
	{
		public static Uri GetLocalUri(string ResourceRelativePath) => new Uri("pack://application:,,,/Editor;component/" + ResourceRelativePath);
	}
}
