using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal partial class Node
	{
		public void WriteDeclaration(IndentedStreamWriter SW) => Declare(SW, DataTypeName, Multiple);

		public override string DataTypeName => TypeName ?? Name + "Data";

		protected override void ConstructorPost(IndentedStreamWriter SW)
		{
			foreach (var N in Nodes)
				SW.WriteRead(N);
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
