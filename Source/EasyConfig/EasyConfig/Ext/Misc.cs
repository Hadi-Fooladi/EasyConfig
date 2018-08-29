using System;
using System.Reflection;

namespace EasyConfig
{
	internal static class MiscExt
	{
		public static bool HasAttribute<T>(this MemberInfo MI)
			where T : Attribute
			=> MI.GetCustomAttribute<T>() != null;
	}
}
