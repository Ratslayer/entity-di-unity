namespace BB
{
	public abstract class BaseProviderAsset<T>
		: BaseScriptableObject,
		IProvider<T>
	{
		public abstract T GetValue();
	}
}