using System.Collections.Generic;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Garage.GarageSellService;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ConfirmGarageSellWindow : WindowScript
	{
		[SerializeField]
		private List<CombinedPriceView> CombinedPriceViews = default;

		[SerializeField]
		private List<GameObject> ForRemoveObjects = default;

		[SerializeField]
		private List<GameObject> ForSaleObjects = default;

		[SerializeField]
		private RawImageLoadAsync MainIcon = default;

		[SerializeField]
		private List<TextMeshProUGUI> NameTexts = default;

		[SerializeField]
		private TextMeshProUGUI Capacity = default;

		[SerializeField]
		private TextMeshProUGUI Location = default;

		[SerializeField]
		private TextMeshProUGUI TimerForSaleText = default;

		[SerializeField]
		private TextMeshProUGUI TimerForRemoveText = default;

		[SerializeField]
		private Button ToSecondScreenSell = default;

		[SerializeField]
		private Button ToSecondScreenRemove = default;

		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		private Button SellGarageButton = default;

		[SerializeField]
		private List<Button> CancelButtons = default;

		[SerializeField]
		private Button RemoveButton = default;

		[SerializeField]
		private GameObject FirstScreen = default;

		[SerializeField]
		private GameObject SecondScreen = default;

		private IGarageSellService _garageSellService;

		private GaragePositionsConfig _config;

		private IRemoteConfigManager _remoteConfigManager;

		private bool _isLegacyGarage;

		private bool _forSale;

		private int _index;

		private ConfirmAdditionalControllerConfirmTimer _timer;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(IGarageSellService garageSellService, GaragePositionsConfig config, IRemoteConfigManager remoteConfigManager)
		{
			_garageSellService = garageSellService;
			_remoteConfigManager = remoteConfigManager;
			_config = config;
		}

		protected override void OnEnable()
		{
			SubscribeButtons(subscribe: true);
			FirstScreen?.SetActive(value: false);
			SecondScreen?.SetActive(value: false);
			base.OnEnable();
			BackButton.SetActive(active: false);
		}

		private void OnDisable()
		{
			SubscribeButtons(subscribe: false);
		}

		private void SubscribeButtons(bool subscribe)
		{
			SetListener(SellGarageButton, OnSellClicked, subscribe);
			SetListener(ToSecondScreenSell, OnToSecondScreen, subscribe);
			SetListener(ToSecondScreenRemove, OnToSecondScreen, subscribe);
			SetListener(BackButton, OnCancelClicked, subscribe);
			SetListener(RemoveButton, OnRemoveClicked, subscribe);
			foreach (Button cancelButton in CancelButtons)
			{
				SetListener(cancelButton, OnCancelClicked, subscribe);
			}
		}

		private static void SetListener(Button button, UnityAction action, bool subscribe)
		{
			if (subscribe)
			{
				button.onClick.AddListener(action);
			}
			else
			{
				button.onClick.RemoveListener(action);
			}
		}

		private void Update()
		{
			_timer?.Update();
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			_index = int.Parse(data);
			InitAsync().Forget();
		}

		private async UniTask InitAsync()
		{
			await GetSellAvailable();
			if (_isLegacyGarage)
			{
				await V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true);
				if (!(await _garageSellService.FetchConsumableStatus()))
				{
					V.CoreGameManager.ManagerUI.CloseWaitingWindow();
					OnCancelClicked();
					return;
				}
				_forSale = _garageSellService.IsConsumable;
			}
			else
			{
				_forSale = true;
			}
			UpdateUI(_config.Get(_index));
			InitTimer();
			V.CoreGameManager.ManagerUI.CloseWaitingWindow();
			FirstScreen.SetActive(value: true);
		}

		private void UpdateUI(GarageData garageData)
		{
			foreach (CombinedPriceView combinedPriceView in CombinedPriceViews)
			{
				combinedPriceView.Init(garageData.SaleCost);
			}
			foreach (TextMeshProUGUI nameText in NameTexts)
			{
				nameText.text = new LocalizedString(garageData.NameLocalizedKey);
			}
			MainIcon.LoadFromAddressables(garageData.GarageIconAddressablePath);
			Capacity.text = garageData.MaxCarSpotCount.ToString();
			Location.text = new LocalizedString(garageData.LocationKey);
			SetObjectsActive(ForRemoveObjects, !_forSale);
			SetObjectsActive(ForSaleObjects, _forSale);
		}

		private void InitTimer()
		{
			TextMeshProUGUI timeText = (_forSale ? TimerForSaleText : TimerForRemoveText);
			Button confirmButton = (_forSale ? ToSecondScreenSell : ToSecondScreenRemove);
			_timer = new ConfirmAdditionalControllerConfirmTimer(10f, timeText, confirmButton, showConfirmText: false);
		}

		private static void SetObjectsActive(List<GameObject> objects, bool active)
		{
			foreach (GameObject @object in objects)
			{
				@object.SetActive(active);
			}
		}

		private void OnToSecondScreen()
		{
			FirstScreen?.SetActive(value: false);
			SecondScreen?.SetActive(value: true);
		}

		private void OnSellClicked()
		{
			_garageSellService.SellOrRemove(_index, GaragePropertyActionType.SELL).Forget();
			Close();
		}

		private void OnRemoveClicked()
		{
			_garageSellService.SellOrRemove(_index, GaragePropertyActionType.REMOVE).Forget();
			Close();
		}

		private void OnCancelClicked()
		{
			_managerUI.ShowMainMenuWindow();
			Close();
		}

		private async UniTask GetSellAvailable()
		{
			_isLegacyGarage = (await _remoteConfigManager.GetConfigValuesStorage()).legacyGarage;
		}
	}
}
