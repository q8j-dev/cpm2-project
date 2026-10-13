using System;
using System.Threading;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _Car_Parking.Scripts.VinylsAi
{
	public class PlayerVinylCensorChecker : IDisposable
	{
		private VinylsCheckApi _vinylsCheckApi;

		private CancellationTokenSource _cancellationTokenCheckCurrentCar;

		private bool _isChecking;

		[Inject]
		public PlayerVinylCensorChecker(VinylsCheckApi vinylsCheckApi)
		{
			_vinylsCheckApi = vinylsCheckApi;
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenCheckCurrentCar);
		}

		public void CheckCurrentCar()
		{
			if ((bool)V.Car)
			{
				CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenCheckCurrentCar);
				_cancellationTokenCheckCurrentCar = new CancellationTokenSource();
				CheckCurrentCarAsync(_cancellationTokenCheckCurrentCar.Token).Forget();
			}
		}

		private async UniTaskVoid CheckCurrentCarAsync(CancellationToken cancellationToken)
		{
			await UniTask.Yield();
			while (_isChecking)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.5));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			_isChecking = true;
			VehicleData vehicleData = V.Car.GetComponent<VehicleData>();
			vehicleData.carInfo.OnVinylUpdated();
			if (vehicleData != null && vehicleData.carInfo.GetNeedToCheckAdultVinyls())
			{
				vehicleData.carInfo.SetHaveAdultVinyls(value: true);
				bool haveAdultVinyls = false;
				VinylsCensorChecker vinylsCensorChecker = new VinylsCensorChecker(_vinylsCheckApi, vehicleData);
				foreach (VinylsType value in Enum.GetValues(typeof(VinylsType)))
				{
					if (vehicleData.carInfo.TryGetCarVinyls(value, out var _))
					{
						if (!(await vinylsCensorChecker.IsVinylSafe(value)))
						{
							haveAdultVinyls = true;
							break;
						}
						if (cancellationToken.IsCancellationRequested)
						{
							break;
						}
						await UniTask.Delay(TimeSpan.FromSeconds(0.5));
					}
				}
				if (!cancellationToken.IsCancellationRequested)
				{
					vehicleData.carInfo.SetNeedToCheckAdultVinyls(value: false);
					vehicleData.carInfo.SetHaveAdultVinyls(haveAdultVinyls);
				}
			}
			_isChecking = false;
		}
	}
}
