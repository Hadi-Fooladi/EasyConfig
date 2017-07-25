using System.Collections.Generic;

namespace EasyConfig
{
	internal partial class Enum
	{
		public new void Declare()
		{
			WriteDesc();

			var SW = Global.SW;
			SW.WriteLine($"{Access} enum {Name}");

			var L = new List<EnumMember>();

			if (Members != null)
				foreach (var Member in Members.Replace(" ", "").Split(','))
					L.Add(new EnumMember(Member));

			L.AddRange(MembersList);

			SW.Block(() =>
			{
				int i, n = L.Count - 1;
				for (i = 0; i <= n; i++)
				{
					L[i].WriteDesc();
					SW.WriteLine(L[i] + (i == n ? "" : ","));
				}
			});
		}

		public void WriteExtMethods()
		{
			var SW = Global.SW;
			var Parse = string.Format("({0})System.Enum.Parse(typeof({0}), Node.Attr(Name));", Name);

			SW.WriteLine($"public static void Attr(this XmlNode Node, string Name, out {Name} Value)");
			SW.Block(() => SW.WriteLine($"Value = {Parse}"));

			SW.WriteLine("public static void Attr(this XmlNode Node, string Name, out {0} Value, {0} Default)", Name);
			SW.Block(() =>
			{
				SW.WriteLine("var A = Node.Attributes[Name];");
				SW.WriteLine($"Value = A == null ? Default : {Parse}");
			});
		}
	}
}
