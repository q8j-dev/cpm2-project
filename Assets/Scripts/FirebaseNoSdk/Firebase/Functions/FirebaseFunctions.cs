using Firebase.Impl;

namespace Firebase.Functions
{
	public abstract class FirebaseFunctions
	{
		public static readonly FirebaseFunctions DefaultInstance = new Firebase.Impl.Functions();

		public abstract HttpsCallableReference GetHttpsCallable(string endpoint, int? timeout = null);
	}
}
