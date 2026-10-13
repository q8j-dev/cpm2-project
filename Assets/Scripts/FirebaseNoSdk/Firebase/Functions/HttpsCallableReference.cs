using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Firebase.Functions
{
	public abstract class HttpsCallableReference
	{
		public abstract UniTask<TRes> CallAsync<TRes>(object data = null);

		public abstract UniTask<string> CallAsyncRaw(object data = null);

		public abstract UniTask<FunctionCallResult<TRes>> CallAsyncResult<TRes>(object data = null, int networkAttempts = 1, Dictionary<string, string> extraHeaders = null);
	}
}
