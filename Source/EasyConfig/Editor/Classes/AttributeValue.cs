using Attr = EasyConfig.Attribute;

namespace Editor
{
	internal class AttributeValue
	{
		public Attr Attr;
		public string Value;
		public bool OverrideDefault;

		public AttributeValue(Attr Attr) { this.Attr = Attr; }
	}
}
