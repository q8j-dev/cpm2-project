using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.MenuUI;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public class SellExchangeFreeHelperDeleteCar
	{
		private readonly ISlotService _slotService;

		private readonly IDatabaseCarsService _databaseCarsService;

		private readonly LockCarButtonController _lockCarButtonController;

		public SellExchangeFreeHelperDeleteCar(ISlotService slotService, IDatabaseCarsService databaseCarsService, LockCarButtonController lockCarButtonController)
		{
			_slotService = slotService;
			_databaseCarsService = databaseCarsService;
			_lockCarButtonController = lockCarButtonController;
		}

		public void DeleteCarAfterSellExchange(VehicleData vehicleData)
		{
			vehicleData.IsTraded = new AdvancedBoolean(v: true);
			CarSlot slot = _slotService.GetSlot(vehicleData.slotId);
			_databaseCarsService.DeleteCarFromLocalData(slot, deleteFromProgressService: false);
			vehicleData.slotId = -1;
			_lockCarButtonController.UnLockCar(vehicleData, showMessage: false);
		}
	}
}
