using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer
{
	public static class PhotonRpcCacheExtensions
	{
		public static void RefreshPhotonRpcCache(this Component component)
		{
			if ((bool)component)
			{
				PhotonView photonView = PhotonView.Get(component);
				if ((bool)photonView)
				{
					photonView.RefreshRpcMonoBehaviourCache();
				}
			}
		}

		public static void RefreshPhotonRpcCacheAfterDestroy(this Component component)
		{
			if ((bool)component)
			{
				PhotonView photonView = PhotonView.Get(component);
				if ((bool)photonView)
				{
					RefreshNextFrameAsync(photonView).Forget();
				}
			}
		}

		private static async UniTaskVoid RefreshNextFrameAsync(PhotonView view)
		{
			if (!(await UniTask.Yield(PlayerLoopTiming.EarlyUpdate, view.GetCancellationTokenOnDestroy()).SuppressCancellationThrow()) && (bool)view)
			{
				view.RefreshRpcMonoBehaviourCache();
			}
		}
	}
}
