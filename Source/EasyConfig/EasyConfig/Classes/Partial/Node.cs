using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal partial class Node
	{
		public override void Declare() => Declare(DataTypeName, Multiple, ReadOnly);

		public override string DataTypeName => TypeName ?? Name + "Data";

		protected override void ConstructorPost()
		{
			foreach (var N in Nodes)
				Global.SW.WriteRead(N);
		}

		protected override void ImplementNestedClasses()
		{
			foreach (var N in Nodes)
			{
				Global.SW.WriteLine();
				N.WriteImplementation();
			}

			foreach (var T in Types)
			{
				Global.SW.WriteLine();
				T.WriteImplementation();
			}
		}

		protected override void DeclareFields()
		{
			base.DeclareFields();

			foreach (var N in Nodes)
				N.Declare();
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

		public override void SaveMethodPost()
		{
			Global.SW.WriteLine();

			foreach (var N in Nodes)
				N.WriteSave(N.TagName ?? N.Name, N.Multiple);
		}

		public void WriteAssignment() => WriteAssignment(DataTypeName, Multiple, Instantiate);

		protected override void DefaultConstructorPost()
		{
			foreach (var N in Nodes)
				N.WriteAssignment();
		}
	}
}
