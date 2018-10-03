using System.Xml;

namespace EasyConfig.Editor
{
	public interface IVersionWriter
	{
		void Write(XmlElement Root);
	}
}
