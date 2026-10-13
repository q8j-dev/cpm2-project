using System;
using System.Threading;
using CPM.Database;
using CPM.Scripts.Garage;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car.Physics.AirSuspension;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class SuspensionTuningWindow : WindowScript, IBackableWindow
	{
		private const float HideDuration = 0.5f;

		private const float SpacerWidthMinVal = 0f;

		private const float SpacerWidthMaxVal = 15f;

		[SerializeField]
		private CanvasGroup[] InteractablePanels;

		[SerializeField]
		private CanvasGroup LeftPanelCanvasGroup;

		[SerializeField]
		[Header("Canvas group")]
		private AlphaHidableObject[] AlphaHidableObjects;

		[SerializeField]
		[Header("Image panel")]
		private TextMeshProUGUI TopLeftImageText;

		[SerializeField]
		private Image BgImage;

		[SerializeField]
		private TextMeshProUGUI TopCenterImageText;

		[SerializeField]
		private TextMeshProUGUI TopRightImageText;

		[SerializeField]
		private Sprite[] SpritesArray;

		[SerializeField]
		private Image RightImage;

		[SerializeField]
		private TextMeshProUGUI DownLeftValueText;

		[SerializeField]
		private TextMeshProUGUI RightLeftValueText;

		[SerializeField]
		private TextMeshProUGUI Title;

		[SerializeField]
		private TextMeshProUGUI Description;

		[SerializeField]
		[Header("Buttons")]
		private Button ApplyButton;

		[SerializeField]
		private Button DontSaveButton;

		[SerializeField]
		private Button SaveButton;

		[SerializeField]
		private Button ResetButton;

		[SerializeField]
		private float AnimationDuration = 1f;

		[Header("Sliders")]
		[SerializeField]
		private CustomSliderValueHandler[] FrontSliderValueHandlers;

		[SerializeField]
		private CustomSliderValueHandler[] RearSliderValueHandlers;

		[SerializeField]
		private CustomSliderValueHandler[] BothSliderValueHandlers;

		[Header("TopSliders")]
		[SerializeField]
		private TextMeshProUGUI TopTitleText;

		[Header("Panels")]
		[SerializeField]
		private GameObject FrontPanel;

		[SerializeField]
		private GameObject RearPanel;

		[SerializeField]
		private GameObject OtherPanel;

		[SerializeField]
		private Toggle FrontToggle;

		[SerializeField]
		private Toggle RearToggle;

		[SerializeField]
		[Header("Panels")]
		private GameObject MainPanel;

		[SerializeField]
		private GameObject PopupPanel;

		[SerializeField]
		private ScrollRect LeftScrollRect;

		[SerializeField]
		private GameObject TopBarTitleButtonsPanel;

		[SerializeField]
		[Header("Pneumatic")]
		private GameObject SuspensionTypeChoosePanel;

		[SerializeField]
		private GameObject PneumaticCheckbox;

		[SerializeField]
		private GameObject SpringCheckbox;

		[SerializeField]
		private Toggle PneumaticToggle;

		[SerializeField]
		private Toggle SpringToggle;

		[SerializeField]
		private GameObject PneumaticBuyPanel;

		[SerializeField]
		private TextMeshProUGUI PneumaticPriceText;

		[SerializeField]
		private Button PneumaticBuyButton;

		[Header("Header elements")]
		[SerializeField]
		private TextMeshProUGUI SpringsName;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private GarageController _garageController;

		private VehicleEntityProvider _vehicleEntityProvider;

		private ICarTuningFactory _carTuningFactory;

		private bool _hasUnsavedChanges;

		private bool _isPneumaticInstalled;

		private SuspensionTabs _currentSuspensionTab;

		private Tween _tween;

		private TuningLimitsData _suspensionFrontDataLimits;

		private TuningLimitsData _suspensionRearDataLimits;

		private CarDefaultTuningDataConfig _carDefaultTuningDataConfig;

		private Powertrain _powertrain;

		private Axles _axles;

		private SteerSuspensionData _frontSuspensionData;

		private SuspensionData _rearSuspensionData;

		private float _frontWheelSpacerWidth;

		private float _rearWheelSpacerWidth;

		private ISteamAchievementService _steamAchievementService;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _onNextPreviousHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnNextPrevious);

		private readonly Toggle[] _tabToggles = new Toggle[5];

		private AxleType _currentAxleType;

		private IAirSuspension _airSuspensionSystem;

		private CarInputManager _carInputManager;

		private OptionalDependency<GarageController> _garageControllerDep;

		private IDatabaseCarsService _databaseCarsService;

		private bool _refreshingPneumaticSliders;

		private const float MmPerInch = 25.4f;

		private const float MaxHeightSliderStepInch = 0.05f;

		private const int MaxHeightDecimals = 2;

		[Inject]
		public void Construct(ManagerUI managerUI, ICarInfoService carInfoService, IAirSuspension airSuspensionSystem, OptionalDependency<GarageController> garageController, ICarTuningFactory carTuningFactory, CarInputManager carInputManager, ISteamAchievementService steamAchievementService, EventRegistry eventRegistry, IDatabaseCarsService databaseCarsService)
		{
			_steamAchievementService = steamAchievementService;
			_eventRegistry = eventRegistry;
			_airSuspensionSystem = airSuspensionSystem;
			_carTuningFactory = carTuningFactory;
			_garageControllerDep = garageController;
			_carInfoService = carInfoService;
			_managerUI = managerUI;
			_carInputManager = carInputManager;
			_databaseCarsService = databaseCarsService;
		}

		protected override void Awake()
		{
			_garageController = _garageControllerDep.Value;
			base.Awake();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_onNextPreviousHandle.Get(_eventRegistry).Subscribe(NextPrevious);
		}

		private void OnDisable()
		{
			_onNextPreviousHandle.Get(_eventRegistry)?.Unsubscribe(NextPrevious);
		}

		public void RegisterTabToggle(SuspensionTabs tab, Toggle toggle)
		{
			_tabToggles[(int)tab] = toggle;
		}

		private void NextPrevious(bool next)
		{
			if (!PopupPanel.activeSelf)
			{
				int num = _tabToggles.Length;
				int num2 = (int)(_currentSuspensionTab + (next ? 1 : (num - 1))) % num;
				Toggle toggle = _tabToggles[num2];
				if ((bool)toggle)
				{
					toggle.isOn = true;
				}
			}
		}

		protected override void Start()
		{
			V.Car.GetComponent<VehicleEntityProvider>();
			base.Start();
			InitBackButton();
			_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			int carID = _carInfo.CarID;
			_isPneumaticInstalled = carID != 155 && carID != 165 && _carInfo.AdditionalData.AirSuspension.Installed;
			LocalizeSpringName(_isPneumaticInstalled);
			VehicleStockTuningData vehicleStockTuningData = _carTuningFactory.CreateDefault(_carInfo.CarID);
			_carDefaultTuningDataConfig = vehicleStockTuningData.Tuning;
			_suspensionFrontDataLimits = _carDefaultTuningDataConfig.FrontWheelDataLimits;
			_suspensionRearDataLimits = _carDefaultTuningDataConfig.RearWheelDataLimits;
			Init();
			carID = _carInfo.CarID;
			if (carID == 155 || carID == 165)
			{
				PneumaticToggle.gameObject.SetActive(value: false);
				PneumaticToggle.interactable = false;
				return;
			}
			PneumaticToggle.onValueChanged.AddListener(ShowCurrentSuspensionType);
			PneumaticBuyButton.onClick.AddListener(() =>
			{
				_managerUI.ShowConfirmWindow(() =>
				{
					BuyPneumatic().Forget();
				}, "", new LocalizedString("SuspensionWindow.BuySuspensionDescription"));
			});
		}

		private void OnSliderValueChangedByButton(float value)
		{
			SaveButton.interactable = true;
		}

		private async UniTaskVoid UnsaveChanges()
		{
			_carInfo.TuningData.FrontWheelData.SuspensionData = _frontSuspensionData;
			_carInfo.TuningData.RearWheelData.SuspensionData = _rearSuspensionData;
			_carInfo.TuningData.DynamicTuningSettings.MinFrontSuspensionHeight = -1f;
			_carInfo.TuningData.DynamicTuningSettings.MaxFrontSuspensionHeight = -1f;
			_carInfo.TuningData.DynamicTuningSettings.MinRearSuspensionHeight = -1f;
			_carInfo.TuningData.DynamicTuningSettings.MaxRearSuspensionHeight = -1f;
			_carInfo.TuningData.DynamicTuningSettings.HeightAdjustSpeed = 0.5f;
			_carInfo.TuningData.FrontWheelData.SpacerWidth = _frontWheelSpacerWidth;
			_carInfo.TuningData.RearWheelData.SpacerWidth = _rearWheelSpacerWidth;
			CarEntity.RecalculateSuspension(_vehicleEntityProvider, _carTuningFactory, _carInfo);
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			_managerUI.ShowCarTuningWindow();
			UnsubscribeSliderListeners();
			Close();
		}

		private void CacheData()
		{
			_frontSuspensionData = _carInfo.TuningData.FrontWheelData.SuspensionData.Clone() as SteerSuspensionData;
			_rearSuspensionData = _carInfo.TuningData.RearWheelData.SuspensionData.Clone() as SuspensionData;
			_frontWheelSpacerWidth = _carInfo.TuningData.FrontWheelData.SpacerWidth;
			_rearWheelSpacerWidth = _carInfo.TuningData.RearWheelData.SpacerWidth;
		}

		private void SubscribeSliderToSaveButton(CustomSliderValueHandler[] sliderValueHandlers, bool unsubscribe = false)
		{
			if (sliderValueHandlers == null || sliderValueHandlers.Length == 0)
			{
				return;
			}
			foreach (CustomSliderValueHandler customSliderValueHandler in sliderValueHandlers)
			{
				if (!(customSliderValueHandler == null))
				{
					if (!unsubscribe)
					{
						customSliderValueHandler.OnReleased += OnSliderValueChangedByButton;
					}
					else
					{
						customSliderValueHandler.OnReleased -= OnSliderValueChangedByButton;
					}
				}
			}
		}

		private void SetListenersToHideOnMoveSlider(CustomSliderValueHandler[] sliderValueHandlers, int startIndex)
		{
			for (int i = 0; i < sliderValueHandlers.Length; i++)
			{
				SubscribeListener(startIndex, hide: true, sliderValueHandlers[i]);
				SubscribeListener(startIndex, hide: false, sliderValueHandlers[i]);
				startIndex++;
			}
		}

		private void Init()
		{
			CacheData();
			SaveButton.interactable = false;
			_hasUnsavedChanges = false;
			_vehicleEntityProvider = V.Car.GetComponent<VehicleEntityProvider>();
			_axles = _vehicleEntityProvider.GetComponent<Axles>();
			_powertrain = _vehicleEntityProvider.GetComponent<Powertrain>();
			_axles.SuspensionsEditing = true;
			PneumaticPriceText.text = _airSuspensionSystem.GetPrice().ToString();
			SubscribeSliderToSaveButton(FrontSliderValueHandlers);
			SubscribeSliderToSaveButton(RearSliderValueHandlers);
			SubscribeSliderToSaveButton(BothSliderValueHandlers);
			SetListenersToHideOnMoveSlider(FrontSliderValueHandlers, 0);
			SetListenersToHideOnMoveSlider(RearSliderValueHandlers, 5);
			SetListenersToHideOnMoveSlider(BothSliderValueHandlers, 10);
			AddButtonsListeners();
			EnableWheelsPhysics(enable: true);
			OnTabSelected(SuspensionTabs.Alignment);
			FlagsStorage.IsCarInService = true;
		}

		private void HideAll()
		{
			HideSliders(FrontSliderValueHandlers);
			HideSliders(RearSliderValueHandlers);
			HideSliders(BothSliderValueHandlers);
		}

		private void RemoveAllListeners()
		{
			RemoveListeners(FrontSliderValueHandlers);
			RemoveListeners(RearSliderValueHandlers);
			RemoveListeners(BothSliderValueHandlers);
		}

		private void RemoveListeners(CustomSliderValueHandler[] sliderValueHandlers)
		{
			for (int i = 0; i < sliderValueHandlers.Length; i++)
			{
				sliderValueHandlers[i].RemoveAllListeners();
			}
		}

		private static bool SupportsCamber(SuspensionType type)
		{
			if (type != SuspensionType.Dependent)
			{
				return type != SuspensionType.LeafSpring;
			}
			return false;
		}

		private static bool HasAntiRollBar(TuningLimitsData limits)
		{
			if (limits == null)
			{
				return true;
			}
			if (Mathf.Approximately(limits.AntiSwayBarForceMin, 0f))
			{
				return !Mathf.Approximately(limits.AntiSwayBarForceMax, 0f);
			}
			return true;
		}

		private void ShowNeedSlidersCount(int count, CustomSliderValueHandler[] sliders)
		{
			for (int i = 0; i < count; i++)
			{
				sliders[i].Parent.SetActive(value: true);
			}
		}

		private void HideSliders(CustomSliderValueHandler[] sliderValueHandlers)
		{
			for (int i = 0; i < sliderValueHandlers.Length; i++)
			{
				sliderValueHandlers[i].Parent.gameObject.SetActive(value: false);
			}
		}

		private void SubscribeListener(int id, bool hide, CustomSliderValueHandler subscribeListener)
		{
			if (hide)
			{
				subscribeListener.OnPointerDownAction = () =>
				{
					HideOrShowCanvasGroups(id, hide: true, subscribeListener);
				};
			}
			else
			{
				subscribeListener.OnPointerUpAction = () =>
				{
					HideOrShowCanvasGroups(id, hide: false, subscribeListener);
				};
			}
		}

		private void ShowCurrentSuspensionType(bool isPneumatic)
		{
			DisablePanels();
			HideAll();
			RemoveAllListeners();
			SpringCheckbox.SetActive(!isPneumatic);
			LocalizeSpringName(isPneumatic);
			if (isPneumatic)
			{
				ShowPneumatic();
				if (_carInfo.AdditionalData.AirSuspension.Bought)
				{
					SaveButton.interactable = true;
				}
			}
			else
			{
				ShowSprings();
				SaveButton.interactable = true;
			}
		}

		private void LocalizeSpringName(bool isPneumatic)
		{
			SpringsName.text = (isPneumatic ? new LocalizedString("Suspensionuiwindow.Pneumatic") : new LocalizedString("Suspensionuiwindow.Springs"));
		}

		private async UniTask BuyPneumatic()
		{
			_managerUI.ShowWaitingWindow(set: true).Forget();
			bool num = await _airSuspensionSystem.TryBuy(_carInfo);
			_managerUI.CloseWaitingWindow();
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			if (num)
			{
				PneumaticCheckbox.SetActive(value: true);
				PneumaticBuyPanel.SetActive(value: false);
				SaveButton.interactable = true;
				ResetTab(_currentSuspensionTab);
				CarEntity.RecalculateSuspension(_vehicleEntityProvider, _carTuningFactory, _carInfo);
				ShowPneumatic();
				_isPneumaticInstalled = true;
				Save();
			}
		}

		private void HideOrShowCanvasGroups(int ignoreId, bool hide, CustomSliderValueHandler subscribeListener)
		{
			_tween?.Kill();
			_hasUnsavedChanges = true;
			SaveButton.interactable = true;
			BgImage.enabled = !hide;
			DelayedLockButtons(hide).Forget();
			if (_currentSuspensionTab == SuspensionTabs.Alignment)
			{
				_carInputManager.IgnoreSteerControl(hide);
			}
			for (int i = 0; i < AlphaHidableObjects.Length; i++)
			{
				if (i != ignoreId)
				{
					AlphaHidableObjects[i].ChangeAlphaState(hide ? 0f : 1f, 0.5f);
				}
				else
				{
					AlphaHidableObjects[i].ChangeAlphaState(hide ? 0.5f : 1f, 0.5f, 0f, notIgnoreDependObjects: true);
				}
			}
		}

		public void OnTabSelected(SuspensionTabs suspensionTabs)
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(0f, useAssistent: false);
			}
			_currentSuspensionTab = suspensionTabs;
			DisablePanels();
			HideAll();
			RemoveAllListeners();
			switch (suspensionTabs)
			{
			case SuspensionTabs.Alignment:
				ShowAlignment();
				break;
			case SuspensionTabs.Spacers:
				ShowSpacers();
				break;
			case SuspensionTabs.AntiRollBars:
				ShowAntiRollBars();
				break;
			case SuspensionTabs.Springs:
				HandleSpringTab();
				break;
			case SuspensionTabs.ShockAbsorber:
				ShowShockAbsorber();
				break;
			}
			ReloadLayouts().Forget();
		}

		private async UniTaskVoid DelayedLockButtons(bool hide)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			CanvasGroup[] interactablePanels = InteractablePanels;
			for (int i = 0; i < interactablePanels.Length; i++)
			{
				interactablePanels[i].blocksRaycasts = !hide;
			}
		}

		private void HandleSpringTab()
		{
			_isPneumaticInstalled = _carInfo.AdditionalData.AirSuspension.Installed;
			ActivateSuspensionTypeChoosePanel(isActive: true);
			SpringToggle.isOn = !_isPneumaticInstalled;
			PneumaticToggle.SetIsOnWithoutNotify(_isPneumaticInstalled);
			SpringCheckbox.SetActive(!_isPneumaticInstalled);
			if (_isPneumaticInstalled)
			{
				ShowPneumatic();
			}
			else
			{
				ShowSprings();
			}
		}

		private async UniTaskVoid ReloadLayouts(bool isBought = true)
		{
			CancellationToken token = this.GetCancellationTokenOnDestroy();
			await UniTask.Yield(token);
			await UniTask.Yield(token);
			LeftScrollRect.SetScrollVerticalPosDelayed(1f).Forget();
			LeftPanelCanvasGroup.alpha = 1f;
			LeftPanelCanvasGroup.interactable = isBought;
		}

		private void ActivateSuspensionTypeChoosePanel(bool isActive)
		{
			SuspensionTypeChoosePanel.SetActive(isActive);
		}

		private void HideShowSliderPanels(bool showAll)
		{
			FrontSliderValueHandlers[0].transform.parent.gameObject.SetActive(value: true);
			RearSliderValueHandlers[0].transform.parent.gameObject.SetActive(value: true);
		}

		private void PublishSliderValues(CustomSliderValueHandler sliderHandler, float4 sliderValues, string sliderTitle, string plusTitle = "", string minusTitle = "", bool isRound = false, int numberAfterFloat = 2)
		{
			SetSliderValues(sliderHandler, sliderValues.x, sliderValues.y, sliderValues.z, sliderValues.w, isRound, numberAfterFloat);
			sliderHandler.SetButtonsTitleText(plusTitle, minusTitle);
			sliderHandler.SetSliderTexts(sliderTitle);
		}

		private void ShowAlignment()
		{
			RightImage.sprite = SpritesArray[0];
			ShowNeedSlidersCount(5, FrontSliderValueHandlers);
			ShowNeedSlidersCount(2, RearSliderValueHandlers);
			FrontPanel.SetActive(value: true);
			RearPanel.SetActive(value: true);
			DownLeftValueText.text = Mathf.Round(_carInfo.TuningData.FrontWheelData.SuspensionData.Camber) + "°";
			RightLeftValueText.text = Mathf.Round(_carInfo.TuningData.RearWheelData.SuspensionData.Camber) + "°";
			bool flag = SupportsCamber(_carInfo.TuningData.FrontWheelData.SuspensionData.SuspensionType);
			bool active = SupportsCamber(_carInfo.TuningData.RearWheelData.SuspensionData.SuspensionType);
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.CamberMin, _suspensionFrontDataLimits.CamberMax, _carInfo.TuningData.FrontWheelData.SuspensionData.Camber, 0.1f), sliderHandler: FrontSliderValueHandlers[0], sliderTitle: new LocalizedString("SuspensionTuningWindow.Camber"), plusTitle: flag ? ((string)new LocalizedString("SuspensionTuningWindow.Positive")) : "", minusTitle: flag ? ((string)new LocalizedString("SuspensionTuningWindow.Negative")) : "", isRound: false, numberAfterFloat: 1);
			FrontSliderValueHandlers[0].Parent.SetActive(flag);
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.ToeMin, _suspensionFrontDataLimits.ToeMax, _carInfo.TuningData.FrontWheelData.SuspensionData.ToeIn, 0.01f), sliderHandler: FrontSliderValueHandlers[1], sliderTitle: new LocalizedString("BipedNaming.Toe"), plusTitle: (!flag) ? ((string)new LocalizedString("SuspensionTuningWindow.Positive")) : "", minusTitle: (!flag) ? ((string)new LocalizedString("SuspensionTuningWindow.Negative")) : "");
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.CasterMin, _suspensionFrontDataLimits.CasterMax, _carInfo.TuningData.FrontWheelData.SuspensionData.Caster, 0.1f), sliderHandler: FrontSliderValueHandlers[2], sliderTitle: new LocalizedString("SuspensionTuningWindow.Caster"), plusTitle: "", minusTitle: "", isRound: false, numberAfterFloat: 1);
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.AckermanMin, _suspensionFrontDataLimits.AckermanMax, _carInfo.TuningData.FrontWheelData.SuspensionData.Ackerman, 1f), sliderHandler: FrontSliderValueHandlers[3], sliderTitle: new LocalizedString("SuspensionTuningWindow.Ackerman"), plusTitle: "", minusTitle: "", isRound: true);
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.SteerAngleMin, _suspensionFrontDataLimits.SteerAngleMax, _carInfo.TuningData.FrontWheelData.SuspensionData.MaxSteerAngle, 1f), sliderHandler: FrontSliderValueHandlers[4], sliderTitle: new LocalizedString("SuspensionTuningWindow.SteeringAngle"), plusTitle: "", minusTitle: "", isRound: true);
			float4 sliderValues = new float4(_suspensionRearDataLimits.CamberMin, _suspensionRearDataLimits.CamberMax, _carInfo.TuningData.RearWheelData.SuspensionData.Camber, 0.1f);
			RearSliderValueHandlers[0].Parent.SetActive(active);
			PublishSliderValues(RearSliderValueHandlers[0], sliderValues, new LocalizedString("SuspensionTuningWindow.Camber"), "", "", isRound: false, 1);
			SetTitleAndDescription(new LocalizedString("SuspensionTuningWindow.Alignment.Title"), new LocalizedString("SuspensionTuningWindow.Alignment.Description"));
			PublishSliderValues(sliderValues: new float4(_suspensionRearDataLimits.ToeMin, _suspensionRearDataLimits.ToeMax, _carInfo.TuningData.RearWheelData.SuspensionData.ToeIn, 0.01f), sliderHandler: RearSliderValueHandlers[1], sliderTitle: new LocalizedString("BipedNaming.Toe"), plusTitle: new LocalizedString("SuspensionTuningWindow.Positive"), minusTitle: new LocalizedString("SuspensionTuningWindow.Negative"));
			FrontSliderValueHandlers[0].AddListener(OnFrontChamberChanged);
			FrontSliderValueHandlers[1].AddListener(OnFrontToeChanged);
			FrontSliderValueHandlers[2].AddListener(OnCasterChanged);
			FrontSliderValueHandlers[3].AddListener(OnAckermanChanged);
			FrontSliderValueHandlers[4].AddListener(OnMaxSteeringAngleChanged);
			RearSliderValueHandlers[0].AddListener(OnRearChamberChanged);
			RearSliderValueHandlers[1].AddListener(OnRearToeChanged);
		}

		private void ShowSpacers()
		{
			RightImage.sprite = SpritesArray[1];
			ShowNeedSlidersCount(1, FrontSliderValueHandlers);
			ShowNeedSlidersCount(1, RearSliderValueHandlers);
			FrontPanel.SetActive(value: true);
			RearPanel.SetActive(value: true);
			float z = Utils.Mm2Cm(_carInfo.TuningData.FrontWheelData.SpacerWidth);
			float z2 = Utils.Mm2Cm(_carInfo.TuningData.RearWheelData.SpacerWidth);
			DownLeftValueText.text = z.ToString() + new LocalizedString("SuspensionTuningWindow.Cm");
			RightLeftValueText.text = z2.ToString() + new LocalizedString("SuspensionTuningWindow.Cm");
			PublishSliderValues(sliderValues: new float4(0f, 15f, z, 0.1f), sliderHandler: FrontSliderValueHandlers[0], sliderTitle: new LocalizedString("SuspensionTuningWindow.SpacersThicknessCm"), plusTitle: new LocalizedString("SuspensionTuningWindow.Thinner"), minusTitle: new LocalizedString("SuspensionTuningWindow.Thicker"), isRound: false, numberAfterFloat: 1);
			PublishSliderValues(sliderValues: new float4(0f, 15f, z2, 0.1f), sliderHandler: RearSliderValueHandlers[0], sliderTitle: new LocalizedString("SuspensionTuningWindow.SpacersThicknessCm"), plusTitle: new LocalizedString("SuspensionTuningWindow.Thinner"), minusTitle: new LocalizedString("SuspensionTuningWindow.Thicker"), isRound: false, numberAfterFloat: 1);
			SetTitleAndDescription(new LocalizedString("SuspensionTuningWindow.Spacers.Title"), new LocalizedString("SuspensionTuningWindow.Spacers.Description"));
			FrontSliderValueHandlers[0].AddListener(OnFrontSpacerChanged);
			RearSliderValueHandlers[0].AddListener(OnRearSpacerChanged);
		}

		private void ShowAntiRollBars()
		{
			RightImage.sprite = SpritesArray[2];
			ShowNeedSlidersCount(1, FrontSliderValueHandlers);
			ShowNeedSlidersCount(1, RearSliderValueHandlers);
			bool active = HasAntiRollBar(_suspensionFrontDataLimits);
			bool active2 = HasAntiRollBar(_suspensionRearDataLimits);
			FrontPanel.SetActive(active);
			RearPanel.SetActive(active2);
			PublishSliderValues(sliderValues: new float4(Utils.N2KN(_suspensionFrontDataLimits.AntiSwayBarForceMin), Utils.N2KN(_suspensionFrontDataLimits.AntiSwayBarForceMax), Utils.N2KN(_carInfo.TuningData.FrontWheelData.SuspensionData.AntiSwayBarForce), 0.01f), sliderHandler: FrontSliderValueHandlers[0], sliderTitle: new LocalizedString("SuspensionTuningWindow.Antirollbars,level"), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			PublishSliderValues(sliderValues: new float4(Utils.N2KN(_suspensionRearDataLimits.AntiSwayBarForceMin), Utils.N2KN(_suspensionRearDataLimits.AntiSwayBarForceMax), Utils.N2KN(_carInfo.TuningData.RearWheelData.SuspensionData.AntiSwayBarForce), 0.01f), sliderHandler: RearSliderValueHandlers[0], sliderTitle: new LocalizedString("SuspensionTuningWindow.Antirollbars,level"), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			SetTitleAndDescription(new LocalizedString("SuspensionTuningWindow.AntiRollBars.Title"), new LocalizedString("SuspensionTuningWindow.AntiRollBars.Description"));
			FrontSliderValueHandlers[0].AddListener(OnFrontAntisWayForceChanged);
			RearSliderValueHandlers[0].AddListener(OnRearAntisWayForceChanged);
		}

		private void ShowSprings()
		{
			DisablePanels();
			HideAll();
			RemoveAllListeners();
			ActivateSuspensionTypeChoosePanel(isActive: true);
			_isPneumaticInstalled = false;
			PneumaticCheckbox.SetActive(value: false);
			PneumaticBuyPanel.SetActive(value: false);
			RightImage.sprite = SpritesArray[3];
			ShowNeedSlidersCount(2, FrontSliderValueHandlers);
			ShowNeedSlidersCount(2, RearSliderValueHandlers);
			FrontPanel.SetActive(value: true);
			RearPanel.SetActive(value: true);
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.SpringRateMin, _suspensionFrontDataLimits.SpringRateMax, _carInfo.TuningData.FrontWheelData.SuspensionData.SpringRate, 0.01f), sliderHandler: FrontSliderValueHandlers[1], sliderTitle: new LocalizedString("SuspensionTuningWindow.Springs,kgf/mm"), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.SpringLengthMin, _suspensionFrontDataLimits.SpringLengthMax, _carInfo.TuningData.FrontWheelData.SuspensionData.SpringLength, 0.01f), sliderHandler: FrontSliderValueHandlers[0], sliderTitle: new LocalizedString("SuspensionTuningWindow.SpringLengthInch"), plusTitle: new LocalizedString("SuspensionTuningWindow.High"), minusTitle: new LocalizedString("SuspensionTuningWindow.Low"));
			PublishSliderValues(sliderValues: new float4(_suspensionRearDataLimits.SpringRateMin, _suspensionRearDataLimits.SpringRateMax, _carInfo.TuningData.RearWheelData.SuspensionData.SpringRate, 0.01f), sliderHandler: RearSliderValueHandlers[1], sliderTitle: new LocalizedString("SuspensionTuningWindow.Springs,kgf/mm"), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			PublishSliderValues(sliderValues: new float4(_suspensionRearDataLimits.SpringLengthMin, _suspensionRearDataLimits.SpringLengthMax, _carInfo.TuningData.RearWheelData.SuspensionData.SpringLength, 0.01f), sliderHandler: RearSliderValueHandlers[0], sliderTitle: new LocalizedString("SuspensionTuningWindow.SpringLengthInch"), plusTitle: new LocalizedString("SuspensionTuningWindow.High"), minusTitle: new LocalizedString("SuspensionTuningWindow.Low"));
			SetTitleAndDescription(new LocalizedString("SuspensionTuningWindow.Springs.Title"), new LocalizedString("SuspensionTuningWindow.Springs.Description"));
			FrontSliderValueHandlers[0].AddListener(OnFrontSpringLengthChanged);
			FrontSliderValueHandlers[1].AddListener(OnFrontSpringRateChanged);
			RearSliderValueHandlers[0].AddListener(OnRearSpringLengthChanged);
			RearSliderValueHandlers[1].AddListener(OnRearSpringRateChanged);
			LeftPanelCanvasGroup.alpha = 1f;
			LeftPanelCanvasGroup.interactable = true;
		}

		private void ShowPneumatic()
		{
			DisablePanels();
			HideAll();
			RemoveAllListeners();
			ActivateSuspensionTypeChoosePanel(isActive: true);
			bool flag = _carInfo.AdditionalData.AirSuspension?.Bought ?? false;
			PneumaticBuyPanel.SetActive(!flag);
			PneumaticCheckbox.SetActive(flag);
			if (flag)
			{
				_isPneumaticInstalled = true;
			}
			RightImage.sprite = SpritesArray[6];
			ShowNeedSlidersCount(3, FrontSliderValueHandlers);
			ShowNeedSlidersCount(3, RearSliderValueHandlers);
			ShowNeedSlidersCount(1, BothSliderValueHandlers);
			FrontPanel.SetActive(value: true);
			RearPanel.SetActive(value: true);
			OtherPanel.SetActive(value: true);
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.SpringRateMin, _suspensionFrontDataLimits.SpringRateMax, _carInfo.TuningData.FrontWheelData.SuspensionData.SpringRate, 0.01f), sliderHandler: FrontSliderValueHandlers[0], sliderTitle: new LocalizedString("Suspensionuiwindow.Stiffness"), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			PublishSliderValues(sliderValues: new float4(_suspensionFrontDataLimits.SpringLengthMin, _suspensionFrontDataLimits.SpringLengthMax, _carInfo.TuningData.FrontWheelData.SuspensionData.SpringLength, 0.01f), sliderHandler: FrontSliderValueHandlers[1], sliderTitle: new LocalizedString("SuspensionTuningWindow.SpringLengthInch"));
			FrontSliderValueHandlers[1].AlphaHidableObject.AddDependObject(FrontSliderValueHandlers[2].Parent);
			PublishSliderValues(sliderValues: new float4(_suspensionRearDataLimits.SpringRateMin, _suspensionRearDataLimits.SpringRateMax, _carInfo.TuningData.RearWheelData.SuspensionData.SpringRate, 0.01f), sliderHandler: RearSliderValueHandlers[0], sliderTitle: new LocalizedString("Suspensionuiwindow.Stiffness"), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			PublishSliderValues(sliderValues: new float4(_suspensionRearDataLimits.SpringLengthMin, _suspensionRearDataLimits.SpringLengthMax, _carInfo.TuningData.RearWheelData.SuspensionData.SpringLength, 0.01f), sliderHandler: RearSliderValueHandlers[1], sliderTitle: new LocalizedString("SuspensionTuningWindow.SpringLengthInch"));
			RearSliderValueHandlers[1].AlphaHidableObject.AddDependObject(RearSliderValueHandlers[2].Parent);
			PublishSliderValues(sliderValues: new float4(0.01f, 0.99f, _carInfo.TuningData.DynamicTuningSettings.HeightAdjustSpeed, 0.01f), sliderHandler: BothSliderValueHandlers[0], sliderTitle: new LocalizedString("Suspensionuiwindow.CompressorPower"));
			BothSliderValueHandlers[0].SetCurrentValueTextActive(state: false);
			SetFrontPneumaticSlidersValue(firstInit: true);
			SetRearPneumaticSlidersValue(firstInit: true);
			SetTitleAndDescription(new LocalizedString("SuspensionTuningWindow.Springs.Title"), new LocalizedString("Suspensionuiwindow.pneumaticDescription"));
			FrontSliderValueHandlers[0].AddListener(OnFrontSpringRateChanged);
			FrontSliderValueHandlers[1].AddListener(OnFrontPneumaticLengthChanged);
			FrontSliderValueHandlers[2].AddListener(OnFrontMaxHeightChanged);
			RearSliderValueHandlers[0].AddListener(OnRearSpringRateChanged);
			RearSliderValueHandlers[1].AddListener(OnRearPneumaticLengthChanged);
			RearSliderValueHandlers[2].AddListener(OnRearMaxHeightChanged);
			BothSliderValueHandlers[0].AddListener(OnCompressorPowerChanged);
			ReloadLayouts(flag).Forget();
		}

		private float FrontReachableClearanceMm()
		{
			return Mathf.Round(AirSuspensionMath.SpringLenToClearance(Utils.Inch2Meter(_suspensionFrontDataLimits.SpringLengthMax), _powertrain.GetFrontAirSag()) * 1000f);
		}

		private float RearReachableClearanceMm()
		{
			return Mathf.Round(AirSuspensionMath.SpringLenToClearance(Utils.Inch2Meter(_suspensionRearDataLimits.SpringLengthMax), _powertrain.GetRearAirSag()) * 1000f);
		}

		private float FrontFloorClearanceMm()
		{
			return Mathf.Round(AirSuspensionMath.EffectiveLimits(Utils.Inch2Meter(_suspensionFrontDataLimits.SpringLengthMin), Utils.Inch2Meter(_suspensionFrontDataLimits.SpringLengthMax), _powertrain.GetFrontAirSag(), new float2(-1f, -1f)).x * 1000f);
		}

		private float RearFloorClearanceMm()
		{
			return Mathf.Round(AirSuspensionMath.EffectiveLimits(Utils.Inch2Meter(_suspensionRearDataLimits.SpringLengthMin), Utils.Inch2Meter(_suspensionRearDataLimits.SpringLengthMax), _powertrain.GetRearAirSag(), new float2(-1f, -1f)).x * 1000f);
		}

		private float FrontStockClearanceMm()
		{
			return Mathf.Round(AirSuspensionMath.SpringLenToClearance(Utils.Inch2Meter(_carInfo.TuningData.FrontWheelData.SuspensionData.SpringLength), _powertrain.GetFrontAirSag()) * 1000f);
		}

		private float RearStockClearanceMm()
		{
			return Mathf.Round(AirSuspensionMath.SpringLenToClearance(Utils.Inch2Meter(_carInfo.TuningData.RearWheelData.SuspensionData.SpringLength), _powertrain.GetRearAirSag()) * 1000f);
		}

		private void SetRearPneumaticSlidersValue(bool firstInit = false)
		{
			if (_refreshingPneumaticSliders)
			{
				return;
			}
			_refreshingPneumaticSliders = true;
			try
			{
				float num = RearReachableClearanceMm();
				float num2 = Mathf.Min(RearFloorClearanceMm(), num);
				float maxRearSuspensionHeight = _carInfo.TuningData.DynamicTuningSettings.MaxRearSuspensionHeight;
				float mm = ((maxRearSuspensionHeight < 0f) ? num : Mathf.Clamp(maxRearSuspensionHeight, num2, num));
				float4 sliderValues = new float4(MmToInch(num2), MmToInch(num), MmToInch(mm), 0.05f);
				if (firstInit)
				{
					PublishSliderValues(RearSliderValueHandlers[2], sliderValues, new LocalizedString("Suspensionuiwindow.MaxHeightInch"));
				}
				else
				{
					RearSliderValueHandlers[2].Init(sliderValues.x, sliderValues.y, sliderValues.w, isRoundValue: false, removeSpecialListener: true, 2, ignoreSetDefaultValue: true);
				}
				RearSliderValueHandlers[2].ShowLimitLines(state: false);
			}
			finally
			{
				_refreshingPneumaticSliders = false;
			}
		}

		private void SetFrontPneumaticSlidersValue(bool firstInit = false)
		{
			if (_refreshingPneumaticSliders)
			{
				return;
			}
			_refreshingPneumaticSliders = true;
			try
			{
				float num = FrontReachableClearanceMm();
				float num2 = Mathf.Min(FrontFloorClearanceMm(), num);
				float maxFrontSuspensionHeight = _carInfo.TuningData.DynamicTuningSettings.MaxFrontSuspensionHeight;
				float mm = ((maxFrontSuspensionHeight < 0f) ? num : Mathf.Clamp(maxFrontSuspensionHeight, num2, num));
				float4 sliderValues = new float4(MmToInch(num2), MmToInch(num), MmToInch(mm), 0.05f);
				if (firstInit)
				{
					PublishSliderValues(FrontSliderValueHandlers[2], sliderValues, new LocalizedString("Suspensionuiwindow.MaxHeightInch"));
				}
				else
				{
					FrontSliderValueHandlers[2].Init(sliderValues.x, sliderValues.y, sliderValues.w, isRoundValue: false, removeSpecialListener: true, 2, ignoreSetDefaultValue: true);
				}
				FrontSliderValueHandlers[2].ShowLimitLines(state: false);
			}
			finally
			{
				_refreshingPneumaticSliders = false;
			}
		}

		private void ShowShockAbsorber()
		{
			RightImage.sprite = SpritesArray[4];
			ShowNeedSlidersCount(2, FrontSliderValueHandlers);
			ShowNeedSlidersCount(2, RearSliderValueHandlers);
			FrontPanel.SetActive(value: true);
			RearPanel.SetActive(value: true);
			PublishSliderValues(sliderValues: new float4(Utils.N2KN(_suspensionFrontDataLimits.BumpMin), Utils.N2KN(_suspensionFrontDataLimits.BumpMax), Utils.N2KN(_carInfo.TuningData.FrontWheelData.SuspensionData.Bump), 0.05f), sliderHandler: FrontSliderValueHandlers[0], sliderTitle: new LocalizedString(new LocalizedString("Suspensionuiwindow.Bump")), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			PublishSliderValues(sliderValues: new float4(Utils.N2KN(_suspensionFrontDataLimits.ReboundMin), Utils.N2KN(_suspensionFrontDataLimits.ReboundMax), Utils.N2KN(_carInfo.TuningData.FrontWheelData.SuspensionData.Rebound), 0.05f), sliderHandler: FrontSliderValueHandlers[1], sliderTitle: new LocalizedString(new LocalizedString("Suspensionuiwindow.rebound")));
			PublishSliderValues(sliderValues: new float4(Utils.N2KN(_suspensionRearDataLimits.BumpMin), Utils.N2KN(_suspensionRearDataLimits.BumpMax), Utils.N2KN(_carInfo.TuningData.RearWheelData.SuspensionData.Bump), 0.05f), sliderHandler: RearSliderValueHandlers[0], sliderTitle: new LocalizedString(new LocalizedString("Suspensionuiwindow.Bump")), plusTitle: new LocalizedString("SuspensionTuningWindow.Stiff"), minusTitle: new LocalizedString("SuspensionTuningWindow.Soft"));
			PublishSliderValues(sliderValues: new float4(Utils.N2KN(_suspensionRearDataLimits.ReboundMin), Utils.N2KN(_suspensionRearDataLimits.ReboundMax), Utils.N2KN(_carInfo.TuningData.RearWheelData.SuspensionData.Rebound), 0.05f), sliderHandler: RearSliderValueHandlers[1], sliderTitle: new LocalizedString(new LocalizedString("Suspensionuiwindow.rebound")));
			SetTitleAndDescription(new LocalizedString("SuspensionTuningWindow.ShockAbsorber.Title"), new LocalizedString("SuspensionTuningWindow.ShockAbsorber.Description"));
			FrontSliderValueHandlers[0].AddListener(OnFrontBumpChanged);
			FrontSliderValueHandlers[1].AddListener(OnFrontReboundChanged);
			RearSliderValueHandlers[0].AddListener(OnRearBumpChanged);
			RearSliderValueHandlers[1].AddListener(OnRearReboundChanged);
		}

		private void SetSliderValues(SliderValueHandler sliderValueHandler, float minVal, float maxValue, float value, float step, bool roundValue = false, int numberAfterFloat = 2, bool dontRemoveListener = false)
		{
			sliderValueHandler.Init(minVal, maxValue, step, roundValue, dontRemoveListener, numberAfterFloat);
			sliderValueHandler.SetValue(value);
		}

		private void DisablePanels()
		{
			LeftPanelCanvasGroup.alpha = 0f;
			LeftPanelCanvasGroup.interactable = false;
			FrontPanel.SetActive(value: false);
			RearPanel.SetActive(value: false);
			OtherPanel.SetActive(value: false);
			PneumaticBuyPanel.SetActive(value: false);
			ActivateSuspensionTypeChoosePanel(isActive: false);
			TopBarTitleButtonsPanel.SetActive(value: false);
			FrontSliderValueHandlers[0].ShowSliderTitlTexts(state: false);
			TopLeftImageText.gameObject.SetActive(value: false);
			TopCenterImageText.gameObject.SetActive(value: false);
			TopRightImageText.gameObject.SetActive(value: false);
			DownLeftValueText.transform.parent.gameObject.SetActive(value: false);
			RightLeftValueText.transform.parent.gameObject.SetActive(value: false);
		}

		private void SetTitleAndDescription(string title, string description)
		{
			SetText(Title, title);
			SetText(Description, description);
		}

		private void SetText(TextMeshProUGUI textMesh, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				textMesh.SetActive(active: true);
				textMesh.text = value;
			}
		}

		private void AddButtonsListeners()
		{
			SaveButton.onClick.AddListener(Save);
			ResetButton.onClick.AddListener(Reset);
			ApplyButton.onClick.AddListener(Apply);
			DontSaveButton.onClick.AddListener(() =>
			{
				UnsaveChanges().Forget();
			});
		}

		private void HidePopup()
		{
			MainPanel.SetActive(value: true);
			PopupPanel.SetActive(value: false);
		}

		private void EnableWheelsPhysics(bool enable)
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				if (!_garageController)
				{
					component.GetPowertrain.SetKinematic(!enable);
				}
				else
				{
					_garageController.EnableCarPhysics(component, enable);
				}
			}
		}

		public void OnFrontChamberChanged(float value)
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(0f, useAssistent: false);
			}
			_vehicleEntityProvider.UpdateCambers(AxleType.Front, value);
		}

		public void OnRearChamberChanged(float value)
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(0f, useAssistent: false);
			}
			_vehicleEntityProvider.UpdateCambers(AxleType.Rear, value);
		}

		public void OnFrontToeChanged(float value)
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(0f, useAssistent: false);
			}
			_vehicleEntityProvider.UpdateToe(AxleType.Front, value);
		}

		public void OnRearToeChanged(float value)
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(0f, useAssistent: false);
			}
			_vehicleEntityProvider.UpdateToe(AxleType.Rear, value);
		}

		public void OnMaxSteeringAngleChanged(float value)
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(-1f, useAssistent: false);
			}
			_vehicleEntityProvider.UpdateMaxSteeringAngle((int)value);
		}

		public void OnAckermanChanged(float value)
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(-1f, useAssistent: false);
			}
			_vehicleEntityProvider.UpdateAckerman((int)value);
		}

		public void OnCasterChanged(float value)
		{
			value = Utils.RoundTo(value, 1);
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(-1f, useAssistent: false);
			}
			_vehicleEntityProvider.UpdateCaster(value);
		}

		public void OnFrontSpacerChanged(float value)
		{
			_carInfo.TuningData.FrontWheelData.SpacerWidth = Utils.Cm2Mm(value);
			_powertrain.SetSpacers(AxleType.Front, Utils.Cm2Mm(value));
		}

		public void OnRearSpacerChanged(float value)
		{
			_carInfo.TuningData.RearWheelData.SpacerWidth = Utils.Cm2Mm(value);
			_powertrain.SetSpacers(AxleType.Rear, Utils.Cm2Mm(value));
		}

		public void OnFrontSpringLengthChanged(float value)
		{
			_carInfo.TuningData.FrontWheelData.SuspensionData.SpringLength = value;
			_vehicleEntityProvider.UpdateSprings(AxleType.Front);
		}

		public void OnRearSpringLengthChanged(float value)
		{
			_carInfo.TuningData.RearWheelData.SuspensionData.SpringLength = value;
			_vehicleEntityProvider.UpdateSprings(AxleType.Rear);
		}

		public void OnFrontSpringRateChanged(float value)
		{
			_carInfo.TuningData.FrontWheelData.SuspensionData.SpringRate = value;
			_vehicleEntityProvider.UpdateSprings(AxleType.Front);
			if (_isPneumaticInstalled)
			{
				SetFrontPneumaticSlidersValue();
			}
		}

		public void OnRearSpringRateChanged(float value)
		{
			_carInfo.TuningData.RearWheelData.SuspensionData.SpringRate = value;
			_vehicleEntityProvider.UpdateSprings(AxleType.Rear);
			if (_isPneumaticInstalled)
			{
				SetRearPneumaticSlidersValue();
			}
		}

		public void OnFrontPneumaticLengthChanged(float value)
		{
			_carInfo.TuningData.FrontWheelData.SuspensionData.SpringLength = value;
			_vehicleEntityProvider.UpdateSprings(AxleType.Front);
			SetFrontPneumaticSlidersValue();
		}

		public void OnRearPneumaticLengthChanged(float value)
		{
			_carInfo.TuningData.RearWheelData.SuspensionData.SpringLength = value;
			_vehicleEntityProvider.UpdateSprings(AxleType.Rear);
			SetRearPneumaticSlidersValue();
		}

		private void OnFrontMaxHeightChanged(float valueInch)
		{
			if (!_refreshingPneumaticSliders)
			{
				_carInfo.TuningData.DynamicTuningSettings.MaxFrontSuspensionHeight = MaxHeightToSaveMm(InchToMm(valueInch), FrontReachableClearanceMm());
				_vehicleEntityProvider.UpdateSprings(AxleType.Front);
				SetFrontPneumaticSlidersValue();
			}
		}

		private void OnRearMaxHeightChanged(float valueInch)
		{
			if (!_refreshingPneumaticSliders)
			{
				_carInfo.TuningData.DynamicTuningSettings.MaxRearSuspensionHeight = MaxHeightToSaveMm(InchToMm(valueInch), RearReachableClearanceMm());
				_vehicleEntityProvider.UpdateSprings(AxleType.Rear);
				SetRearPneumaticSlidersValue();
			}
		}

		private static float MmToInch(float mm)
		{
			return mm / 25.4f;
		}

		private static float InchToMm(float inch)
		{
			return inch * 25.4f;
		}

		private static float MaxHeightToSaveMm(float valueMm, float ceilingMm)
		{
			if (!(valueMm >= ceilingMm - 0.5f))
			{
				return Mathf.Round(valueMm);
			}
			return -1f;
		}

		private void OnCompressorPowerChanged(float value)
		{
			_vehicleEntityProvider.UpdateCompressorPower(value);
		}

		public void OnFrontAntisWayForceChanged(float value)
		{
			_vehicleEntityProvider.UpdateAntirollBars(AxleType.Front, value);
		}

		public void OnRearAntisWayForceChanged(float value)
		{
			_vehicleEntityProvider.UpdateAntirollBars(AxleType.Rear, value);
		}

		public void OnFrontBumpChanged(float value)
		{
			_vehicleEntityProvider.SetShockAbsorberBump(AxleType.Front, value);
		}

		public void OnRearBumpChanged(float value)
		{
			_vehicleEntityProvider.SetShockAbsorberBump(AxleType.Rear, value);
		}

		public void OnFrontReboundChanged(float value)
		{
			_vehicleEntityProvider.SetShockAbsorberRebound(AxleType.Front, value);
		}

		public void OnRearReboundChanged(float value)
		{
			_vehicleEntityProvider.SetShockAbsorberRebound(AxleType.Rear, value);
		}

		protected override void OnDestroy()
		{
			FlagsStorage.IsCarInService = false;
			_axles.SuspensionsEditing = false;
			EnableWheelsPhysics(enable: false);
		}

		private void Save()
		{
			_carInfo.AdditionalData.AirSuspension?.Set(_isPneumaticInstalled);
			if (!_isPneumaticInstalled)
			{
				_carInfo.TuningData.DynamicTuningSettings.CurrentFrontSuspensionHeight = -1f;
				_carInfo.TuningData.DynamicTuningSettings.CurrentRearSuspensionHeight = -1f;
			}
			_powertrain.ApplySuspensionData();
			_carInfo.Save();
			_databaseCarsService.SaveCurrentCar(saveNow: true);
			if (_carInfo.TuningData.FrontWheelData.SpacerWidth >= 100f || _carInfo.TuningData.RearWheelData.SpacerWidth >= 100f)
			{
				_steamAchievementService?.Unlock("ACH_WIDE_STANCE");
			}
			float num = Utils.Cm2Mm(15f);
			if (_carInfo.TuningData.FrontWheelData.SpacerWidth >= num || _carInfo.TuningData.RearWheelData.SpacerWidth >= num)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQHA", "suspension_tuning");
			}
			_managerUI.ShowNotificationWindow(new LocalizedString("SuspensionTuningWindow.Done!"), 1f).Forget();
			_hasUnsavedChanges = false;
		}

		private void Apply()
		{
			Save();
			_managerUI.ShowCarTuningWindow();
			UnsubscribeSliderListeners();
			Close();
		}

		private void UnsubscribeSliderListeners()
		{
			SubscribeSliderToSaveButton(FrontSliderValueHandlers, unsubscribe: true);
			SubscribeSliderToSaveButton(RearSliderValueHandlers, unsubscribe: true);
			SubscribeSliderToSaveButton(BothSliderValueHandlers, unsubscribe: true);
		}

		public void Reset()
		{
			SaveButton.interactable = false;
			_hasUnsavedChanges = false;
			ResetTab(_currentSuspensionTab);
			CarEntity.RecalculateSuspension(_vehicleEntityProvider, _carTuningFactory, _carInfo);
			OnTabSelected(_currentSuspensionTab);
			_managerUI.ShowNotificationWindow(new LocalizedString("SuspensionTuningWindow.ResettoDefault!"), 1f).Forget();
		}

		private void CloseWindow()
		{
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(0f, useAssistent: false);
			}
			UnsubscribeSliderListeners();
			_managerUI.ShowCarTuningWindow();
			base.NavigationModeSet(isEnabled: false);
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			if (_hasUnsavedChanges)
			{
				if ((bool)_powertrain)
				{
					_powertrain.SetSteer(0f, useAssistent: false);
				}
				_managerUI.OnBackButtonActivated?.Invoke(obj: false);
				MainPanel.SetActive(value: false);
				PopupPanel.SetActive(value: true);
			}
			else
			{
				CloseWindow();
			}
		}

		public void ResetTab(SuspensionTabs suspensionTabs)
		{
			FrontWheelData frontWheelData = _carInfo.TuningData.FrontWheelData;
			FrontWheelData frontWheelData2 = _carDefaultTuningDataConfig.FrontWheelData;
			RearWheelData rearWheelData = _carInfo.TuningData.RearWheelData;
			RearWheelData rearWheelData2 = _carDefaultTuningDataConfig.RearWheelData;
			switch (suspensionTabs)
			{
			case SuspensionTabs.Alignment:
				frontWheelData.SuspensionData.Camber = frontWheelData2.SuspensionData.Camber;
				frontWheelData.SuspensionData.ToeIn = frontWheelData2.SuspensionData.ToeIn;
				frontWheelData.SuspensionData.Ackerman = frontWheelData2.SuspensionData.Ackerman;
				frontWheelData.SuspensionData.Caster = frontWheelData2.SuspensionData.Caster;
				frontWheelData.SuspensionData.MaxSteerAngle = frontWheelData2.SuspensionData.MaxSteerAngle;
				rearWheelData.SuspensionData.Camber = rearWheelData2.SuspensionData.Camber;
				rearWheelData.SuspensionData.ToeIn = rearWheelData2.SuspensionData.ToeIn;
				break;
			case SuspensionTabs.Spacers:
				frontWheelData.SpacerWidth = frontWheelData2.SpacerWidth;
				rearWheelData.SpacerWidth = rearWheelData2.SpacerWidth;
				break;
			case SuspensionTabs.AntiRollBars:
				frontWheelData.SuspensionData.AntiSwayBarForce = frontWheelData2.SuspensionData.AntiSwayBarForce;
				rearWheelData.SuspensionData.AntiSwayBarForce = rearWheelData2.SuspensionData.AntiSwayBarForce;
				break;
			case SuspensionTabs.Springs:
				frontWheelData.SuspensionData.SuspensionType = frontWheelData2.SuspensionData.SuspensionType;
				frontWheelData.SuspensionData.SuspensionState = frontWheelData2.SuspensionData.SuspensionState;
				frontWheelData.SuspensionData.SpringLength = frontWheelData2.SuspensionData.SpringLength;
				frontWheelData.SuspensionData.SpringRate = frontWheelData2.SuspensionData.SpringRate;
				rearWheelData.SuspensionData.SuspensionState = rearWheelData2.SuspensionData.SuspensionState;
				rearWheelData.SuspensionData.SuspensionType = rearWheelData2.SuspensionData.SuspensionType;
				rearWheelData.SuspensionData.SpringLength = rearWheelData2.SuspensionData.SpringLength;
				rearWheelData.SuspensionData.SpringRate = rearWheelData2.SuspensionData.SpringRate;
				_carInfo.TuningData.DynamicTuningSettings.CurrentFrontSuspensionHeight = -1f;
				_carInfo.TuningData.DynamicTuningSettings.CurrentRearSuspensionHeight = -1f;
				_carInfo.TuningData.DynamicTuningSettings.MinFrontSuspensionHeight = -1f;
				_carInfo.TuningData.DynamicTuningSettings.MaxFrontSuspensionHeight = -1f;
				_carInfo.TuningData.DynamicTuningSettings.MinRearSuspensionHeight = -1f;
				_carInfo.TuningData.DynamicTuningSettings.MaxRearSuspensionHeight = -1f;
				_carInfo.TuningData.DynamicTuningSettings.HeightAdjustSpeed = 0.5f;
				break;
			case SuspensionTabs.ShockAbsorber:
				frontWheelData.SuspensionData.Bump = frontWheelData2.SuspensionData.Bump;
				frontWheelData.SuspensionData.Rebound = frontWheelData2.SuspensionData.Rebound;
				rearWheelData.SuspensionData.Bump = rearWheelData2.SuspensionData.Bump;
				rearWheelData.SuspensionData.Rebound = rearWheelData2.SuspensionData.Rebound;
				break;
			}
		}
	}
}
