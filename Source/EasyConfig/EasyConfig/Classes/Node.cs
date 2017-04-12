using XmlExt;
using System.Xml;
using System.Collections.Generic;

namespace EasyConfig
{
	internal class Node
	{
		public bool Multiple;
		public string Name, Desc, Type, ProposedTypeName;

		public readonly List<Node> Nodes = new List<Node>();
		public readonly List<Field> Fields = new List<Field>();
		public readonly List<Attribute> Attributes = new List<Attribute>();

		public readonly bool isStruct;

		private readonly Node Container;

		public Node(XmlNode N, Node Container = null)
		{
			this.Container = Container;
			Name = N.Attr("Name");
			Desc = N.Attr("Desc", null);
			Type = N.Attr("Type", null);
			Multiple = N.ynAttr("Multiple", false);
			ProposedTypeName = N.Attr("TypeName", null);

			isStruct = (Type ?? Global.DefaultType) == "struct";

			// ReSharper disable VirtualMemberCallInConstructor
			if (isStruct) Global.Structures.Add(TypeName);

			Global.Name2Node.Add(TypeName, this);
			// ReSharper restore VirtualMemberCallInConstructor

			foreach (XmlNode X in N.SelectNodes("Attribute"))
				Attributes.Add(new Attribute(X));

			foreach (XmlNode X in N.SelectNodes("Node"))
				Nodes.Add(new Node(X, this));

			foreach (XmlNode X in N.SelectNodes("Field"))
				Fields.Add(new Field(X, this));
		}

		public void WriteDeclaration(IndentatedStreamWriter SW) => SW.Declare(Name, TypeName, Multiple, null);

		public void WriteRead(IndentatedStreamWriter SW)
		{
			SW.WriteLine();
			if (Multiple)
			{
				SW.WriteLine("{0} = new List<{1}>();", Name, TypeName);
				SW.WriteLine("foreach (XmlNode X in Node.SelectNodes(\"{0}\"))", Name);
				SW.Inside(() => SW.WriteLine("{0}.Add(new {1}(X));", Name, TypeName));
			}
			else
				if (isStruct)
					SW.WriteLine("{0} = new {1}(Node.SelectSingleNode(\"{0}\"));", Name, TypeName);
				else
				{
					string NameNode = Name + "Node";
					SW.WriteLine("var {0} = Node.SelectSingleNode(\"{1}\");", NameNode, Name);
					SW.WriteLine("if ({0} != null)", NameNode);
					SW.Inside(() => SW.WriteLine("{0} = new {1}({2});", Name, TypeName, NameNode));
					if (Container.isStruct)
					{
						SW.WriteLine("else");
						SW.Inside(() => SW.WriteLine("{0} = null;", Name));
					}
				}
		}

		public void WriteImplementation(IndentatedStreamWriter SW)
		{
			SW.WriteDesc(Desc);

			string T = TypeName;
			SW.WriteLine("public {0} {1}", Type ?? Global.DefaultType, T);
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

					foreach (var N in Nodes)
						N.WriteRead(SW);

					foreach (var F in Fields)
						F.WriteRead(SW);
				});

				foreach (var N in Nodes)
				{
					SW.WriteLine();
					N.WriteImplementation(SW);
				}
			});
		}

		protected virtual string TypeName => ProposedTypeName ?? Name + "Data";
		protected virtual string ConstructorParameters => "XmlNode Node";
		protected virtual void ConstructorPre(IndentatedStreamWriter SW) { }

		protected virtual void DeclareFields(IndentatedStreamWriter SW)
		{
			foreach (var A in Attributes)
			{
				A.WriteDeclaration(SW);
				SW.WriteLine();
			}

			foreach (var F in Fields)
			{
				F.WriteDeclaration(SW);
				SW.WriteLine();
			}

			foreach (var N in Nodes)
			{
				N.WriteDeclaration(SW);
				SW.WriteLine();
			}
		}

		public virtual void WriteSample(XmlNode Node) => WriteSample(Node, true);

		public virtual void WriteSample(XmlNode Node, bool IncludeFields)
		{
			foreach (var A in Attributes) A.WriteSample(Node);
			foreach (var N in Nodes) N.WriteSample(Node.AppendNode(N.Name));

			if (IncludeFields)
				foreach (var F in Fields)
					Global.Name2Node[F.Type].WriteSample(Node.AppendNode(F.TagName), false);
		}
	}
}
