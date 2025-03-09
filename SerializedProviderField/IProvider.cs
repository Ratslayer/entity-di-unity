namespace BB
{
	public interface IProvider<T>
	{
		T GetValue();
	}
}