using System;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.DataStorageService.MenuNumberPlates
{
	public class MenuPlatesHolder : IInitializable, IDisposable
	{
		private readonly Dictionary<int, VehicleData> _frontPlates = new Dictionary<int, VehicleData>();

		private readonly Dictionary<int, VehicleData> _rearPlates = new Dictionary<int, VehicleData>();

		[Inject]
		public MenuPlatesHolder()
		{
			for (int i = 1; i < 7; i++)
			{
				_frontPlates.Add(i, null);
				_rearPlates.Add(i, null);
			}
		}

		public void Initialize()
		{
			GlobalEventSystem.OnPlateCreated -= PlateCreatedHandler;
			GlobalEventSystem.OnPlateCreated += PlateCreatedHandler;
		}

		private void PlateCreatedHandler(int plateId, VehicleData vehicleData, bool isFront)
		{
			if (isFront)
			{
				if ((bool)_frontPlates[plateId])
				{
					_frontPlates[plateId].VinylsComposition.DeleteFrontPlate();
				}
				_frontPlates[plateId] = vehicleData;
			}
			else
			{
				if ((bool)_rearPlates[plateId])
				{
					_rearPlates[plateId].VinylsComposition.DeleteRearPlate();
				}
				_rearPlates[plateId] = vehicleData;
			}
		}

		public void Dispose()
		{
			GlobalEventSystem.OnPlateCreated -= PlateCreatedHandler;
			_frontPlates.Clear();
			_rearPlates.Clear();
		}
	}
}
