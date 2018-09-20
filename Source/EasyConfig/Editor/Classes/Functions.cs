using System.Xml;

namespace EasyConfig.Editor
{
	internal static class Fn
	{
		public static XmlDocument LoadXml(string Path)
		{
			var Doc = new XmlDocument();
			Doc.Load(Path);
			return Doc;
		}
	}
}
