using System;
using CPM.Database;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Car.Physics.AirSuspension
{
	public interface IAirSuspension
	{
		event Action<CarInfo, bool> OnBoughtUpdated;

		bool IsBought(CarInfo carInfo);

		int GetPrice();

		UniTask<bool> TryBuy(CarInfo carInfo);
	}
}
