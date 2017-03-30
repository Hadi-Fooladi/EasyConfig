using XmlExt;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace EasyConfig
{
	internal class Node
	{
		public string Name, Desc;
		public bool Multiple;

		public readonly List<Node> Nodes = new List<Node>();
		public readonly List<Attribute> Attributes = new List<Attribute>();

		public Node(XmlNode N)
		{
			Name = N.Attr("Name");
			Desc = N.Attr("Desc", null);
			Multiple = N.ynAttr("Multiple", false);

			foreach (XmlNode X in N.SelectNodes("Attribute"))
				Attributes.Add(new Attribute(X));

			foreach (XmlNode X in N.SelectNodes("Node"))
				Nodes.Add(new Node(X));
		}

		public void WriteDeclaration(StreamWriter SW)
		{
			if (Multiple)
			{
				string T = string.Format("List<{0}>", Type);
				SW.WriteLine("public readonly {0} {1} = new {0}();", T, Name);
			}
			else
				SW.WriteLine("public readonly {0} {1};", Type, Name);
		}

		public void WriteRead(IndentatedStreamWriter SW)
		{
			SW.WriteLine();
			if (Multiple)
			{
				SW.WriteLine("foreach (XmlNode X in Node.SelectNodes(\"{0}\"))", Name);
				SW.IndentationCount++;
				SW.WriteLine("{0}.Add(new {1}(X));", Name, Type);
				SW.IndentationCount--;
			}
			else
			{
				string NameNode = Name + "Node";
				SW.WriteLine("var {0} = Node.SelectSingleNode(\"{1}\");", NameNode, Name);
				SW.WriteLine("if ({0} != null)", NameNode);
				SW.IndentationCount++;
				SW.WriteLine("{0} = new {1}({2});", Name, Type, NameNode);
				SW.IndentationCount--;
			}
		}

		public void WriteImplementation(IndentatedStreamWriter SW)
		{
			if (Desc != null)
			{
				SW.WriteLine("/// <summary>");
				SW.WriteLine("/// {0}", Desc);
				SW.WriteLine("/// </summary>");
			}

			string T = Type;
			SW.WriteLine("public class " + T);
			SW.Block(() =>
			{
				DeclareFields(SW);

				// Writing Constructor
				SW.WriteLine("public {0}({1})", T, ConstructorParameters);

				SW.Block(() =>
				{
					ConstructorPre(SW);

					foreach (var A in Attributes)
						A.WriteRead(SW);

					// Write Nodes
					foreach (var N in Nodes)
						N.WriteRead(SW);
				});

				foreach (var N in Nodes)
				{
					SW.WriteLine();
					N.WriteImplementation(SW);
				}
			});
		}

		protected virtual string Type => Name + "Data";
		protected virtual string ConstructorParameters => "XmlNode Node";
		protected virtual void ConstructorPre(IndentatedStreamWriter SW) { }

		protected virtual void DeclareFields(IndentatedStreamWriter SW)
		{
			foreach (var A in Attributes)
			{
				A.WriteDeclaration(SW);
				SW.WriteLine();
			}

			foreach (var N in Nodes)
			{
				N.WriteDeclaration(SW);
				SW.WriteLine();
			}
		}
	}
}
