using CPM.Database;
using UnityEngine;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.CarMaintenance
{
	public abstract class MaintenanceServiceBaseSO : ScriptableObject, ICarMaintenanceService
	{
		protected enum ExecutionType
		{
			Continuous = 0,
			Instant = 1
		}

		[TextArea]
		public string description;

		[SerializeField]
		protected float _serviceCost = 5000f;

		[SerializeField]
		protected ExecutionType _serviceProvideType;

		protected float _executionDuration;

		protected MaintenanceService _maintenanceService;

		public abstract bool IsServiceConditionsMet(out ServiceMessages messages);

		public virtual void Init(MaintenanceService maintenanceService)
		{
			_maintenanceService = maintenanceService;
			_executionDuration = maintenanceService.ExecutionDuration;
		}

		public abstract void Execute(IPricesService pricesService, CurrencyManagerController currencyManagerController, FreeDriveDB freeDriveDB);
	}
}
