// auto-generated by EasyConfig (v4.5.28)

using XmlExt;
using System;
using System.Xml;
using System.Collections.Generic;

internal class Config
{
	/// <summary>
	/// Hostname/IP address
	/// </summary>
	public readonly string Hostname;

	public readonly int Port;

	public Config(string Filename)
	{
		var Doc = new XmlDocument();
		Doc.Load(Filename);

		var Node = Doc.DocumentElement;

		Hostname = Node.Attr("Hostname");
		Port = Node.iAttr("Port");
	}
}