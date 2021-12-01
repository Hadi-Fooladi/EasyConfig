using System;

namespace EasyConfig.Editor
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
	public class EditorAttribute : Attribute
	{
		/// <param name="editorType">Should implement <see cref="IEditor" /> and have a default constructor</param>
		public EditorAttribute(Type editorType)
		{
			EditorType = editorType;
		}

		public Type EditorType { get; }

		internal IEditor CreateNewEditor() => (IEditor)Activator.CreateInstance(EditorType);
	}
}
