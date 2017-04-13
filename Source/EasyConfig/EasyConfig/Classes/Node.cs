using XmlExt;
using System.Xml;

namespace Schema
{
	internal partial class Node
	{
		public void WriteDeclaration(IndentedStreamWriter SW) => SW.Declare(Name, DataTypeName, Multiple, null);

		public void WriteRead(IndentedStreamWriter SW)
		{
			string T = DataTypeName;
			SW.WriteLine();
			if (Multiple)
			{
				SW.WriteLine("{0} = new List<{1}>();", Name, T);
				SW.WriteLine("foreach (XmlNode X in Node.SelectNodes(\"{0}\"))", Name);
				SW.Inside(() => SW.WriteLine("{0}.Add(new {1}(X));", Name, T));
			}
			else
			{
				string NameNode = Name + "Node";
				SW.WriteLine("var {0} = Node.SelectSingleNode(\"{1}\");", NameNode, Name);
				SW.WriteLine("if ({0} != null)", NameNode);
				SW.Inside(() => SW.WriteLine("{0} = new {1}({2});", Name, T, NameNode));
			}
			SW.WriteLine();
		}

		protected override string DataTypeName => TypeName ?? Name + "Data";

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
