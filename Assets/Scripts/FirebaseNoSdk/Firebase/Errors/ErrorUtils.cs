using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Firebase.Errors
{
	public static class ErrorUtils
	{
		public static TRes Handler<TErr, TRes>(TErr err) where TErr : FirebaseError
		{
			Debug.LogError($"[POST] Handling error: {err}");
			return default;
		}

		public static UniTask<TRes> AsyncHandler<TErr, TRes>(TErr err) where TErr : FirebaseError
		{
			Debug.LogError($"[POST] AsyncHandling error: {err}");
			return default;
		}

		public static void Handler<TErr>(TErr err) where TErr : FirebaseError
		{
			Debug.LogError($"[POST] ActionHandling error: {err}");
		}
	}
}
