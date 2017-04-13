using XmlExt;
using System.Xml;
using System.Collections.Generic;

namespace EasyConfig
{
	internal class Node : DataType
	{
		public bool Multiple;
		public string ProposedTypeName;

		public readonly List<Node> Nodes = new List<Node>();
		private readonly List<DataType> Types = new List<DataType>();

		public Node(XmlNode N) : base(N)
		{
			Multiple = N.ynAttr("Multiple", false);
			ProposedTypeName = N.Attr("TypeName", null);

			foreach (XmlNode X in N.SelectNodes("Node"))
				Nodes.Add(new Node(X));

			foreach (XmlNode X in N.SelectNodes("DataType"))
				Types.Add(new DataType(X));
		}

		public void WriteDeclaration(IndentedStreamWriter SW) => SW.Declare(Name, TypeName, Multiple, null);

		public void WriteRead(IndentedStreamWriter SW)
		{
			if (Multiple)
			{
				SW.WriteLine();
				SW.WriteLine("{0} = new List<{1}>();", Name, TypeName);
				SW.WriteLine("foreach (XmlNode X in Node.SelectNodes(\"{0}\"))", Name);
				SW.Inside(() => SW.WriteLine("{0}.Add(new {1}(X));", Name, TypeName));
				SW.WriteLine();
			}
			else
			{
				SW.WriteLine();
				string NameNode = Name + "Node";
				SW.WriteLine("var {0} = Node.SelectSingleNode(\"{1}\");", NameNode, Name);
				SW.WriteLine("if ({0} != null)", NameNode);
				SW.Inside(() => SW.WriteLine("{0} = new {1}({2});", Name, TypeName, NameNode));
				SW.WriteLine();
			}
		}

		protected override string TypeName => ProposedTypeName ?? Name + "Data";

		protected override void ConstructorPost(IndentedStreamWriter SW)
		{
			foreach (var N in Nodes)
				N.WriteRead(SW);
		}

		protected override void ImplementNestedClasses(IndentedStreamWriter SW)
		{
			foreach (var N in Nodes)
			{
				SW.WriteLine();
				N.WriteImplementation(SW);
			}

			foreach (var T in Types)
			{
				SW.WriteLine();
				T.WriteImplementation(SW);
			}
		}

		protected override void DeclareFields(IndentedStreamWriter SW)
		{
			base.DeclareFields(SW);

			foreach (var N in Nodes)
				N.WriteDeclaration(SW);
		}

		public override void WriteSample(XmlNode Node, bool IncludeFields)
		{
			base.WriteSample(Node, IncludeFields);
			foreach (var N in Nodes)
				N.WriteSample(Node.AppendNode(N.Name), IncludeFields);
		}

		public override void RegisterName()
		{
			base.RegisterName();
			foreach (var N in Nodes) N.RegisterName();
			foreach (var T in Types) T.RegisterName();
		}
	}
}
