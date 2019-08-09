using System;
using System.Xml;

namespace Tests
{
	internal static class MiscExt
	{
		public static XmlDocument ToXmlDocument(this string XmlString)
		{
			var Doc = new XmlDocument();
			Doc.LoadXml(XmlString);
			return Doc;
		}

		public static Exception GetInnerMost(this Exception E)
		{
			do
				E = E.InnerException;
			while (E?.InnerException != null);

			return E;
		}
	}
}
