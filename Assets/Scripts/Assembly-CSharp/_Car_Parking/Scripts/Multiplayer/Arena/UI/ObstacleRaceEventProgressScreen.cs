using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.EventSystem.View;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.UIElement.StepProgress;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public class ObstacleRaceEventProgressScreen : WindowScriptAsyncResult<EmptyData>
	{
		[SerializeField]
		private Button CloseButton = default;

		[SerializeField]
		private Button ClaimRewardButton = default;

		[SerializeField]
		private TMP_Text ClaimedText = default;

		[SerializeField]
		private StepProgressView Progress = default;

		[SerializeField]
		private GameObject GiftIcon = default;

		[SerializeField]
		private RawImageLoadAsync PrizeImage = default;

		private IEventService _eventService;

		private ObstacleRacePresentsPresenter _presents;

		private bool _claiming;

		private ObstacleRacePresentsPresenter Presents => _presents ?? (_presents = new ObstacleRacePresentsPresenter(Progress, ClaimedText, ClaimRewardButton));

		[Inject]
		public void Construct(IEventService eventService)
		{
			_eventService = eventService;
		}

		protected override void Start()
		{
			base.Start();
			if ((bool)CloseButton)
			{
				CloseButton.onClick.AddListener(Close);
			}
			if ((bool)ClaimRewardButton)
			{
				ClaimRewardButton.onClick.AddListener(OnClaim);
			}
			if (_eventService != null)
			{
				_eventService.OnEventUpdated += EventUpdated;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ((bool)CloseButton)
			{
				CloseButton.onClick.RemoveListener(Close);
			}
			if ((bool)ClaimRewardButton)
			{
				ClaimRewardButton.onClick.RemoveListener(OnClaim);
			}
			if (_eventService != null)
			{
				_eventService.OnEventUpdated -= EventUpdated;
			}
		}

		public void Init(ObstacleRaceEventProgress gained)
		{
			Presents.PlayAsync(gained.Before, gained.After, CurrentData(), this.GetCancellationTokenOnDestroy()).Forget();
		}

		private void EventUpdated(_Car_Parking.Scripts.EventSystem.EventType eventType)
		{
			if (eventType == _Car_Parking.Scripts.EventSystem.EventType.ObstacleRace)
			{
				_Car_Parking.Scripts.EventSystem.Data.Event obj = CurrentEvent();
				if (obj != null)
				{
					Presents.Render(obj.Progress, obj.Data as ObstacleRaceEventData);
				}
			}
		}

		private void OnClaim()
		{
			if (_eventService != null && !_claiming)
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ClaimRewardSwitchState, base.gameObject);
				ClaimAsync().Forget();
			}
		}

		private async UniTaskVoid ClaimAsync()
		{
			_claiming = true;
			try
			{
				ClaimRewardResult claimRewardResult = await _eventService.ClaimReward(_Car_Parking.Scripts.EventSystem.EventType.ObstacleRace);
				if ((bool)this && claimRewardResult.Success)
				{
					ShowGrantedPrize(claimRewardResult.Rewards);
				}
			}
			finally
			{
				_claiming = false;
			}
		}

		private void ShowGrantedPrize(RewardResponseBase[] rewards)
		{
			string text = EquipmentIconPath(rewards);
			if (!string.IsNullOrEmpty(text) && (bool)PrizeImage)
			{
				if ((bool)GiftIcon)
				{
					GiftIcon.SetActive(value: false);
				}
				PrizeImage.gameObject.SetActive(value: true);
				PrizeImage.Load(text);
			}
		}

		private static string EquipmentIconPath(RewardResponseBase[] rewards)
		{
			if (rewards == null)
			{
				return null;
			}
			for (int i = 0; i < rewards.Length; i++)
			{
				if (rewards[i] is RewardEquipmentResponse rewardEquipmentResponse)
				{
					return CharactersService.GetIconPathForKit(rewardEquipmentResponse.BodyType, rewardEquipmentResponse.TypeId, rewardEquipmentResponse.Id);
				}
			}
			return null;
		}

		private _Car_Parking.Scripts.EventSystem.Data.Event CurrentEvent()
		{
			return _eventService?.GetEventByName(_Car_Parking.Scripts.EventSystem.EventType.ObstacleRace);
		}

		private ObstacleRaceEventData CurrentData()
		{
			return CurrentEvent()?.Data as ObstacleRaceEventData;
		}
	}
}
