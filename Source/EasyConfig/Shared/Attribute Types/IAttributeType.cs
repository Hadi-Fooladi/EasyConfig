namespace EasyConfig
{
	internal interface IAttributeType
	{
		string ToString(object Value);
		object FromString(string S);

		string TypeName { get; }
	}
}
