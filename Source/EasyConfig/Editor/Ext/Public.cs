using System;
using System.Reflection;
using System.Collections.Generic;

namespace EasyConfig.Editor
{
	using Attributes;

	public static class PublicExt
	{
		public static IEditor CreateEditor(this MemberInfo mi, object value)
		{
			var type = mi.GetMemberType();

			var editor = mi.GetCostumEditor();
			if (editor != null)
			{
				editor.Value = value;
				return editor;
			}

			// T? => T
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type.IsEnum)
				return new EnumEditor(type, value, getDefault());

			var attributeType = s_attributeByType.GetValueOrNull(type);
			if (attributeType != null)
				return new PrimitiveEditor(value, attributeType, getDefault());

			if (type.IsCollection())
				return new CollectionEditor(type, value);

			return new CompoundEditor(type, value);

			object getDefault() => mi.GetCustomAttribute<DefaultAttribute>()?.Value;
		}

		public static IEditor CreateEditor(this Type type, object value)
		{
			var editor = type.GetCostumEditor();
			if (editor != null)
			{
				editor.Value = value;
				return editor;
			}

			// T? => T
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type.IsEnum)
				return new EnumEditor(type, value, null);

			var attributeType = s_attributeByType.GetValueOrNull(type);
			if (attributeType != null)
				return new PrimitiveEditor(value, attributeType, null);

			if (type.IsCollection())
				return new CollectionEditor(type, value);

			return new CompoundEditor(type, value);
		}

		private static readonly IReadOnlyDictionary<Type, IAttributeType> s_attributeByType = new Dictionary<Type, IAttributeType>
		{
			{ typeof(bool), new BoolAttr() },
			{ typeof(char), new CharAttr() },
			{ typeof(byte), new ByteAttr() },
			{ typeof(short), new Int16Attr() },
			{ typeof(ushort), new UInt16Attr() },
			{ typeof(int), new IntAttr() },
			{ typeof(long), new Int64Attr() },
			{ typeof(float), new SingleAttr() },
			{ typeof(double), new DoubleAttr() },
			{ typeof(string), new StringAttr() },
			{ typeof(Version), new VersionAttr() }
		};
	}
}
