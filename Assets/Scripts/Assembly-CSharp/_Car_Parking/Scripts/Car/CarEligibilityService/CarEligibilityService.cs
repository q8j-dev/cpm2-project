using CPM.Database;
using CPMEngine;
using Unity.Mathematics;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Car.CarEligibilityService
{
	public class CarEligibilityService : ICarEligibilityService
	{
		private readonly AllCarsInfo _allCarsInfo;

		private readonly ICarInfoService _carInfoService;

		private readonly ITimerTuningService _timerTuningService;

		private readonly ManagerUI _managerUI;

		private readonly IGameFactManager _gameFactManager;

		[Inject]
		public CarEligibilityService(AllCarsInfo allCarsInfo, ICarInfoService carInfoService, ManagerUI managerUI, OptionalDependency<ITimerTuningService> timerTuningService, IGameFactManager gameFactManager)
		{
			_allCarsInfo = allCarsInfo;
			_carInfoService = carInfoService;
			_managerUI = managerUI;
			_timerTuningService = timerTuningService.Value;
			_gameFactManager = gameFactManager;
		}

		public bool CanUseCar()
		{
			if (!_gameFactManager.IsTrue(GameFact.Carless) && !V.CoreGameManager.HasFirstCar())
			{
				V.CoreGameManager.SetMessage(new LocalizedString("Mainmenuwindow.YouNeedCar"), 3f);
				return false;
			}
			if ((bool)V.LastUsedCar)
			{
				CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
				if (V.LastUsedCar.TryGetComponent<VehicleEntityProvider>(out var component))
				{
					int2 notValid = Utils.NotValid;
					component.CheckerEngineCheating(out var res);
					if (math.all(res == notValid))
					{
						_managerUI.ShowCarBlockNotification(NotificationMessages.CheatEngine, 3f, NotificationType.Error);
						return false;
					}
					component.CheckerPartsInEngine(out var partsLine, out var isIncompatible);
					if (math.all(isIncompatible == Utils.NotValid))
					{
						_managerUI.ShowCarBlockNotification(NotificationMessages.IncompatibleParts, 3f, NotificationType.Error, partsLine);
						return false;
					}
				}
				_allCarsInfo.ResolveCarMeta(fromSlot.CarID, out var result);
				if (math.all(result == Utils.NotValid))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CantDriveThisCar, 3f, NotificationType.Error);
					return false;
				}
				if (component.CarEntity.GetValue().IsBroken)
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CarIsBroken, 3f, NotificationType.Error);
					return false;
				}
				if (_timerTuningService != null)
				{
					_timerTuningService.IsCarBlocked(V.SlotID).Get(out var value);
					if (value)
					{
						_managerUI.ShowCarBlockNotification(NotificationMessages.CarInService, 3f, NotificationType.Error);
						return false;
					}
				}
			}
			return true;
		}
	}
}
