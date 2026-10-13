using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	public class CarSpeedLimitIcon : MonoBehaviour
	{
		private const int UpdateFrame = 10;

		[SerializeField]
		private GameObject Icon;

		private VehicleData _currentVehicleData;

		private MultiPlayerPosControl _currentMultiPlayerPosControl;

		private GameObject _carPlayer;

		private VehicleData _vehicleDataPlayer;

		private CancellationTokenSource _cancellationTokenSource;

		private void OnEnable()
		{
			Icon.SetActive(value: false);
			_currentVehicleData = GetComponentInParent<VehicleData>();
			if ((bool)_currentVehicleData)
			{
				_currentMultiPlayerPosControl = _currentVehicleData.gameObject.GetComponent<MultiPlayerPosControl>();
			}
			_cancellationTokenSource = new CancellationTokenSource();
			UpdateIcon(_cancellationTokenSource.Token).Forget();
		}

		private void OnDisable()
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
			_cancellationTokenSource = null;
		}

		private async UniTaskVoid UpdateIcon(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (!FlagsStorage.IsSpectatorMode && (bool)_currentVehicleData && !_currentVehicleData.IsMultiplayerAndMine && (bool)_currentMultiPlayerPosControl)
				{
					if (_carPlayer != V.Car)
					{
						_vehicleDataPlayer = null;
						_carPlayer = V.Car;
						if ((bool)_carPlayer)
						{
							_vehicleDataPlayer = _carPlayer.GetComponent<VehicleData>();
						}
					}
					if (!_vehicleDataPlayer || !_vehicleDataPlayer.CarInfoProvider.CarInfo.AdditionalData.Police.Installed)
					{
						Icon.SetActive(value: false);
						break;
					}
					Icon.SetActive(_currentMultiPlayerPosControl.speeding);
				}
				else
				{
					Icon.SetActive(value: false);
				}
				await UniTask.DelayFrame(10, PlayerLoopTiming.Update, cancellationToken);
			}
		}
	}
}
