using System;

namespace EasyConfig.Attributes
{
	/// <summary>
	/// Corresponding name in the config file
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class NameAttribute : Attribute
	{
		public string Name { get; }
		public NameAttribute(string Name) => this.Name = Name;
	}
}
