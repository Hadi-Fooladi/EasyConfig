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

	/// <summary>
	/// Determines the default value if the field is not present in the config file.<br />
	/// Note: Only works for primitive types.<br />
	/// Useful for 'struct' data-types which fields can not be initialized like a 'class'.<br />
	/// If set, 'Necessary' attribute will be discarded
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class DefaultAttribute : Attribute
	{
		public object Value { get; }
		public DefaultAttribute(object Value) => this.Value = Value;
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class NecessaryAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Field)]
	public class OptionalAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class AllFieldsNecessaryAttribute : Attribute { }
}
