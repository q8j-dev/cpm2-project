using Cysharp.Threading.Tasks;
using Firebase.Impl;

namespace Firebase.RemoteConfig
{
	public abstract class FirebaseRemoteConfig
	{
		public static readonly FirebaseRemoteConfig DefaultInstance = new Firebase.Impl.RemoteConfig();

		public abstract UniTask FetchAsync();

		public abstract string GetRaw();
	}
}
