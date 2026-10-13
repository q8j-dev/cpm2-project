using System;
using CPM.Database;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Car
{
	public class CarInfoProvider
	{
		private VehicleData _vehicleData;

		private CarInfoContainer _carInfoContainer;

		public bool IsReady
		{
			get
			{
				if (!_vehicleData || _vehicleData.slotId == -1)
				{
					return _carInfoContainer;
				}
				return true;
			}
		}

		public CarInfo CarInfo
		{
			get
			{
				if ((bool)_carInfoContainer)
				{
					return _carInfoContainer.CarInfo;
				}
				if (!_vehicleData)
				{
					return null;
				}
				return _vehicleData.carInfo;
			}
		}

		public event Action<CarInfoProvider, CarInfo> OnReady;

		public CarInfoProvider(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
		}

		public void SetCarInfoContainer(CarInfoContainer carInfoContainer)
		{
			_carInfoContainer = carInfoContainer;
			OnReady?.Invoke(this, CarInfo);
		}

		public void OnSlotIdUpdated()
		{
			OnReady?.Invoke(this, CarInfo);
		}
	}
}
