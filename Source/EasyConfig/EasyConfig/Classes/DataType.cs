using XmlExt;
using System.Xml;
using System.Collections.Generic;

namespace EasyConfig
{
	internal class DataType
	{
		public string Name, Desc, Inherit;

		public readonly List<Field> Fields = new List<Field>();
		public readonly List<Attribute> Attributes = new List<Attribute>();

		public DataType(XmlNode N)
		{
			Name = N.Attr("Name");
			Desc = N.Attr("Desc", null);
			Inherit = N.Attr("Inherit", null);

			foreach (XmlNode X in N.SelectNodes("Attribute"))
				Attributes.Add(new Attribute(X));

			foreach (XmlNode X in N.SelectNodes("Field"))
				Fields.Add(new Field(X));
		}

		public void WriteImplementation(IndentedStreamWriter SW)
		{
			SW.WriteDesc(Desc);

			string T = TypeName;
			SW.WriteLine("public class {0}{1}", T, Inherit != null ? " : " + Inherit : "");
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
			foreach (var A in Attributes)
				A.WriteDeclaration(SW);

			foreach (var F in Fields)
				F.WriteDeclaration(SW);
		}

		public void WriteSample(XmlNode Node) => WriteSample(Node, true);

		public virtual void WriteSample(XmlNode Node, bool IncludeFields)
		{
			if (Inherit != null)
				Global.Name2DataType[Inherit].WriteSample(Node, IncludeFields);

			foreach (var A in Attributes) A.WriteSample(Node);

			if (IncludeFields)
				foreach (var F in Fields)
					Global.Name2DataType[F.Type].WriteSample(Node.AppendNode(F.TagName), false);
		}

		public virtual void RegisterName() => Global.Name2DataType.Add(TypeName, this);
	}
}
