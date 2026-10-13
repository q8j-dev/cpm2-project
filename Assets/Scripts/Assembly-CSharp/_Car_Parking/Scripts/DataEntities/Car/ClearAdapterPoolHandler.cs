using CommonUtility.Contracts;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	public class ClearAdapterPoolHandler : IFirstSignInDependant
	{
		public UniTask OnFirstSignIn()
		{
			OneCarAdapterPool.Clear();
			return UniTask.CompletedTask;
		}

		public void OnSignOut()
		{
			OneCarAdapterPool.Clear();
		}
	}
}
