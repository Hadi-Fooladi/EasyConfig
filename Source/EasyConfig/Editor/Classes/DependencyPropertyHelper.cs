using System.Windows;

/// <summary>
/// Dependency Property Helper
/// </summary>
internal static class DPH<PropertyType, ClassType> where ClassType : DependencyObject
{
	public delegate void dlgValueChanged(ClassType obj, PropertyType NewValue);

	public static DependencyProperty Register(string Name, PropertyType DefaultValue) =>
		DependencyProperty.Register(Name, typeof(PropertyType), typeof(ClassType), new PropertyMetadata(DefaultValue));

	public static DependencyProperty Register(string Name, PropertyType DefaultValue, dlgValueChanged OnValueChanged) =>
		DependencyProperty.Register(Name, typeof(PropertyType), typeof(ClassType), new PropertyMetadata(DefaultValue, (d, e) => OnValueChanged((ClassType)d, (PropertyType)e.NewValue)));

	public static DependencyProperty RegisterAttached(string Name, PropertyType DefaultValue) =>
		DependencyProperty.RegisterAttached(Name, typeof(PropertyType), typeof(ClassType), new PropertyMetadata(DefaultValue));
}
