using System;

namespace EasyConfig.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DescriptionAttribute : Attribute
	{
		public readonly string Desc;
		public DescriptionAttribute(string Desc) => this.Desc = Desc;
	}
}
