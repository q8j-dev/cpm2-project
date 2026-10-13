using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.EventSystem.View;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class EventViewWindow : WindowScript, IBackableWindow
	{
		public class AdditionalData
		{
			public IBigBannerView.BannerData ActiveBannerData;

			public string PreviousWindow;

			public bool SingleOfferOnly;

			public AdditionalData()
			{
			}

			public AdditionalData(IBigBannerView.BannerData activeBannerData)
			{
				ActiveBannerData = activeBannerData;
			}
		}

		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private SnapController SnapController;

		private ParkingMenuUI _parkingMenuUI;

		private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

		private IEventService _eventService;

		private AdditionalData _additionalData;

		private ISlotService _slotService;

		private string PreviousWindow => _additionalData?.PreviousWindow;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(IEventService eventService, IPrefabFactory prefabFactory, IOfferSystem offerSystem, OptionalDependency<ParkingMenuUI> parkingMenuUI, ISlotService slotService)
		{
			_eventService = eventService;
			_parkingMenuUIDep = parkingMenuUI;
			_slotService = slotService;
			SnapController.Inject(_eventService, prefabFactory, offerSystem);
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			Debug.Log(data);
			_additionalData = JsonConvert.DeserializeObject<AdditionalData>(data);
			AsyncInit().Forget();
		}

		private async UniTask AsyncInit()
		{
			if (_additionalData != null)
			{
				await SnapController.Initialize(_additionalData.ActiveBannerData, _additionalData.SingleOfferOnly);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_parkingMenuUI = _parkingMenuUIDep?.Value;
			_managerUI.CloseBalanceWindow();
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShortNotifySwitchState, base.gameObject);
		}

		protected override void Start()
		{
			InitBackButton();
			base.Start();
		}

		protected override void OnEnable()
		{
			EventView.Claimed = (Action)Delegate.Combine(EventView.Claimed, new Action(OnClaimed));
			EventView.Closed = (Action)Delegate.Combine(EventView.Closed, new Action(OnClosed));
			base.OnEnable();
		}

		private void OnDisable()
		{
			EventView.Claimed = (Action)Delegate.Remove(EventView.Claimed, new Action(OnClaimed));
			EventView.Closed = (Action)Delegate.Remove(EventView.Closed, new Action(OnClosed));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SnapController.Dispose();
		}

		private void OnClosed()
		{
			CheckPreviousWindowAndClose();
		}

		private void OnClaimed()
		{
			if (_additionalData != null)
			{
				_additionalData.PreviousWindow = null;
			}
			CheckPreviousWindowAndClose();
		}

		private void CheckPreviousWindowAndClose()
		{
			if (!string.IsNullOrEmpty(PreviousWindow))
			{
				if (PreviousWindow == "NewCarsWindow")
				{
					WindowScript window = _managerUI.GetWindow("NewCarsWindow");
					if ((bool)window && window.TryGetComponent<TweenerCanvas>(out var component))
					{
						component.Show();
					}
					else
					{
						_managerUI.ShowWindow(PreviousWindow).Forget();
					}
					GameOverlayWindow.AdditionalData additionalData = new GameOverlayWindow.AdditionalData();
					additionalData.OpenMode = GameOverlayWindow.OpenMode.OnlyBalance;
					additionalData.BackButtonMode = (_slotService.HaveCarInAnyGarage ? GameOverlayWindow.BackButtonMode.Show : GameOverlayWindow.BackButtonMode.Hide);
					_managerUI.ShowUserBalanceWindow(null, JsonConvert.SerializeObject(additionalData));
				}
				else if (!(PreviousWindow == "EmoteLockerWindow"))
				{
					_managerUI.ShowWindow(PreviousWindow).Forget();
				}
			}
			if (!_parkingMenuUI && string.IsNullOrEmpty(PreviousWindow))
			{
				_managerUI.ShowMainMenuWindow();
				_managerUI.ShowUserBalanceWindow();
			}
			Close();
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				if (!_parkingMenuUI && ScenesHelper.IsMenuSceneActive() && string.IsNullOrEmpty(PreviousWindow))
				{
					_managerUI.ShowMainMenuWindow();
					_managerUI.ShowUserBalanceWindow();
				}
			}
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			OnClosed();
		}
	}
}
