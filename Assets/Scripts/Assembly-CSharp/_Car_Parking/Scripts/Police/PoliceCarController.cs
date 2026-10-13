using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarController : MonoBehaviour
	{
		public PoliceCarBodyKitsUpdater BodyKitsUpdater;

		public PoliceCarBodyKitsSetter BodyKitsSetter;

		public PoliceCarSirenControllerInTuning PoliceCarSirenControllerInTuning;

		public PoliceCarSirenControllerInGameplay PoliceCarSirenControllerInGameplay;

		public PoliceCarSirenMuteController MuteController = new PoliceCarSirenMuteController();

		[SerializeField]
		private PoliceCarSirenController PoliceCarSirenController;

		[SerializeField]
		private CarSpeedLimitIcon CarSpeedLimitIcon;

		private VehicleData _vehicleData;

		private CancellationTokenSource _cancellationInitCarSpeedLimitSource;

		public void Init(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
			BodyKitsUpdater.Init(_vehicleData);
			BodyKitsSetter.Init(_vehicleData);
			PoliceCarSirenControllerInTuning.Init(_vehicleData);
			PoliceCarSirenController.Init(_vehicleData);
			PoliceCarSirenControllerInGameplay.Init(_vehicleData);
			MuteController.Init(_vehicleData);
			if (FlagsStorage.IsMultiFree)
			{
				_cancellationInitCarSpeedLimitSource = new CancellationTokenSource();
				InitCarSpeedLimitIconAsync(_cancellationInitCarSpeedLimitSource.Token).Forget();
			}
		}

		private void OnDestroy()
		{
			_cancellationInitCarSpeedLimitSource?.Cancel();
			_cancellationInitCarSpeedLimitSource?.Dispose();
			_cancellationInitCarSpeedLimitSource = null;
		}

		private async UniTaskVoid InitCarSpeedLimitIconAsync(CancellationToken cancellationToken)
		{
			await UniTask.DelayFrame(1000, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested && (bool)V.CoreGameManager && (bool)_vehicleData)
			{
				CarSpeedLimitIcon.gameObject.SetActive(value: true);
				CarSpeedLimitIcon.transform.localPosition = new Vector3(0f, _vehicleData.RoofPosition + 0.5f, 0f);
			}
		}
	}
}
