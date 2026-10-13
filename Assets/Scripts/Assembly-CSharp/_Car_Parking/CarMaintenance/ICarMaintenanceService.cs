using CPM.Database;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.CarMaintenance
{
	public interface ICarMaintenanceService
	{
		bool IsServiceConditionsMet(out ServiceMessages messages);

		void Init(MaintenanceService maintenanceService);

		void Execute(IPricesService pricesService, CurrencyManagerController currencyManagerController, FreeDriveDB freeDriveDB);
	}
}
