
public abstract class StringAsset : BaseScriptableObject
{
	public abstract string Value { get; }
	public static implicit operator string(StringAsset str)
		=> str ? str.Value : "";
}
public static class StringAssetExtensions
{
	public static string DefaultTo(this StringAsset str, string value)
		=> ((string)str).DefaultTo(value);
}