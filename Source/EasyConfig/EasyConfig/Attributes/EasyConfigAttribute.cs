using System;

namespace EasyConfig.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EasyConfigAttribute : Attribute
	{
		public string Tag;
	}
}
