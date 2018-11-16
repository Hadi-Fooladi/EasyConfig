using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace EasyConfig
{
	using Attributes;

	internal static class MiscExt
	{
		private static readonly Type CollectionType = typeof(IEnumerable);

		public static Type GetCollectionElementType(this Type T)
			=> T.IsGenericType ?
				T.GetGenericArguments().Single() :
				T.GetElementType();

		public static bool IsCollection(this Type T)
		{
			return T.GetInterface(CollectionType.Name) != null;
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

		public static bool IsNecessary(this MemberInfo MI, bool Necessary)
			=> Necessary ?
				!MI.HasAttribute<OptionalAttribute>() :
				MI.HasAttribute<NecessaryAttribute>();
	}
}
