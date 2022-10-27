using System;

namespace EasyConfig.Editor
{
	/// <summary>
	/// Used to show custom texts in <see cref="CollectionEditor" /> instead of showing "Item"
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class TextPropertyNameAttribute : Attribute
	{
		public string Name { get; }
		public TextPropertyNameAttribute(string Name) => this.Name = Name;
	}
}
