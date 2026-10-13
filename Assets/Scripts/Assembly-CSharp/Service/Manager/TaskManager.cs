using System;
using Photon.Pun;
using Service.Abstract;
using Service.Cargo;
using Service.Delivery;
using Service.Taxi;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Service.Manager
{
	public class TaskManager : Service.Abstract.TaskManager
	{
		public TaxiManager TaxiManager;

		public DeliveryManager DeliveryManager;

		public CargoManager CargoManager;

		private Service.Abstract.Task _task;

		public const string RPC_TASK_STATE_CHANGE = "ChangeState";

		public const string RPC_ATTACH_TRAILER = "AttachTrailer";

		public const string RPC_DETACH_TRAILER = "DetachTrailer";

		public TYPE LastTaskType { get; private set; }

		public Service.Abstract.Task Task
		{
			get
			{
				return _task;
			}
			set
			{
				_task = value;
				GPSTracker.GPSButton.SetActive(value == null);
				if (_task != null)
				{
					LastTaskType = _task.Type;
				}
			}
		}

		public event Action OnFinish;

		public event Action OnCancelReward;

		public event Action OnClear;

		public event Action OnCancelTask;

		private void Awake()
		{
			Global.TaskManager = this;
		}

		private void OnDestroy()
		{
			if (Global.TaskManager == this)
			{
				Global.TaskManager = null;
			}
			OnFinish = null;
			OnCancelReward = null;
			OnClear = null;
			OnCancelTask = null;
		}

		public void FinishTask()
		{
			_task.SetState(TASK_STATE.END);
			OnFinish?.Invoke();
		}

		public void Clear()
		{
			if (Task.GetState() != TASK_STATE.END)
			{
				OnFinish = null;
			}
			OnCancelReward = null;
			OnClear?.Invoke();
			if (Task.Type == TYPE.TAXI)
			{
				TaxiManager.serviceController.OnTaskFinished();
			}
			OnClear = null;
			Task.OnItemsCountChanged = null;
			Task = null;
		}

		public void Drop()
		{
			OnCancelTask?.Invoke();
		}

		public void CancelReward()
		{
			OnCancelReward?.Invoke();
		}

		[PunRPC]
		public void ChangeState(int viewID, byte stateIndex)
		{
			if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				PhotonView photonView = PhotonView.Find(viewID);
				if (stateIndex == 2)
				{
					photonView.GetComponent<TaxiController>()?.Clear();
					EndTaskForOther(photonView.OwnerActorNr);
				}
			}
		}

		public override void EndTaskForOther(int OwnerActorNr)
		{
			Global.TaskManager.TaxiManager.EndTaskForOther(OwnerActorNr);
			Global.TaskManager.CargoManager.EndTaskForOther(OwnerActorNr);
			Global.TaskManager.DeliveryManager.EndTaskForOther(OwnerActorNr);
		}

		[PunRPC]
		public void AttachTrailer(int ownerID, byte serviceType, Vector3 triggerPosition)
		{
			switch ((TYPE)serviceType)
			{
			case TYPE.DELIVERY:
				DeliveryManager.AttachTrailer(ownerID, triggerPosition);
				break;
			case TYPE.CARGO:
				CargoManager.AttachTrailer(ownerID, triggerPosition);
				break;
			}
		}

		[PunRPC]
		public void DetachTrailer(int ownerID, byte serviceType, Vector3 trailerPosition)
		{
			switch ((TYPE)serviceType)
			{
			case TYPE.DELIVERY:
				DeliveryManager.DetachTrailer(ownerID, trailerPosition);
				break;
			case TYPE.CARGO:
				CargoManager.DetachTrailer(ownerID, trailerPosition);
				break;
			}
		}

		[PunRPC]
		private void DestroyRPC(int ownerID, byte serviceType)
		{
			switch ((TYPE)serviceType)
			{
			case TYPE.DELIVERY:
				if (DeliveryManager.clients.ContainsKey(ownerID))
				{
					DeliveryManager.clients[ownerID].DestroyRPC();
				}
				break;
			case TYPE.CARGO:
				if (CargoManager.clients.ContainsKey(ownerID))
				{
					CargoManager.clients[ownerID].DestroyRPC();
				}
				break;
			}
		}
	}
}
