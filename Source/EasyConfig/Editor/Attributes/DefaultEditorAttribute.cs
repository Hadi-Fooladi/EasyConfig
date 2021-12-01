using System;

namespace EasyConfig.Editor
{
	/// <summary>
	/// Used to bypass the custom editor for the type
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DefaultEditorAttribute : Attribute { }
}
