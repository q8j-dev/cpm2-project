using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Service.Cargo
{
	public class FinishCollisionDetector : MonoBehaviour
	{
		private int parkingTriggers;

		private CancellationTokenSource _cts;

		public int ParkingTriggers
		{
			get
			{
				return parkingTriggers;
			}
			set
			{
				parkingTriggers = value;
				if (value == 4)
				{
					FinishTask();
				}
			}
		}

		private void OnEnable()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = new CancellationTokenSource();
			RaycastRoutine(_cts.Token).Forget();
		}

		private void OnDisable()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
		}

		private void OnDestroy()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
		}

		private async UniTaskVoid RaycastRoutine(CancellationToken cancellationToken)
		{
			while (Vector3.Distance(V.DriverPerson.transform.position, base.transform.position) > 100f)
			{
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			Raycast();
		}

		private void Raycast()
		{
			if (Physics.Raycast(base.transform.position, Vector3.down, out var hitInfo, 2f, -1, QueryTriggerInteraction.Ignore))
			{
				base.transform.parent.position += Vector3.down * (hitInfo.distance - 0.9f);
			}
		}

		private void FinishTask()
		{
			Global.TaskManager.FinishTask();
			Global.TaskManager.Clear();
		}
	}
}
