namespace EasyConfig
{
	internal static class FieldExt
	{
		public static void WriteRead(this Schema.Field F, IndentedStreamWriter SW)
		{
			SW.WriteLine();
			if (F.Multiple)
			{
				SW.WriteLine("{0} = new List<{1}>();", F.Name, F.Type);
				SW.WriteLine("foreach (XmlNode X in Node.SelectNodes(\"{0}\"))", F.TagName ?? F.Name);
				SW.Inside(() => SW.WriteLine("{0}.Add(new {1}(X));", F.Name, F.Type));
			}
			else
			{
				string NameNode = F.Name + "Node";
				SW.WriteLine("var {0} = Node.SelectSingleNode(\"{1}\");", NameNode, F.TagName ?? F.Name);
				SW.WriteLine("if ({0} != null)", NameNode);
				SW.Inside(() => SW.WriteLine("{0} = new {1}({2});", F.Name, F.Type, NameNode));
			}
			SW.WriteLine();
		}
	}
}
