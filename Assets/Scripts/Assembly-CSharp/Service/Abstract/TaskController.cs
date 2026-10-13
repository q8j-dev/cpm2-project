using Photon.Pun;
using Service.Manager;
using UnityEngine;

namespace Service.Abstract
{
	public abstract class TaskController : MonoBehaviour, ITaskController
	{
		[SerializeField]
		protected PhotonView playerView;

		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

		public virtual void UpdateTask(Order order)
		{
			_taskManager.OnFinish += GiveMoney;
			_taskManager.OnFinish += GiveExtraReward;
			_taskManager.OnFinish += Finish;
			_taskManager.OnClear += Clear;
		}

		protected virtual void GiveMoney()
		{
			CurrencyManagerController.Instance.UpdateLabelsEvents();
			_taskManager.OnFinish -= GiveMoney;
		}

		protected virtual void GiveExtraReward()
		{
			CurrencyManagerController.Instance.UpdateLabelsEvents();
			_taskManager.OnFinish -= GiveExtraReward;
		}

		protected virtual void Finish()
		{
			_taskManager.OnFinish -= Finish;
			GPSTracker.CancelGPSDraw();
			_taskManager.OnCancelReward -= CancelExtraReward;
		}

		public virtual void Clear()
		{
			_taskManager.OnClear -= Clear;
		}

		public virtual void CancelExtraReward()
		{
			_taskManager.OnFinish -= GiveExtraReward;
			_taskManager.OnCancelReward -= CancelExtraReward;
		}
	}
}
