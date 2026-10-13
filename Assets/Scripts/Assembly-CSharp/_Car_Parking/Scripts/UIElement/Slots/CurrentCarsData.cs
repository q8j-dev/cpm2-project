using System.Collections.Generic;
using CPM.Scripts.Garage;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class CurrentCarsData
	{
		private class VehicleAndCarInstanceIdData
		{
			public VehicleData VehicleData;

			public string CarInstanceId;

			public VehicleAndCarInstanceIdData(VehicleData vehicleData, string carInstanceId)
			{
				VehicleData = vehicleData;
				CarInstanceId = carInstanceId;
			}
		}

		private List<VehicleAndCarInstanceIdData> _currentCars = new List<VehicleAndCarInstanceIdData>();

		public CurrentCarsData(SlotCollection slotCollection, IGarageEntity currentGarage)
		{
			for (int i = 0; i < slotCollection.MaxCarSpotCount; i++)
			{
				CarSlot carSlot = slotCollection[i];
				if (carSlot != null)
				{
					VehicleData carBySpot = currentGarage.GetCarBySpot(carSlot.ID);
					if (!(carBySpot == null))
					{
						_currentCars.Add(new VehicleAndCarInstanceIdData(carBySpot, carBySpot.carInfo.CarInstanceId));
					}
				}
			}
		}

		public List<string> GetAllCarInstanceIds()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < _currentCars.Count; i++)
			{
				list.Add(_currentCars[i].CarInstanceId);
			}
			return list;
		}

		public string GetCarInstanceId(VehicleData vehicleData)
		{
			for (int i = 0; i < _currentCars.Count; i++)
			{
				if ((bool)_currentCars[i].VehicleData && (bool)vehicleData && _currentCars[i].VehicleData == vehicleData)
				{
					return _currentCars[i].CarInstanceId;
				}
			}
			return null;
		}

		public VehicleData GetCar(string carInstanceId)
		{
			for (int i = 0; i < _currentCars.Count; i++)
			{
				if ((bool)_currentCars[i].VehicleData && _currentCars[i].CarInstanceId == carInstanceId)
				{
					return _currentCars[i].VehicleData;
				}
			}
			return null;
		}

		public void RemoveCar(string carInstanceId)
		{
			for (int i = 0; i < _currentCars.Count; i++)
			{
				if (_currentCars[i].CarInstanceId == carInstanceId)
				{
					_currentCars.RemoveAt(i);
					break;
				}
			}
		}
	}
}
