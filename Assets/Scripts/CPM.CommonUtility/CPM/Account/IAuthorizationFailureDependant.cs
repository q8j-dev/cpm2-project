using Cysharp.Threading.Tasks;

namespace CPM.Account
{
	public interface IAuthorizationFailureDependant
	{
		UniTaskVoid OnAuthorizationFailure();
	}
}
