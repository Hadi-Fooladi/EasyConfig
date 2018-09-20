using System;

namespace EasyConfig.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class NecessaryAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Field)]
	public class OptionalAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class AllFieldsNecessaryAttribute : Attribute { }
}
