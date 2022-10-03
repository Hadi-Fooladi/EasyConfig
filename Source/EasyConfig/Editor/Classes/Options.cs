using System;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	public static class Options
	{
		public static bool
			UseFields = true,
			UseProperties = false;

		private static readonly Dictionary<Type, Func<IEditor>> s_editorByType = new Dictionary<Type, Func<IEditor>>();

		public static void AddCustomEditor<T>(Func<IEditor> editor)
		{
			s_editorByType[typeof(T)] = editor;
		}

		public static void AddCustomEditor<T, TEditor>() where TEditor : IEditor, new()
		{
			AddCustomEditor<T>(() => new TEditor());
		}

		internal static IEditor GetCustomEditor(Type type) => s_editorByType.TryGetValue(type, out var fn) ? fn() : null;
	}
}
