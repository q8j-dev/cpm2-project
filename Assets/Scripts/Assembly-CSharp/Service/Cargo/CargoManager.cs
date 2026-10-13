using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using UnityEngine;

namespace Service.Cargo
{
	public class CargoManager : TaskManager
	{
		public ServiceData data;

		public void AddTrailer(int OwnerActorNr, TrailerController trailerController)
		{
			clients[OwnerActorNr] = trailerController;
		}

		public override void EndTaskForOther(int ownerID)
		{
			clients.Remove(ownerID);
		}

		public TrailerController GetTrailerController(int OwnerActorNr)
		{
			if (clients.ContainsKey(OwnerActorNr))
			{
				return (TrailerController)clients[OwnerActorNr];
			}
			return null;
		}

		public void AttachTrailer(int ownerID, Vector3 triggerPosition)
		{
			if (clients.Count > 0)
			{
				PhotonView photonView = PhotonView.Find(ownerID);
				TrailerController obj = (TrailerController)clients[photonView.OwnerActorNr];
				obj.SetTriggerPosition(triggerPosition);
				obj.ChangeState(CLIENT_STATE.ATTACHED);
			}
		}

		public void DetachTrailer(int ownerID, Vector3 trailerPosition)
		{
			PhotonView photonView = PhotonView.Find(ownerID);
			TrailerController obj = (TrailerController)clients[photonView.OwnerActorNr];
			Transform obj2 = obj.transform;
			obj2.SetParent(V.GlobalPoint);
			obj2.localPosition = trailerPosition;
			obj.ChangeState(CLIENT_STATE.DEATTACHED);
		}

		public void Clear()
		{
			StopCounter();
			base.ProgressTime = 0f;
			Global.TaskManager.OnClear -= Clear;
		}

		private void OnDestroy()
		{
			StopCounter();
		}

		public void RequestOwnershit(GameObject car)
		{
			TrailerController trailerController = FindTrailerByCar(car);
			if (!(trailerController == null))
			{
				trailerController.photonView.RequestOwnership();
				if (trailerController.GetState() == CLIENT_STATE.ATTACHED)
				{
					CancellationToken cancellationTokenOnDestroy = trailerController.GetCancellationTokenOnDestroy();
					trailerController.TransferOwnership(car, cancellationTokenOnDestroy).Forget();
				}
			}
		}

		private TrailerController FindTrailerByCar(GameObject car)
		{
			foreach (TrailerController value in clients.Values)
			{
				if (value.Car == car)
				{
					return value;
				}
			}
			return null;
		}

		public void StopCounter()
		{
			while (cancelToken.Count > 0)
			{
				cancelToken.Dequeue().Cancel();
			}
		}
	}
}
