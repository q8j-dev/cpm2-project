public class FlagNode<T>
{
	public FlagNode<T> Previous;

	public FlagNode<T> Next;

	public T Value;

	public FlagNode(T value)
	{
		Value = value;
	}
}
