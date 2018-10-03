using XmlExt;
using System;
using System.Xml;

namespace EasyConfig.Editor
{
	public class SimpleVersionWriter : IVersionWriter
	{
		public Version Version;
		public string AttributeName = "Version";

		public void Write(XmlElement Root) => Root.AddAttr(AttributeName, Version);
	}
}
