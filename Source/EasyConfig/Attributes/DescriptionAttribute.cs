using System;

namespace EasyConfig.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DescriptionAttribute : Attribute
	{
		public readonly string Desc;
		public DescriptionAttribute(string Desc) => this.Desc = Desc;
	}
}
