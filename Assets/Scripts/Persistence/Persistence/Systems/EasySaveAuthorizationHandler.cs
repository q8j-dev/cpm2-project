using CommonUtility.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Persistence.Systems
{
	[Preserve]
	public class EasySaveAuthorizationHandler : IFirstSignInDependant
	{
		public async UniTask OnFirstSignIn()
		{
			await EasySaveDataHandler.ReCipherToAuthorized();
			EasySaveDataHandler.ClearAllCache();
		}

		public void OnSignOut()
		{
			EasySaveDataHandler.ResetCombinedKey();
			EasySaveDataHandler.DeleteAll();
		}
	}
}
