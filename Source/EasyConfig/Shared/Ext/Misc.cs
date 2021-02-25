using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace EasyConfig
{
	using Attributes;

	internal static class MiscExt
	{
		private static readonly Type[] AcceptedGenericCollectionTypes =
		{
			typeof(List<>),
			typeof(IList<>),
			typeof(IEnumerable<>),
			typeof(ICollection<>),
			typeof(IReadOnlyList<>),
			typeof(IReadOnlyCollection<>)
		};

		public static Type GetCollectionElementType(this Type T) => T.GetGenericArguments().Single();

		public static bool IsCollection(this Type T)
		{
			if (T.IsGenericType)
			{
				var GTD = T.GetGenericTypeDefinition();
				foreach (var GCT in AcceptedGenericCollectionTypes)
					if (GTD == GCT)
						return true;
			}

			return false;
		}

		public static TValue GetValueOrNull<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> Dic, TKey Key)
			where TValue : class
			=> Dic.TryGetValue(Key, out var Value) ? Value : null;

		public static bool HasAttribute<T>(this MemberInfo MI)
			where T : Attribute
			=> MI.GetCustomAttribute<T>() != null;

		public static string GetConfigName(this MemberInfo MI)
		{
			var A = MI.GetCustomAttribute<NameAttribute>();
			return A == null ? MI.Name : A.Name;
		}

		public static bool IsNecessary(this MemberInfo mi)
		{
			if (mi.HasAttribute<NecessaryAttribute>())
				return true;

			if (mi.DeclaringType.HasAttribute<AllFieldsNecessaryAttribute>())
				return !mi.HasAttribute<OptionalAttribute>();

			return false;
		}
	}
}
