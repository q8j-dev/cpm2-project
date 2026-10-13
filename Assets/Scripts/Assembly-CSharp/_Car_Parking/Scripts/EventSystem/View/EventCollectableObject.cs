using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class EventCollectableObject : CollectableGameObjectClaim
	{
		[SerializeField]
		private int ID;

		private IEventService _eventService;

		private EventType _eventType;

		private UiNotificationsManager _uiNotificationsManager;

		private EventCollectablesPopupConfig _eventCollectablesPopupConfig;

		public int CollectableID => ID;

		[Inject]
		public void Construct(IEventService eventService, EventCollectablesPopupConfig eventCollectablesPopupConfig, OptionalDependency<UiNotificationsManager> uiNotificationsManager)
		{
			_eventService = eventService;
			_uiNotificationsManager = uiNotificationsManager.Value;
			_eventCollectablesPopupConfig = eventCollectablesPopupConfig;
		}

		public void Init(EventType eventType)
		{
			_eventType = eventType;
		}

		public override void Claim(CollectableGameObject collectableGameObject)
		{
			base.gameObject.SetActive(value: false);
			SubmitResultAsync(ID, base.destroyCancellationToken).Forget();
		}

		private async UniTask SubmitResultAsync(int itemID, CancellationToken cancellationToken)
		{
			CollectableData data = new CollectableData
			{
				Item = itemID
			};
			int num = await _eventService.SubmitEventRecord(_eventType, data);
			if (cancellationToken.IsCancellationRequested || num == 0)
			{
				return;
			}
			await _eventService.RetrieveCurrentEvents(forceRefresh: true);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(_eventType);
			if (eventByName != null && eventByName.Status == EventStatus.Finished)
			{
				_eventService.RemoveCacheEventByName(eventByName.Type);
			}
			EventCollectablesPopupConfig.PopupData popupData = _eventCollectablesPopupConfig.GetPopupData(_eventType);
			if (_uiNotificationsManager != null && eventByName != null && eventByName.Data is CollectablesEventData && popupData != null)
			{
				CollectablesEventData collectablesEventData = eventByName.Data as CollectablesEventData;
				PopupData popupData2 = new PopupData();
				popupData2.Delay = popupData.DelayToShow;
				popupData2.Description = $"{collectablesEventData.Items.Count}/{collectablesEventData.ItemCount}";
				popupData2.PopupKey = popupData.PopupKey;
				if (!string.IsNullOrEmpty(popupData.AudioKey))
				{
					popupData2.AudioKey = AudioWrapper.GetId(popupData.AudioKey);
				}
				_uiNotificationsManager.ShowPopup(popupData2);
			}
		}
	}
}
