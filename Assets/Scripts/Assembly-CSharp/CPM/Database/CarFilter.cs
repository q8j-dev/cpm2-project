using System;
using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace CPM.Database
{
	public class CarFilter
	{
		private List<VehicleStockTuningData> _cars;

		private IOfferSystem _offerSystem;

		public CarFilter(List<VehicleStockTuningData> cars, IOfferSystem offerSystem)
		{
			_cars = cars;
			_offerSystem = offerSystem;
		}

		public Func<IEnumerable<VehicleStockTuningData>> CreateFilter(CarRegion? region = null, CarType? carType = null, VehicleClass? vehicleClass = null, bool checkSpecial = false)
		{
			return () =>
			{
				IEnumerable<VehicleStockTuningData> enumerable = _cars;
				if (checkSpecial && _offerSystem != null)
				{
					enumerable = enumerable.Where((VehicleStockTuningData car) => car.PurchaseItemType != PurchaseItemType.None || _offerSystem.GetOffersWithCarId(car.Serial).Count > 0);
				}
				else if (!carType.HasValue || carType != CarType.NewCars)
				{
					enumerable = enumerable.Where((VehicleStockTuningData car) => car.PurchaseItemType == PurchaseItemType.None && _offerSystem.GetOffersWithCarId(car.Serial).Count == 0);
				}
				if (region.HasValue)
				{
					enumerable = enumerable.Where((VehicleStockTuningData car) => car.region == region.Value);
				}
				if (carType.HasValue)
				{
					enumerable = enumerable.Where((VehicleStockTuningData car) => car.CarTypes.Any((CarType type) => type == carType.Value));
				}
				if (vehicleClass.HasValue)
				{
					enumerable = enumerable.Where((VehicleStockTuningData car) => car.GetClass == vehicleClass.Value);
				}
				return enumerable;
			};
		}
	}
}
