using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using Service.Manager;
using UnityEngine;

namespace Service.Taxi
{
	public class FinishCollisionDetector : CollisionDetector
	{
		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

		private readonly TaxiManager _taxiManager = Global.TaskManager?.TaxiManager;

		private void OnTriggerEnter(Collider other)
		{
			TriggerEnter(other).Forget();
		}

		private void OnTriggerExit(Collider other)
		{
			TriggerExit(other);
		}

		public override async UniTask<bool> TriggerEnter(Collider other)
		{
			if (!(await base.TriggerEnter(other)))
			{
				return false;
			}
			(_taxiManager.clients[playerView.OwnerActorNr] as ClientController)?.ChangeState(CLIENT_STATE.GO_OUT).Forget();
			if (!PhotonNetwork.IsConnected)
			{
				await _taxiManager.GetOutCarInternal(playerView.ViewID);
			}
			else
			{
				photonView.RpcToDefaultGroup("GetOutCar", RpcTarget.All, playerView.ViewID);
			}
			_taskManager.FinishTask();
			_taskManager.Clear();
			return true;
		}
	}
}
