public class FirebaseWrappedFunctionFactory
{
	public FirebaseWrappedFunction Create(string functionName, int? timeout = null)
	{
		return new FirebaseWrappedFunction(functionName, timeout);
	}
}
