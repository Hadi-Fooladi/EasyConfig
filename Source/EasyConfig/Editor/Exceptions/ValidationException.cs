using System;

namespace EasyConfig.Editor
{
	internal class ValidationException : Exception
	{
		public readonly object EditorItem;
		public readonly IEditor Editor;

		public ValidationException(IEditor Editor, object EditorItem, Exception InnerException)
			: base(InnerException.Message, InnerException)
		{
			this.Editor = Editor;
			this.EditorItem = EditorItem;
		}

		public void ShowItemInEditor() => Editor.ShowItem(EditorItem);
	}

	internal class NecessaryFieldIgnoredException : Exception
	{
		public NecessaryFieldIgnoredException() : base("Necessary Field Ignored") { }
	}
}
