using Cysharp.Threading.Tasks;
using Service.Abstract;
using Service.Manager;
using UnityEngine;

namespace Service.Delivery
{
	public class FinishCollisionDetector : CollisionDetector
	{
		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

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
			_taskManager.FinishTask();
			_taskManager.Clear();
			return true;
		}
	}
}
