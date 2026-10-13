using Cysharp.Threading.Tasks;

namespace Firebase
{
	public abstract class FirebaseApp
	{
		public static FirebaseApp DefaultInstance { get; }

		public static UniTask<DependencyStatus> CheckDependenciesAsync()
		{
			return UniTask.FromResult(DependencyStatus.Available);
		}

		public static UniTask<DependencyStatus> CheckAndFixDependenciesAsync()
		{
			return UniTask.FromResult(DependencyStatus.Available);
		}
	}
}
