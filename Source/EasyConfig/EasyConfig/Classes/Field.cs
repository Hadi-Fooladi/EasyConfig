namespace EasyConfig
{
	internal partial class Field
	{
		public void WriteRead(IndentedStreamWriter SW)
		{
			SW.WriteLine();
			if (Multiple)
			{
				SW.WriteLine("{0} = new List<{1}>();", Name, Type);
				SW.WriteLine("foreach (XmlNode X in Node.SelectNodes(\"{0}\"))", TagName ?? Name);
				SW.Inside(() => SW.WriteLine("{0}.Add(new {1}(X));", Name, Type));
			}
			else
			{
				string NameNode = Name + "Node";
				SW.WriteLine("var {0} = Node.SelectSingleNode(\"{1}\");", NameNode, TagName ?? Name);
				SW.WriteLine("if ({0} != null)", NameNode);
				SW.Inside(() => SW.WriteLine("{0} = new {1}({2});", Name, Type, NameNode));
			}
			SW.WriteLine();
		}
	}
}
