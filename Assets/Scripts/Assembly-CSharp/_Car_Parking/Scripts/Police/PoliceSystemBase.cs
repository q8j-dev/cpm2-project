using System;
using CPM.Database;

namespace _Car_Parking.Scripts.Police
{
	public abstract class PoliceSystemBase : IPoliceSystem, IDisposable
	{
		public const string ResourcesFolder = "PoliceSystem";

		public const string PoliceCarPrefabPath = "PoliceSystem/PoliceCar";

		protected PoliceCarBoughtBodyKit boughtBodyKit;

		protected PoliceCarBoughtSiren boughtSiren;

		private PoliceSystemConfig _policeSystemConfig;

		public PoliceSystemConfig PoliceSystemConfig => _policeSystemConfig;

		public PoliceCarBoughtBodyKit BoughtBodyKit => boughtBodyKit;

		public PoliceCarBoughtSiren BoughtSiren => boughtSiren;

		public event Action<CarInfo, bool> OnBoughtUpdated;

		public PoliceSystemBase()
		{
		}

		public PoliceSystemBase(PoliceSystemConfig policeSystemConfig)
		{
			boughtBodyKit = new PoliceCarBoughtBodyKit();
			boughtSiren = new PoliceCarBoughtSiren();
			_policeSystemConfig = policeSystemConfig;
		}

		public void Dispose()
		{
		}

		public abstract bool IsBought(CarInfo carInfo);

		public abstract void TryBuy(CarInfo carInfo);

		public void SaveInstalledData(CarInfo carInfo)
		{
			boughtBodyKit.SaveInstalledBodyKitsFromCarInfo(carInfo);
			boughtSiren.SaveInstalledBodyKitsFromCarInfo(carInfo);
		}

		protected void BoughtUpdated(CarInfo carInfo)
		{
			OnBoughtUpdated?.Invoke(carInfo, IsBought(carInfo));
		}
	}
}
