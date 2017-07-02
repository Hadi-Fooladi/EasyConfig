namespace EasyConfig
{
	internal partial class Enum
	{
		public new void Declare()
		{
			WriteDesc();

			var SW = Global.SW;
			SW.WriteLine($"{Access} enum {Name}");

			SW.Block(() =>
			{
				var M = Members.Replace(" ", "").Split(',');
				int i, n = M.Length - 1;
				for (i = 0; i <= n; i++)
					SW.WriteLine(M[i] + (i == n ? "" : ","));
			});
		}

		public void WriteExtMethods()
		{
			var SW = Global.SW;
			SW.WriteLine($"public static void Attr(this XmlNode Node, string Name, out {Name} Value)");
			SW.Block(() =>
			{
				SW.WriteLine("Value = ({0})Enum.Parse(typeof({0}), Node.Attr(Name));", Name);
			});

			SW.WriteLine("public static void Attr(this XmlNode Node, string Name, out {0} Value, {0} Default)", Name);
			SW.Block(() =>
			{
				SW.WriteLine("var A = Node.Attributes[Name];");
				SW.WriteLine("Value = A == null ? Default : ({0})Enum.Parse(typeof({0}), Node.Attr(Name));", Name);
			});
		}
	}
}
