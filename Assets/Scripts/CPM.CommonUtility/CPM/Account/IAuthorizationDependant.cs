using Cysharp.Threading.Tasks;

namespace CPM.Account
{
	public interface IAuthorizationDependant
	{
		UniTaskVoid OnSuccessfulAuthorization();

		void OnSignOut();
	}
}
