using XmlExt;
using System.Xml;

namespace EasyConfig
{
	internal partial class DataType
	{
		public void WriteImplementation(IndentedStreamWriter SW)
		{
			WriteDesc(SW);

			string T = DataTypeName;
			SW.WriteLine(
				"{0} {1}class {2}{3}",
				Access ?? Global.DefaultAccessModifier,
				Partial ? "partial " : "",
				T,
				Inherit != null ? " : " + Inherit : "");

			SW.Block(() =>
			{
				DeclareFields(SW);
				SW.WriteLine();

				// Writing Constructor
				SW.WriteLine("public {0}({1}){2}", T, ConstructorParameters, Inherit != null ? " : base(Node)" : "");

				SW.Block(() =>
				{
					ConstructorPre(SW);

					foreach (var A in Attributes)
						A.WriteRead(SW);

					foreach (var F in Fields)
						SW.WriteRead(F);

					ConstructorPost(SW);
				});

				ImplementNestedClasses(SW);
			});
		}

		public virtual string DataTypeName => Name;
		protected virtual string ConstructorParameters => "XmlNode Node";
		protected virtual void ConstructorPre(IndentedStreamWriter SW) { }
		protected virtual void ConstructorPost(IndentedStreamWriter SW) { }
		protected virtual void ImplementNestedClasses(IndentedStreamWriter SW) { }

		protected virtual void DeclareFields(IndentedStreamWriter SW)
		{
			foreach (var A in Attributes) A.WriteDeclaration(SW);
			foreach (var F in Fields) F.Declare(SW, F.Type, F.Multiple);
		}

		public void WriteSample(XmlNode Node) => WriteSample(Node, true);

		public virtual void WriteSample(XmlNode Node, bool IncludeFields)
		{
			if (Inherit != null)
				Global.Name2DataType[Inherit].WriteSample(Node, IncludeFields);

			foreach (var A in Attributes) A.WriteSample(Node);

			if (IncludeFields)
				foreach (var F in Fields)
					Global.Name2DataType[F.Type].WriteSample(Node.AppendNode(F.TagName ?? F.Name), false);
		}

		public virtual void RegisterName() => Global.Name2DataType.Add(DataTypeName, this);
	}
}
