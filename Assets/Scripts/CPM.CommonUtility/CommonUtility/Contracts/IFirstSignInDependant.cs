using Cysharp.Threading.Tasks;

namespace CommonUtility.Contracts
{
	public interface IFirstSignInDependant
	{
		UniTask OnFirstSignIn();

		void OnSignOut();
	}
}
