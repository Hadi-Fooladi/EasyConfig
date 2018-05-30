namespace EasyConfig
{
	internal partial class Field
	{
		public override void Declare() => Declare(Type, Multiple, ReadOnly);

		public DataType DataType => Global.Name2DataType[Type];

		public void WriteAssignment() => WriteAssignment(Type, Multiple, Instantiate);
	}
}
