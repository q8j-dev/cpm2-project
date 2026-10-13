using CPM.Database;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Notification;

namespace _Car_Parking.Scripts.EventSystem.EventService.Processes
{
	public class SuitProcessor : RewardProcessor
	{
		private readonly INotificationService _notificationService;

		private CommonProvider _commonProvider;

		public SuitProcessor(EventsAnalytic eventsAnalytic, INotificationService notificationService, CommonProvider commonProvider)
			: base(eventsAnalytic)
		{
			_notificationService = notificationService;
			_commonProvider = commonProvider;
		}

		public override UniTask Execute(RewardResponseBase responseBase, string eventId)
		{
			if (responseBase is RewardEquipmentResponse rewardEquipmentResponse)
			{
				EquipmentsData equipmentsData = _commonProvider.CharactersService.GetEquipmentsData(rewardEquipmentResponse.BodyType);
				if (!equipmentsData.IsItemBought(rewardEquipmentResponse.TypeId, rewardEquipmentResponse.Id))
				{
					equipmentsData.BuyItem(rewardEquipmentResponse.TypeId, rewardEquipmentResponse.Id);
					_notificationService.SetNotice(rewardEquipmentResponse.TypeId, rewardEquipmentResponse.Id, rewardEquipmentResponse.BodyType);
					EventsAnalytic.LogClaimReward(eventId, rewardRecieved: true, rewardEquipmentResponse.Id.ToString(), responseBase.Type);
				}
			}
			return default;
		}
	}
}
