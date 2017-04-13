using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal class DataType
	{
		private readonly Schema.DataType DT;

		public DataType(Schema.DataType DT) { this.DT = DT; }

		public string Name => DT.Name;

		public void WriteImplementation(IndentedStreamWriter SW)
		{
			SW.WriteDesc(DT.Desc);

			string T = TypeName;
			SW.WriteLine("public class {0}{1}", T, DT.Inherit != null ? " : " + DT.Inherit : "");
			SW.Block(() =>
			{
				DeclareFields(SW);
				SW.WriteLine();

				// Writing Constructor
				SW.WriteLine("public {0}({1}){2}", T, ConstructorParameters, DT.Inherit != null ? " : base(Node)" : "");

				SW.Block(() =>
				{
					ConstructorPre(SW);

					foreach (var A in DT.Attributes)
						A.WriteRead(SW);

					foreach (var F in DT.Fields)
						F.WriteRead(SW);

					ConstructorPost(SW);
				});

				ImplementNestedClasses(SW);
			});
		}

		protected virtual string TypeName => Name;
		protected virtual string ConstructorParameters => "XmlNode Node";
		protected virtual void ConstructorPre(IndentedStreamWriter SW) { }
		protected virtual void ConstructorPost(IndentedStreamWriter SW) { }
		protected virtual void ImplementNestedClasses(IndentedStreamWriter SW) { }

		protected virtual void DeclareFields(IndentedStreamWriter SW)
		{
			foreach (var A in DT.Attributes) A.WriteDeclaration(SW);
			foreach (var F in DT.Fields) SW.Declare(F);
		}

		public void WriteSample(XmlNode Node) => WriteSample(Node, true);

		public virtual void WriteSample(XmlNode Node, bool IncludeFields)
		{
			if (DT.Inherit != null)
				Global.Name2DataType[DT.Inherit].WriteSample(Node, IncludeFields);

			foreach (var A in DT.Attributes) A.WriteSample(Node);

			if (IncludeFields)
				foreach (var F in DT.Fields)
					Global.Name2DataType[F.Type].WriteSample(Node.AppendNode(F.TagName ?? F.Name), false);
		}

		public virtual void RegisterName() => Global.Name2DataType.Add(TypeName, this);
	}
}
