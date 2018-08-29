using System;

namespace EasyConfig.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EasyConfigAttribute : Attribute
	{
		public string Tag;

		/// <summary>
		/// Default value if attribute is not present in the config file.<br />
		/// Note: Only works for primitive types.<br />
		/// Useful for 'struct' data-types which fields can not be initialized like a 'class'.<br />
		/// If set, 'Necessary' field will be discarded
		/// </summary>
		public object Default;

		/// <summary>
		/// Throws exception when field is missing.<br />
		/// Note: Does not apply to collection types
		/// </summary>
		public bool Necessary;
	}
}
