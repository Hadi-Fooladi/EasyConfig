﻿using System;
using System.Reflection;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	using Attributes;

	internal static class MiscExt
	{
		private const BindingFlags PUBLIC_INSTANCE_FLAG = BindingFlags.Instance | BindingFlags.Public;

		public static object GetValue(this MemberInfo mi, object obj)
		{
			switch (mi)
			{
			case FieldInfo fi: return fi.GetValue(obj);
			case PropertyInfo pi: return pi.GetValue(obj);
			default: throw new NotSupportedException();
			}
		}

		public static void SetValue(this MemberInfo mi, object obj, object value)
		{
			switch (mi)
			{
			case FieldInfo fi:
				fi.SetValue(obj, value);
				break;

			case PropertyInfo pi:
				pi.SetValue(obj, value);
				break;

			default:
				throw new NotSupportedException();
			}
		}

		public static Type GetMemberType(this MemberInfo mi)
		{
			switch (mi)
			{
			case FieldInfo fi: return fi.FieldType;
			case PropertyInfo pi: return pi.PropertyType;
			default: throw new NotSupportedException();
			}
		}

		public static IEnumerable<MemberInfo> GetConfigMembers(this Type t)
		{
			if (Options.UseFields)
				foreach (var field in t.GetFields(PUBLIC_INSTANCE_FLAG))
					if (!field.HasAttribute<IgnoreAttribute>())
						yield return field;

			if (Options.UseProperties)
				foreach (var property in t.GetProperties(PUBLIC_INSTANCE_FLAG))
					if (!property.HasAttribute<IgnoreAttribute>())
						yield return property;
		}

		public static IEditor GetCostumEditor(this MemberInfo mi)
		{
			if (mi.HasAttribute<DefaultEditorAttribute>())
				return null;

			return
				TryCreateEditorByAttribute(mi) ??
				mi.GetMemberType().GetCostumEditor();
		}

		public static IEditor GetCostumEditor(this Type type)
			=> TryCreateEditorByAttribute(type) ?? Options.GetCustomEditor(type);

		/// <returns>null if there is no <see cref="EditorAttribute"/></returns>
		private static IEditor TryCreateEditorByAttribute(MemberInfo mi)
			=> mi.GetCustomAttribute<EditorAttribute>()?.CreateNewEditor();

		private static IEditor CreateNewEditor(this EditorAttribute attr)
			=> (IEditor)Activator.CreateInstance(attr.EditorType);
	}
}
