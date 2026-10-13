using System;
using CPM.Database;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Car.Physics.AirSuspension
{
	public abstract class AirSuspensionBase : IAirSuspension, IDisposable
	{
		public event Action<CarInfo, bool> OnBoughtUpdated;

		public void Dispose()
		{
		}

		public abstract bool IsBought(CarInfo carInfo);

		public abstract int GetPrice();

		public abstract UniTask<bool> TryBuy(CarInfo carInfo);

		protected void BoughtUpdated(CarInfo carInfo)
		{
			OnBoughtUpdated?.Invoke(carInfo, IsBought(carInfo));
		}
	}
}
