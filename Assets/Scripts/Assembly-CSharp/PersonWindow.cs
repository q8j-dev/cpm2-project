using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.FreeDriverService;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using Racing.CameraManagement;
using Racing.CameraManagement.SpecificTypes;
using Sirenix.Utilities;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Basket;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.FirstGameSetup;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Offers.Reward;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement.Settings;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.WindowSystem;

public class PersonWindow : WindowScript, IBackableWindow
{
	private const string PersonCustomizationCompletedPrefsKey = "PersonCustomizationCompleted";

	private const string ShopCharacterNotAvailableForPurchase = "ShopCharacter.NotAvailableForPurchase";

	private const string ShopCharacterEventsAvailable = "ShopCharacter.Events.AvailableForActiveEvent";

	private const string ShopCharacterKingAvailableForKingRank = "ShopCharacter.King.AvailableForKingRank";

	private const string ShopCharacterPackOfferNotAvailable = "SuitPack.PackNotAvailable";

	public CanvasGroup canvasGroup;

	[SerializeField]
	private GameObject[] ToHideObjects;

	[SerializeField]
	private GameObject TopElements;

	[SerializeField]
	private Button CloseButton;

	[SerializeField]
	private GenderSwitchView GenderSwitchView;

	[SerializeField]
	private BasketView BasketView;

	[SerializeField]
	private UnpurchasedItemsWindowView UnpurchasedItemsWindowView;

	[SerializeField]
	private ToggleGroup ToggleGroupCharacterCategoryItem;

	[SerializeField]
	private Toggle AppearanceToggle;

	[SerializeField]
	private Toggle WardrobeToggle;

	[SerializeField]
	private Toggle EventsToggle;

	[SerializeField]
	private ClothItemCategory[] ClothItemCategories;

	[SerializeField]
	private PackCategory[] PackCategories;

	[SerializeField]
	private GameObject SkinNotice;

	[SerializeField]
	private GameObject ClothNotice;

	[SerializeField]
	private GameObject EventsNotice;

	public ScrollRect ScrollViewItemsCategories;

	public RectTransform LeftColumn;

	public Transform SkinPanel;

	public Transform ClothPanel;

	public Transform EventsPanel;

	public Transform AnimationPanel;

	[SerializeField]
	private Transform PacksPanel;

	public OneKitMenuItem oneKitPrefab;

	private int _selectedRightElementIndex;

	private int _selectedCenterElementIndex;

	private int _selectedLeftElementIndex;

	public Color selectedColor;

	public Color unSelectedColor;

	public AnimationShop AnimationShop;

	public CharacterMenuCategoryItem[] CharacterMenuCategoryItems;

	private PersonWindowCategoryType _openedCategory;

	public List<OneKitMenuItem> allItems;

	private List<OneKitMenuItem> _cachedLeftItems = new List<OneKitMenuItem>();

	private OneKitMenuItem _lastSelectedOneKitMenuItem;

	[HideInInspector]
	public OneKitMenuItem LastOwnedOneKitMenuItem;

	[HideInInspector]
	public GameObject Person;

	[HideInInspector]
	public MainPerson mainPerson;

	public TextMeshProUGUI buyButtonLabelConfirm;

	public ButtonEnableDisableColor BuyButton;

	public GameObject coinIcon;

	public GameObject coinIconConfirm;

	public GameObject MainWindow;

	public GameObject RightPanel;

	public GameObject confirmWindow;

	public GameObject GenderPanel;

	private float field;

	private Camera mainCamera;

	public Dictionary<KitType, int> selectedNowKits = new Dictionary<KitType, int>();

	private bool _skipShowMainMenuOnClose;

	private int _prevEquipmentTypeId;

	private bool _suppressItemSelection;

	private Light roomLight;

	private CharactersService _charactersService;

	private ICharacterEquipmentDataService _characterEquipmentDataService;

	private IPrefabFactory _prefabFactory;

	[HideInInspector]
	public PersonKitConfig MaleKitConfig;

	[HideInInspector]
	public PersonKitConfig FemaleKitConfig;

	private CameraManager _cameraManager;

	private OptionalDependency<CameraManager> _cameraManagerDep;

	private OptionalDependency<LightForChangeRoom> _lightDep;

	private OptionalDependency<Camera> _cameraDep;

	private OptionalDependency<IRegistrationController> _registrationControllerDep;

	private GaragePositionsConfig _garagePositionsConfig;

	private IGarageSelectorService _garageSelectorService;

	private IFreeDriverService _freeDriverService;

	private CurrencyManagerController _currencyManagerController;

	private readonly List<int> _pantsWithNoShoes = new List<int> { 18, 20, 21 };

	private int _lastCamIndex;

	private EquipmentsData equipmentsData;

	private FreeDriveProvider _freeDriveProvider;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private IRegistrationController _registrationController;

	private PersonWindowAdditionalData _personWindowAdditionalData;

	private PersonShopCameraHandler _personShopCameraHandler;

	private CharacterCreationAnalytic _characterCreationAnalytic;

	private INotificationService _notificationService;

	private PersonClothesEventChecker _personClothesEventChecker;

	private IGuestAccountCreator _guestAccountCreator;

	private BundleOfferCatalog _bundleOfferCatalog;

	private PresetGrantSession _presetGrantSession;

	[field: SerializeField]
	public Button SaveAndContinueButton { get; private set; }

	[field: SerializeField]
	public Button BackButton { get; private set; }

	[field: SerializeField]
	public TMP_InputField NiknameInputField { get; private set; }

	[field: SerializeField]
	public Toggle AnimationsToggle { get; private set; }

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

	protected override bool ChangeRootGameInputGroup => true;

	[Inject]
	public void Construct(PersonKitConfigs personKitConfigs, CharactersService charactersService, ICharacterEquipmentDataService characterEquipmentDataService, IPrefabFactory prefabFactory, FreeDriveProvider freeDriveProvider, ManagerUI managerUI, GaragePositionsConfig garagePositionsConfig, CurrencyManagerController currencyManagerController, IGarageSelectorService garageSelectorService, IFreeDriverService freeDriverService, VirtualPurchaseAnalytic virtualPurchaseAnalytic, CharacterCreationAnalytic characterCreationAnalytic, INotificationService notificationService, PersonClothesEventChecker personClothesEventChecker, IGuestAccountCreator guestAccountCreator, BundleOfferCatalog bundleOfferCatalog, PresetGrantSession presetGrantSession, OptionalDependency<LightForChangeRoom> light, OptionalDependency<Camera> camera, OptionalDependency<CameraManager> cameraManager, OptionalDependency<IRegistrationController> registrationController)
	{
		_freeDriveProvider = freeDriveProvider;
		_currencyManagerController = currencyManagerController;
		_cameraManagerDep = cameraManager;
		_characterEquipmentDataService = characterEquipmentDataService;
		_charactersService = charactersService;
		_lightDep = light;
		_cameraDep = camera;
		MaleKitConfig = personKitConfigs.Male;
		FemaleKitConfig = personKitConfigs.Female;
		_prefabFactory = prefabFactory;
		_managerUI = managerUI;
		_garagePositionsConfig = garagePositionsConfig;
		_garageSelectorService = garageSelectorService;
		_freeDriverService = freeDriverService;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		_registrationControllerDep = registrationController;
		_characterCreationAnalytic = characterCreationAnalytic;
		_notificationService = notificationService;
		_personClothesEventChecker = personClothesEventChecker;
		_guestAccountCreator = guestAccountCreator;
		_bundleOfferCatalog = bundleOfferCatalog;
		_presetGrantSession = presetGrantSession;
	}

	protected override void Awake()
	{
		base.Awake();
		_cameraManager = _cameraManagerDep?.Value;
		roomLight = _lightDep?.Value?.Light;
		mainCamera = _cameraDep?.Value;
		_registrationController = _registrationControllerDep?.Value;
	}

	protected override void Start()
	{
		base.Start();
		TryInitWindow();
		if ((bool)_personShopCameraHandler || (_cameraManager.GetCurrentCamera() is MonoBehaviour monoBehaviour && monoBehaviour.TryGetComponent<PersonShopCameraHandler>(out _personShopCameraHandler)))
		{
			int currentGarageIndex = _garageSelectorService.CurrentGarageIndex;
			float personShopCameraXAxis = _garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].PersonShopCameraXAxis;
			_personShopCameraHandler.SetXAxisValue(personShopCameraXAxis);
			_personShopCameraHandler.SetMiddleRigScreenX(_personShopCameraHandler.MiddleRigInitScreenXCam);
		}
		_lastCamIndex = -1;
		_managerUI.ScreenFade.FadeOut(0.75f);
		foreach (KitType value in Enum.GetValues(typeof(KitType)))
		{
			if (value != KitType.None)
			{
				selectedNowKits.Add(value, -2);
			}
		}
		CharacterMenuCategoryItems?.ForEach((CharacterMenuCategoryItem item) =>
		{
			item.Init();
			item.OnSelected += OnSelectCategory;
			item.Toggle.group = ToggleGroupCharacterCategoryItem;
		});
		ClothItemCategories?.ForEach((ClothItemCategory item) =>
		{
			item.Init();
			item.OnSelectedClothType += FillLeftPanel;
		});
		PackCategories?.ForEach((PackCategory category) =>
		{
			category.Init();
			category.OnSelectedPack += FillLeftPanelForPack;
		});
		GenderSwitchView.OnGenderChanged += OnGenderChanged;
		BasketView.Init();
		BasketView.BasketItemCreated += (BasketItem item) =>
		{
			item.OnRemoveItem += RemoveBasketItem;
		};
		UnpurchasedItemsWindowView.Init();
		UnpurchasedItemsWindowView.BuyButton.Button.onClick.AddListener(BuyOrSave);
		FillTables();
		SaveAndContinueButton.onClick.AddListener(SaveAndContinue);
		if (ScenesHelper.IsMenuSceneActive())
		{
			GenderPanel.gameObject.SetActive(value: true);
		}
		else
		{
			GameObject[] toHideObjects = ToHideObjects;
			for (int num = 0; num < toHideObjects.Length; num++)
			{
				toHideObjects[num].SetActive(value: false);
			}
			TopElements.SetActive(value: true);
			CloseButton.onClick.AddListener(CloseWindowWithNotify);
			OnSelectCategory(PersonWindowCategoryType.Animation);
		}
		OpenCategoryItems();
		updatePrice();
		NiknameInputField.characterLimit = AccountSection.MaxPlayerNameLength;
		_prevEquipmentTypeId = 5;
	}

	protected override void OnEnable()
	{
		_notificationService.NoticeByTypeChanged += OnNoticeByTypeChanged;
		SetupNotifyComponents();
		_personClothesEventChecker.UpdateEventList();
		base.OnEnable();
	}

	private void OnDisable()
	{
		_notificationService.NoticeByTypeChanged -= OnNoticeByTypeChanged;
	}

	private void OnNoticeByTypeChanged(int type, int gender)
	{
		if (gender == _characterEquipmentDataService.Gender)
		{
			SetupNotifyComponents();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SaveAndContinueButton.onClick.RemoveAllListeners();
	}

	public override void SetAdditionalData(string data)
	{
		base.SetAdditionalData(data);
		if (!string.IsNullOrEmpty(data))
		{
			_personWindowAdditionalData = JsonConvert.DeserializeObject<PersonWindowAdditionalData>(data);
			if (_personWindowAdditionalData != null && (_personWindowAdditionalData.Gender == 0 || _personWindowAdditionalData.Gender == 1))
			{
				_characterEquipmentDataService.Gender = _personWindowAdditionalData.Gender;
				_characterEquipmentDataService.Save();
			}
		}
	}

	private void TryInitWindow()
	{
		if (_managerUI.PersonWindowInitializer != null)
		{
			_managerUI.PersonWindowInitializer.Init(this);
			_managerUI.PersonWindowInitializer = null;
		}
	}

	private void OpenCategoryItems()
	{
		PersonWindowAdditionalData personWindowAdditionalData = _personWindowAdditionalData;
		if (personWindowAdditionalData != null && personWindowAdditionalData.OpenCategory.HasValue)
		{
			CharacterMenuCategoryItem[] characterMenuCategoryItems = CharacterMenuCategoryItems;
			foreach (CharacterMenuCategoryItem characterMenuCategoryItem in characterMenuCategoryItems)
			{
				if (characterMenuCategoryItem.PersonWindowCategory == _personWindowAdditionalData.OpenCategory.Value)
				{
					characterMenuCategoryItem.Toggle.isOn = true;
					return;
				}
			}
		}
		if (_personWindowAdditionalData != null)
		{
			PersonWindowAdditionalData personWindowAdditionalData2 = _personWindowAdditionalData;
			if (personWindowAdditionalData2 == null || personWindowAdditionalData2.PersonClothsEventType != PersonClothsEventType.None)
			{
				CharacterMenuCategoryItem[] characterMenuCategoryItems = CharacterMenuCategoryItems;
				foreach (CharacterMenuCategoryItem characterMenuCategoryItem2 in characterMenuCategoryItems)
				{
					if (characterMenuCategoryItem2.PersonWindowCategory == PersonWindowCategoryType.Events)
					{
						characterMenuCategoryItem2.Toggle.isOn = true;
						break;
					}
				}
				return;
			}
		}
		CharacterMenuCategoryItems[0].Toggle.isOn = true;
	}

	public void CloseWindowsOnFirstStart()
	{
		if ((bool)AnimationsToggle)
		{
			AnimationsToggle.gameObject.SetActive(value: false);
		}
		CharacterMenuCategoryItem[] characterMenuCategoryItems = CharacterMenuCategoryItems;
		foreach (CharacterMenuCategoryItem characterMenuCategoryItem in characterMenuCategoryItems)
		{
			if ((bool)characterMenuCategoryItem)
			{
				PersonWindowCategoryType personWindowCategory = characterMenuCategoryItem.PersonWindowCategory;
				if (personWindowCategory == PersonWindowCategoryType.Events || personWindowCategory == PersonWindowCategoryType.Packs || personWindowCategory == PersonWindowCategoryType.Poses)
				{
					characterMenuCategoryItem.gameObject.SetActive(value: false);
				}
			}
		}
	}

	private void CloseWindowWithNotify()
	{
		if (!base.IsCloseCanceled)
		{
			CountCoins();
			if (!TryHandlePackBasket())
			{
				_managerUI.OnPersonCustomizationWindowClosed?.Invoke();
				_managerUI.CloseSeveralWindows();
				base.Close();
			}
		}
	}

	private void FillTables()
	{
		BuyButton.Button.onClick.AddListener(BuyOrSave);
		LeftColumn.gameObject.SetActive(value: true);
	}

	public void OnGenderChanged(int gender)
	{
		PersonWindowCategoryType openedCategory = _openedCategory;
		BasketView.RemoveAllItemsFromBasket();
		_characterEquipmentDataService.Gender = gender;
		_characterEquipmentDataService.Save();
		OnPersonWindowOpen();
		FillLeftPanel(KitType.Hair, PersonClothsEventType.None);
		ClothItemCategories[0].Toggle.isOn = true;
		int num = FindCategoryIndex(CharacterMenuCategoryItems, openedCategory);
		if (num < 0)
		{
			CharacterMenuCategoryItems[0].Toggle.isOn = true;
			return;
		}
		CharacterMenuCategoryItems[num].Toggle.SetIsOnWithoutNotify(value: true);
		OnSelectCategory(openedCategory);
	}

	public void OnPersonWindowOpen()
	{
		UnityEngine.Object.Destroy(V.DriverPerson);
		CreatePerson();
		if (ScenesHelper.IsMenuSceneActive())
		{
			FlagsStorage.FixedCameraAndLookAt = false;
			_freeDriveProvider.CarCameras.addEuler = new Vector3(5f, 20f, 0f);
			mainCamera.fieldOfView = 33f;
			_freeDriveProvider.CarCameras.speedFollow = 10f;
			_freeDriveProvider.CarCameras.minDistacne = 1f;
			if (V.CoreGameManager.menuControl.mainSceneAll.Length != 0)
			{
				PlayerDatas.DisableEnableAll(isEnable: false, V.CoreGameManager.menuControl.mainSceneAll);
			}
			V.CoreGameManager.carCameras.xAngle = 10f;
			V.CoreGameManager.carCameras.yAngle = 5f;
			_cameraManager.ActivateCamera(RacingCameraType.PersonShop, mainPerson.middle);
			_cameraManager.ForceSetCameraZoom(50f);
			_freeDriveProvider.CarCameras.distance = 4f;
			if ((bool)V.Car)
			{
				V.Car.GetComponent<Rigidbody>().isKinematic = true;
			}
			if ((bool)roomLight)
			{
				roomLight.gameObject.SetActive(value: true);
			}
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
		}
		confirmWindow.gameObject.SetActive(value: false);
		MainWindow.gameObject.SetActive(value: true);
		DOTween.CompleteAll();
		SetupNotifyComponents();
	}

	private void SetupNotifyComponents()
	{
		int gender = _characterEquipmentDataService.Gender;
		SkinNotice.gameObject.SetActive(_notificationService.HasNotice(PersonWindowCategoryType.Skin, gender));
		ClothNotice.gameObject.SetActive(HasNonPackClothNotice(gender));
		EventsNotice.gameObject.SetActive(_notificationService.HasNotice(PersonWindowCategoryType.Events, gender));
	}

	private bool HasNonPackClothNotice(int gender)
	{
		return _notificationService.AnyNotice(gender, (EquipmentNotice notice) => notice.Type >= 3 && notice.Type <= 10 && !_bundleOfferCatalog.IsPackItem(gender, notice.Type, notice.Index));
	}

	private async UniTaskVoid OnPersonWindowOpen(RectTransform rectTransform)
	{
		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, this.GetCancellationTokenOnDestroy());
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}

	public void OpenConfirmWindow()
	{
		confirmWindow.gameObject.SetActive(value: true);
		MainWindow.gameObject.SetActive(value: false);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		if ((bool)mainPerson && !mainPerson.middle.parent && (bool)mainPerson.PersonAnimationsController)
		{
			mainPerson.middle.parent = mainPerson.PersonAnimationsController.transform;
		}
		CountCoins();
		if (TryHandlePackBasket())
		{
			return;
		}
		if (!BasketView.BasketEmpty)
		{
			OpenUnpurchasedWindow(Close, () =>
			{
				_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			});
		}
		else
		{
			BasketView.RemoveAllItemsFromBasket();
			Close();
		}
	}

	public async void BuyAndClose()
	{
		CountCoins();
		if (!TryHandlePackBasket() && await TryBuyAll(equipmentsData))
		{
			onPersonWindowClosed();
			_managerUI.ShowMainMenuWindow();
			OnWindowClose();
		}
	}

	private bool TryHandlePackBasket()
	{
		foreach (KeyValuePair<Enum, BasketItem> item in BasketView.BasketItemsDic)
		{
			if (item.Value.gameObject.activeSelf && item.Value.Info is BasketItemPersonKitInfo { PackId: >=1 } basketItemPersonKitInfo)
			{
				if (!_bundleOfferCatalog.TryGetActiveOfferForPack(basketItemPersonKitInfo.PackId, out var offer))
				{
					ShowPackOfferUnavailableMessage();
					return true;
				}
				CloseToBundleOffer(offer);
				return true;
			}
		}
		return false;
	}

	public void CloseToBundleOffer(OfferModel offer)
	{
		BasketView.RemoveAllItemsFromBasket();
		_managerUI.OnPersonCustomizationWindowClosed?.Invoke();
		_managerUI.CloseSeveralWindows();
		_skipShowMainMenuOnClose = true;
		Close();
		_managerUI.ShowBundleOfferWindow(offer);
	}

	public void BuyOrSave()
	{
		if ((bool)_lastSelectedOneKitMenuItem)
		{
			_lastSelectedOneKitMenuItem.ownedMark.SetActive(value: true);
		}
		if ((bool)LastOwnedOneKitMenuItem)
		{
			LastOwnedOneKitMenuItem.ownedMark.SetActive(value: false);
		}
		if (BasketView.HaveLockedItems)
		{
			ShowNotAvailableForPurchase();
			return;
		}
		ObscuredInt obscuredInt = CountCoins();
		if (TryHandlePackBasket())
		{
			return;
		}
		if ((int)obscuredInt > 0)
		{
			canvasGroup.alpha = 0f;
			LogAnalyticEvent(PurchaseStatus.PURCHASE_CLICKED);
			_managerUI.ShowConfirmWindow(TryBuy, new LocalizedString("Playerprofilewindow.Message"), string.Format(new LocalizedString("VisualTuningWindow.Buythisitemfor"), obscuredInt), () =>
			{
				canvasGroup.alpha = 1f;
				LogAnalyticEvent(PurchaseStatus.PURCHASE_CANCELED_BY_USER);
			});
		}
		else
		{
			TryBuy();
		}
	}

	private async void TryBuy()
	{
		canvasGroup.alpha = 1f;
		LogAnalyticEvent(PurchaseStatus.PURCHASE_CONFIRMED);
		if (await TryBuyAll(equipmentsData) && UnpurchasedItemsWindowView.gameObject.activeSelf)
		{
			UnpurchasedItemsWindowView.BackButton.onClick.Invoke();
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		}
	}

	private void OnWindowClose()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: false);
		PersonWindowAdditionalData personWindowAdditionalData = _personWindowAdditionalData;
		if (personWindowAdditionalData != null && personWindowAdditionalData.ParentWindow == PersonWindowAdditionalData.ParentWindowType.MainMenu)
		{
			onPersonWindowClosed(!_skipShowMainMenuOnClose);
		}
		_skipShowMainMenuOnClose = false;
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			OnWindowClose();
			_cameraManager.garageCam.ResetCamToDefault();
			base.Close();
		}
	}

	public void onPersonWindowClosed(bool showMainMenuAfter = true)
	{
		_freeDriveProvider.CarCameras.addEuler = new Vector3(0f, 0f, 0f);
		mainCamera.fieldOfView = 60f;
		_freeDriveProvider.CarCameras.speedFollow = 2f;
		_freeDriveProvider.CarCameras.minDistacne = 3f;
		field = 60f;
		if ((bool)Person)
		{
			UnityEngine.Object.Destroy(Person);
		}
		if ((bool)V.DriverPerson)
		{
			UnityEngine.Object.Destroy(V.DriverPerson);
		}
		if ((bool)V.Car)
		{
			V.DriverPerson = null;
			V.CoreGameManager.SetupCarDriver(V.Car);
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, V.Car.transform);
		}
		else
		{
			V.DriverPerson = null;
			_freeDriverService.CreateDriver();
		}
		V.CoreGameManager.menuControl.ClosePersons(showMainMenuAfter);
		if (V.CoreGameManager.menuControl.mainSceneAll.Length != 0)
		{
			PlayerDatas.DisableEnableAll(isEnable: true, V.CoreGameManager.menuControl.mainSceneAll);
		}
		if ((bool)roomLight)
		{
			roomLight.gameObject.SetActive(value: false);
		}
	}

	public void CreatePerson()
	{
		if ((bool)Person)
		{
			UnityEngine.Object.Destroy(Person);
		}
		if (ScenesHelper.IsMenuSceneActive())
		{
			int gender = _characterEquipmentDataService.Gender;
			Person = _prefabFactory.Instantiate(Resources.Load("MenuPerson/person" + gender, typeof(GameObject)) as GameObject);
			int currentGarageIndex = _garageSelectorService.CurrentGarageIndex;
			Person.transform.position = _garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].PersonCustomizationPosition.Position;
			Person.transform.rotation = _garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].PersonCustomizationPosition.Rotation;
			V.DriverPerson = Person;
		}
		else
		{
			Person = V.DriverPerson;
		}
		mainPerson = Person.GetComponent<MainPerson>();
		NewMoveController component = Person.GetComponent<NewMoveController>();
		if ((bool)mainPerson && (bool)mainPerson.PersonAnimationsController)
		{
			PersonAnimationsController personAnimationsController = mainPerson.PersonAnimationsController;
			personAnimationsController.Animator = personAnimationsController.GetComponent<Animator>();
			mainPerson.PersonAnimationsController.Init(component);
		}
	}

	public static int FindCategoryIndex(CharacterMenuCategoryItem[] items, PersonWindowCategoryType type)
	{
		if (items == null)
		{
			return -1;
		}
		for (int i = 0; i < items.Length; i++)
		{
			if ((bool)items[i] && items[i].PersonWindowCategory == type)
			{
				return i;
			}
		}
		return -1;
	}

	public void OnSelectCategory(int personWindowCategortTypeId)
	{
		int num = FindCategoryIndex(CharacterMenuCategoryItems, (PersonWindowCategoryType)personWindowCategortTypeId);
		if (num >= 0)
		{
			CharacterMenuCategoryItems[num].Toggle.SetIsOnWithoutNotify(value: true);
		}
		OnSelectCategory((PersonWindowCategoryType)personWindowCategortTypeId);
	}

	public void OnSelectCategory(PersonWindowCategoryType personWindowCategoryType)
	{
		_openedCategory = personWindowCategoryType;
		if (personWindowCategoryType != PersonWindowCategoryType.Animation && personWindowCategoryType != PersonWindowCategoryType.Poses)
		{
			mainPerson.ChangeToIdle();
			if ((bool)AnimationShop)
			{
				AnimationShop.ResetPreviewToIdle();
			}
			if (!mainPerson.middle.parent)
			{
				mainPerson.middle.parent = mainPerson.PersonAnimationsController.transform;
			}
			mainPerson.PersonAnimationsController.transform.localRotation = Quaternion.identity;
		}
		bool suppressItemSelection = _suppressItemSelection;
		_suppressItemSelection = true;
		try
		{
			ResetCenterPanel();
			switch (personWindowCategoryType)
			{
			case PersonWindowCategoryType.Skin:
				OnSkinPressed();
				break;
			case PersonWindowCategoryType.Cloth:
				OnClothesPressed();
				break;
			case PersonWindowCategoryType.Events:
				OnEventsPressed();
				break;
			case PersonWindowCategoryType.Animation:
				OnAnimationPressed();
				break;
			case PersonWindowCategoryType.Poses:
				OnPosesPressed();
				break;
			case PersonWindowCategoryType.Packs:
				OnPacksPressed();
				break;
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"[PersonWindow] OnSelectCategory failed (category={personWindowCategoryType}): {arg}");
			throw;
		}
		finally
		{
			_suppressItemSelection = suppressItemSelection;
		}
	}

	private void OnSkinPressed()
	{
		RightPanel.SetActive(value: true);
		SkinPanel.gameObject.SetActive(value: true);
		BasketView.gameObject.SetActive(value: true);
		ClothItemCategories.First((ClothItemCategory item) => item.KitType == KitType.Hair).Toggle.isOn = true;
		ScrollViewItemsCategories.verticalNormalizedPosition = 1f;
		ScrollViewItemsCategories.enabled = false;
		HideBeardTypeFemale();
		ChangeCameraTarget(0);
		SetApplyRootMotion(enable: false);
	}

	private void HideBeardTypeFemale()
	{
		if (_characterEquipmentDataService.Gender == 0)
		{
			CharacterMenuCategoryItems[0].HideItemCategory(2);
		}
		else
		{
			CharacterMenuCategoryItems[0].HideItemCategory(2, state: true);
		}
	}

	private void ResetCenterPanel()
	{
		AnimationPanel.gameObject.SetActive(value: false);
		ClothPanel.gameObject.SetActive(value: false);
		SkinPanel.gameObject.SetActive(value: false);
		EventsPanel.gameObject.SetActive(value: false);
		if ((bool)PacksPanel)
		{
			PacksPanel.gameObject.SetActive(value: false);
		}
	}

	private void OnClothesPressed()
	{
		if ((bool)ClothPanel)
		{
			ClothPanel.gameObject.SetActive(value: true);
		}
		RightPanel.SetActive(value: true);
		BasketView.gameObject.SetActive(value: true);
		ClothItemCategories.First((ClothItemCategory item) => item.KitType == KitType.Top).Toggle.isOn = true;
		ScrollViewItemsCategories.enabled = true;
		ScrollViewItemsCategories.verticalNormalizedPosition = 1f;
		ChangeCameraTarget(1);
		SetApplyRootMotion(enable: false);
	}

	private void OnEventsPressed()
	{
		if ((bool)EventsPanel)
		{
			EventsPanel.gameObject.SetActive(value: true);
		}
		RightPanel.SetActive(value: true);
		BasketView.gameObject.SetActive(value: true);
		ClothItemCategories.First((ClothItemCategory item) => item.PersonClothsEventType != PersonClothsEventType.None).Toggle.isOn = true;
		ScrollViewItemsCategories.enabled = true;
		ScrollViewItemsCategories.verticalNormalizedPosition = 1f;
		ChangeCameraTarget(1);
		SetApplyRootMotion(enable: false);
	}

	private void OnPacksPressed()
	{
		if ((bool)PacksPanel)
		{
			PacksPanel.gameObject.SetActive(value: true);
		}
		RightPanel.SetActive(value: true);
		BasketView.gameObject.SetActive(value: true);
		ScrollViewItemsCategories.verticalNormalizedPosition = 1f;
		ScrollViewItemsCategories.enabled = false;
		SetupPackCategories();
		ChangeCameraTarget(1);
		SetApplyRootMotion(enable: false);
	}

	private void FillLeftPanelForPack(int packId)
	{
		bool suppressItemSelection = _suppressItemSelection;
		_suppressItemSelection = true;
		try
		{
			int num = mainPerson.Gender;
			PersonKitConfig personKitConfig = ((num == 1) ? MaleKitConfig : FemaleKitConfig);
			List<PersonKitDataWrapper> list = new List<PersonKitDataWrapper>();
			foreach (PackClothItem allPackItem in _bundleOfferCatalog.GetAllPackItems(num))
			{
				if (allPackItem.PackId != packId)
				{
					continue;
				}
				if (allPackItem.TypeId < 0 || allPackItem.TypeId >= personKitConfig.allKits.Length)
				{
					Debug.LogWarning($"[PersonWindow] Pack {allPackItem.PackId}: TypeId {allPackItem.TypeId} out of range");
					continue;
				}
				PersonElements.PersonKitData[] array = personKitConfig.allKits[allPackItem.TypeId];
				if (allPackItem.ItemId < 0 || allPackItem.ItemId >= array.Length)
				{
					Debug.LogWarning($"[PersonWindow] Pack {allPackItem.PackId}: ItemId {allPackItem.ItemId} (TypeId {allPackItem.TypeId}) out of range");
				}
				else
				{
					list.Add(new PersonKitDataWrapper(array[allPackItem.ItemId], (KitType)allPackItem.TypeId, PersonClothsEventType.None, allPackItem.PackId));
				}
			}
			for (int i = 0; i < _cachedLeftItems.Count; i++)
			{
				_cachedLeftItems[i].gameObject.SetActive(value: false);
				_cachedLeftItems[i].Clear();
			}
			ToggleGroup component = LeftColumn.GetComponent<ToggleGroup>();
			for (int j = 0; j < list.Count; j++)
			{
				if (_cachedLeftItems.Count > j)
				{
					_cachedLeftItems[j].Init(list[j], component, selected: false, mainPerson, list[j].PersonKitData.Id);
					_cachedLeftItems[j].gameObject.SetActive(value: true);
					continue;
				}
				OneKitMenuItem oneKitMenuItem = _prefabFactory.Instantiate(oneKitPrefab, LeftColumn);
				oneKitMenuItem.OnSelectedOwnedKitItem += (OneKitMenuItem item) =>
				{
					LastOwnedOneKitMenuItem = item;
				};
				oneKitMenuItem.OnSelected += OnItemSelected;
				_cachedLeftItems.Add(oneKitMenuItem);
				_cachedLeftItems[j].Init(list[j], component, selected: false, mainPerson, list[j].PersonKitData.Id);
				_cachedLeftItems[j].gameObject.SetActive(value: true);
			}
			OnPersonWindowOpen(LeftColumn).Forget();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[PersonWindow] FillLeftPanelForPack failed (packId={packId}): {arg}");
			throw;
		}
		finally
		{
			_suppressItemSelection = suppressItemSelection;
		}
	}

	private void SetupPackCategories()
	{
		if (PackCategories == null)
		{
			FillLeftPanelForPack(-1);
			return;
		}
		int gender = mainPerson.Gender;
		HashSet<int> hashSet = new HashSet<int>();
		foreach (PackClothItem allPackItem in _bundleOfferCatalog.GetAllPackItems(gender))
		{
			hashSet.Add(allPackItem.PackId);
		}
		PackCategory packCategory = null;
		PackCategory packCategory2 = null;
		int? num = _personWindowAdditionalData?.OpenPackId;
		PackCategory[] packCategories = PackCategories;
		foreach (PackCategory packCategory3 in packCategories)
		{
			bool flag = hashSet.Contains(packCategory3.PackId);
			packCategory3.gameObject.SetActive(flag);
			if (flag)
			{
				if (packCategory == null)
				{
					packCategory = packCategory3;
				}
				if (num.HasValue && packCategory3.PackId == num.Value)
				{
					packCategory2 = packCategory3;
				}
			}
		}
		PackCategory packCategory4 = packCategory2 ?? packCategory;
		if (packCategory4 == null)
		{
			FillLeftPanelForPack(-1);
		}
		else if (packCategory4.Toggle.isOn)
		{
			FillLeftPanelForPack(packCategory4.PackId);
		}
		else
		{
			packCategory4.Toggle.isOn = true;
		}
	}

	private void OnAnimationPressed()
	{
		OpenEmbeddedShop(AnimationShopSection.Animations);
	}

	private void OnPosesPressed()
	{
		OpenEmbeddedShop(AnimationShopSection.Poses);
	}

	private void OpenEmbeddedShop(AnimationShopSection section)
	{
		RightPanel.SetActive(value: false);
		BasketView.gameObject.SetActive(value: false);
		AnimationPanel.gameObject.SetActive(value: true);
		ChangeCameraTarget(1);
		mainPerson.middle.parent = null;
		AnimationShop.OpenShop(section);
		SetApplyRootMotion(enable: true);
	}

	private int GetCameraIndexByClothType(KitType kitType)
	{
		if (kitType >= KitType.Hair)
		{
			switch (kitType)
			{
			case KitType.Glasses:
				break;
			case KitType.Shoes:
				return 2;
			default:
				return 1;
			}
		}
		return 0;
	}

	private void SetApplyRootMotion(bool enable)
	{
		if ((bool)mainPerson && (bool)mainPerson.PersonAnimationsController && (bool)mainPerson.PersonAnimationsController.Animator)
		{
			mainPerson.PersonAnimationsController.Animator.applyRootMotion = enable;
		}
	}

	public void FillLeftPanel(KitType kitType, PersonClothsEventType personClothsEventType)
	{
		bool suppressItemSelection = _suppressItemSelection;
		_suppressItemSelection = true;
		try
		{
			HideBeardTypeFemale();
			List<PersonKitDataWrapper> personKits = GetPersonKits(((int)mainPerson.Gender == 1) ? MaleKitConfig : FemaleKitConfig, kitType, personClothsEventType);
			for (int i = 0; i < _cachedLeftItems.Count; i++)
			{
				_cachedLeftItems[i].gameObject.SetActive(value: false);
				_cachedLeftItems[i].Clear();
			}
			ToggleGroup component = LeftColumn.GetComponent<ToggleGroup>();
			for (int j = 0; j < personKits.Count; j++)
			{
				if (_cachedLeftItems.Count > j)
				{
					_cachedLeftItems[j].Init(personKits[j], component, selectedNowKits[kitType] == personKits[j].PersonKitData.Id, mainPerson, personKits[j].PersonKitData.Id);
					_cachedLeftItems[j].gameObject.SetActive(value: true);
					continue;
				}
				OneKitMenuItem oneKitMenuItem = _prefabFactory.Instantiate(oneKitPrefab, LeftColumn);
				oneKitMenuItem.OnSelectedOwnedKitItem += (OneKitMenuItem item) =>
				{
					LastOwnedOneKitMenuItem = item;
				};
				oneKitMenuItem.OnSelected += OnItemSelected;
				_cachedLeftItems.Add(oneKitMenuItem);
				_cachedLeftItems[j].Init(personKits[j], component, selectedNowKits[kitType] == personKits[j].PersonKitData.Id, mainPerson, personKits[j].PersonKitData.Id);
				_cachedLeftItems[j].gameObject.SetActive(value: true);
			}
			OnPersonWindowOpen(LeftColumn).Forget();
			ChangeCameraTarget(GetCameraIndexByClothType(kitType));
		}
		catch (Exception arg)
		{
			Debug.LogError($"[PersonWindow] FillLeftPanel failed (kitType={kitType}, event={personClothsEventType}): {arg}");
			throw;
		}
		finally
		{
			_suppressItemSelection = suppressItemSelection;
		}
	}

	private List<PersonKitDataWrapper> GetPersonKits(PersonKitConfig genderKitConfig, KitType kitType, PersonClothsEventType personClothsEventType)
	{
		int gender = mainPerson.Gender;
		List<PersonKitDataWrapper> list = new List<PersonKitDataWrapper>();
		if (personClothsEventType == PersonClothsEventType.None)
		{
			foreach (PersonElements.PersonKitData item in (from kit in genderKitConfig.GetKitsByTypeId((int)kitType)
				orderby kit.price2
				select kit).ToList())
			{
				if (item.PersonClothsEventType == PersonClothsEventType.None && !_bundleOfferCatalog.TryGetPackIdForItem(gender, (int)kitType, item.Id, out var _))
				{
					list.Add(new PersonKitDataWrapper(item, kitType, personClothsEventType));
				}
			}
		}
		else
		{
			PersonElements.PersonKitData[][] allKits = genderKitConfig.allKits;
			for (int num = 0; num < allKits.Length; num++)
			{
				PersonElements.PersonKitData[] array = allKits[num];
				foreach (PersonElements.PersonKitData personKitData in array)
				{
					if (personKitData.PersonClothsEventType == personClothsEventType && !_bundleOfferCatalog.TryGetPackIdForItem(gender, num, personKitData.Id, out var _))
					{
						list.Add(new PersonKitDataWrapper(personKitData, (KitType)num, personClothsEventType));
					}
				}
			}
		}
		return list;
	}

	public void OnItemSelected(OneKitMenuItem personKit)
	{
		if (_suppressItemSelection)
		{
			return;
		}
		if (personKit.PackId >= 1 && !_charactersService.GetEquipmentsData(mainPerson.Gender).IsItemBought((int)personKit.kitType, personKit.id) && !_bundleOfferCatalog.TryGetActiveOfferForPack(personKit.PackId, out var _))
		{
			ShowPackOfferUnavailableMessage();
			return;
		}
		_lastSelectedOneKitMenuItem = personKit;
		if (personKit.PackId < 1 && (bool)_lastSelectedOneKitMenuItem && _lastSelectedOneKitMenuItem.PersonKitData != null)
		{
			if (_lastSelectedOneKitMenuItem.PersonKitData.AvailableToBuyStatus == OneKitMenuItem.AvailableToBuyType.Locked)
			{
				ShowAvailableForKingRankMessage();
			}
			else if (!IsItemActive(_lastSelectedOneKitMenuItem.PersonKitData.PersonClothsEventType))
			{
				EquipmentsData equipmentsData = _charactersService.GetEquipmentsData(mainPerson.Gender);
				if ((int)_lastSelectedOneKitMenuItem.PersonKitData.obscuredPrice != 0 && !equipmentsData.IsItemBought((int)_lastSelectedOneKitMenuItem.kitType, _lastSelectedOneKitMenuItem.PersonKitData.Id))
				{
					ShowAvailableForEventMessage();
				}
			}
		}
		RemoveLockedClothIgnoreLast();
		LoadThisKit(personKit.kitType, personKit.id);
		UpdateSavedKits();
	}

	private void ShowAvailableForKingRankMessage()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("ShopCharacter.King.AvailableForKingRank"), 2f, NotificationType.Error).Forget();
	}

	private void ShowAvailableForEventMessage()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("ShopCharacter.Events.AvailableForActiveEvent"), 2f, NotificationType.Error).Forget();
	}

	private void ShowPackOfferUnavailableMessage()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("SuitPack.PackNotAvailable"), 2f, NotificationType.Error).Forget();
	}

	private void ShowNotAvailableForPurchase()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("ShopCharacter.NotAvailableForPurchase"), 2f, NotificationType.Error).Forget();
	}

	private void loadKitsAfterPackTookOff()
	{
		EquipmentsData equipmentsData = _charactersService.GetEquipmentsData(mainPerson.Gender);
		_charactersService.LoadPersonEquipment(Person, 5, 0);
		selectedNowKits[KitType.Top] = 0;
		int num = equipmentsData.SelectedEquipments[6];
		_charactersService.LoadPersonEquipment(Person, 6, num);
		selectedNowKits[KitType.Gloves] = num;
		int num2 = equipmentsData.SelectedEquipments[8];
		if (num2 < 0)
		{
			equipmentsData.SetSelectedItem(8, 0);
			_charactersService.Save();
			num2 = 0;
		}
		_charactersService.LoadPersonEquipment(Person, 8, num2);
		selectedNowKits[KitType.Pants] = num2;
		int num3 = equipmentsData.SelectedEquipments[9];
		if (num3 < 0)
		{
			equipmentsData.SetSelectedItem(9, 0);
			_charactersService.Save();
			num3 = 0;
		}
		_charactersService.LoadPersonEquipment(Person, 9, num3);
		selectedNowKits[KitType.Shoes] = num3;
		MonoBehaviour.print(num + " " + num2 + " " + num2);
	}

	public void LoadThisKit(KitType kitType, int id)
	{
		if ((kitType == KitType.Pants || kitType == KitType.Shoes || kitType == KitType.Gloves || kitType == KitType.Top) && (bool)mainPerson.allKits[5])
		{
			Equipment component = mainPerson.allKits[5].GetComponent<Equipment>();
			if (component.pack == PersonElements.Pack.noPants && kitType != KitType.Top)
			{
				if (kitType == KitType.Pants)
				{
					loadKitsAfterPackTookOff();
				}
			}
			else if (component.pack == PersonElements.Pack.noPantsShoes && kitType != KitType.Top)
			{
				if (kitType == KitType.Pants || kitType == KitType.Shoes)
				{
					loadKitsAfterPackTookOff();
				}
			}
			else if (component.pack == PersonElements.Pack.noPantsGloves && kitType != KitType.Top)
			{
				if (kitType == KitType.Pants || kitType == KitType.Gloves)
				{
					loadKitsAfterPackTookOff();
				}
			}
			else if (component.pack != PersonElements.Pack.no)
			{
				MonoBehaviour.print("e.pack != PersonElements.Pack.no");
				loadKitsAfterPackTookOff();
			}
		}
		if (kitType == KitType.Shoes && (bool)mainPerson.allKits[8])
		{
			Equipment component2 = mainPerson.allKits[8].GetComponent<Equipment>();
			if (_pantsWithNoShoes.Contains(component2.id))
			{
				_charactersService.LoadPersonEquipment(mainPerson.gameObject, 8, 0);
				Debug.LogWarning("Pants with no shoes. Set default pants");
			}
		}
		if (kitType == KitType.Pants && _pantsWithNoShoes.Contains(id))
		{
			_charactersService.LoadPersonEquipment(mainPerson.gameObject, 9, -1);
			Debug.LogWarning("Pants with no shoes. Remove shoes.");
		}
		if (kitType == KitType.Hair && (bool)mainPerson.allKits[5] && mainPerson.allKits[5].GetComponent<Equipment>().hairToSet == -2)
		{
			return;
		}
		if (kitType == KitType.Caps && (bool)mainPerson.allKits[5] && mainPerson.allKits[5].GetComponent<Equipment>().hairToSet != -1)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("PersonWindow.YouCanTWearTheCapWithThisTypeOfClothes"), 2f);
		}
		else
		{
			if (kitType == KitType.Beard && (((bool)mainPerson.allKits[4] && mainPerson.allKits[4].GetComponent<Equipment>().HideBeard) || ((bool)mainPerson.allKits[3] && mainPerson.allKits[3].GetComponent<Equipment>().HideBeard)))
			{
				return;
			}
			if ((bool)mainPerson.allKits[(int)kitType])
			{
				Equipment component3 = mainPerson.allKits[(int)kitType].GetComponent<Equipment>();
				if (component3.typeID == (int)kitType && component3.id == id && _prevEquipmentTypeId != 5)
				{
					bool num = component3.typeID == 1 || ((int)mainPerson.Gender == 0 && component3.typeID == 0) || component3.typeID == 8;
					_prevEquipmentTypeId = component3.typeID;
					if (!num)
					{
						DestroyKit(kitType);
						int num2 = equipmentsData.SelectedEquipments[(int)kitType];
						if (num2 >= 0 && num2 != id)
						{
							_charactersService.LoadPersonEquipment(Person, (int)kitType, num2);
							selectedNowKits[kitType] = num2;
							_charactersService.LoadAllMask(Person);
						}
						Observable.NextFrame().Subscribe((Unit _) =>
						{
							updatePrice();
						}).AddTo(this);
					}
					return;
				}
				_prevEquipmentTypeId = component3.typeID;
			}
			_charactersService.LoadPersonEquipment(Person, (int)kitType, id, kitType == KitType.Hair);
			if (kitType == KitType.Top && selectedNowKits.TryGetValue(KitType.Hair, out var value))
			{
				if (value < 0)
				{
					value = equipmentsData.SelectedEquipments[0];
				}
				if ((bool)mainPerson.allKits[0])
				{
					if (mainPerson.allKits[0].GetComponent<Equipment>().id != value)
					{
						_charactersService.LoadPersonEquipment(Person, 0, value);
					}
				}
				else
				{
					_charactersService.LoadPersonEquipment(Person, 0, value);
				}
			}
			_charactersService.LoadAllMask(Person);
			selectedNowKits[kitType] = id;
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				updatePrice();
			}).AddTo(this);
		}
	}

	private void updatePrice()
	{
		int num = CountCoins();
		hasUnAppliedKit();
		BasketView.TotalPriceText.text = num.ToString();
		UnpurchasedItemsWindowView.TotalPriceText.text = num.ToString();
		if (UnpurchasedItemsWindowView.gameObject.activeSelf)
		{
			bool flag = num > 0 || BasketView.HaveLockedItems;
			UnpurchasedItemsWindowView.Total.SetActive(flag);
			if (!flag)
			{
				UnpurchasedItemsWindowView.BuyButton.Disable();
			}
			SyncUnpurchasedItemsWindowWithBasket();
		}
	}

	private void SyncUnpurchasedItemsWindowWithBasket()
	{
		foreach (KeyValuePair<Enum, BasketItem> item in UnpurchasedItemsWindowView.BasketItemsDic)
		{
			if (item.Value.gameObject.activeSelf && (!BasketView.BasketItemsDic.TryGetValue(item.Key, out var value) || !value.gameObject.activeSelf))
			{
				item.Value.gameObject.SetActive(value: false);
			}
		}
	}

	private bool hasUnAppliedKit()
	{
		int num = 0;
		int genderInput = mainPerson.Gender;
		EquipmentsData equipmentsData = _charactersService.GetEquipmentsData(genderInput);
		foreach (KitType key in selectedNowKits.Keys)
		{
			if (selectedNowKits[key] != -2)
			{
				int num2 = equipmentsData.SelectedEquipments[num];
				if (selectedNowKits[key] != num2)
				{
					return true;
				}
			}
			num++;
		}
		return false;
	}

	public async UniTask<bool> TryBuyAll(EquipmentsData equipmentsData)
	{
		bool value = true;
		ObscuredInt overAllCoin = CountCoins();
		AdvancedInt price = new AdvancedInt(overAllCoin);
		(await _currencyManagerController.TryCoinsSpend(SpendCoinsType.PersonEquipment, price, () =>
		{
			LogAnalyticEvent(PurchaseStatus.PURCHASE_COMPLETED);
			CompletePurchase(equipmentsData);
			if ((int)overAllCoin > 0)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("OneKitMenuItem.Purchased"), 2f);
			}
		}, () =>
		{
			LogAnalyticEvent(PurchaseStatus.INSUFFICIENT_FUNDS);
		})).Get(out value);
		return value;
	}

	private int CountCoins()
	{
		ObscuredInt obscuredInt = 0;
		int num = mainPerson.Gender;
		PersonKitConfig personKitConfig = ((num == 1) ? MaleKitConfig : FemaleKitConfig);
		equipmentsData = _charactersService.GetEquipmentsData(num);
		BasketView.RemoveAllItemsFromBasket();
		GameObject[] allKits = mainPerson.allKits;
		foreach (GameObject gameObject in allKits)
		{
			if (!gameObject)
			{
				continue;
			}
			Equipment component = gameObject.GetComponent<Equipment>();
			if (equipmentsData.IsItemBought(component.typeID, component.id))
			{
				continue;
			}
			if (_bundleOfferCatalog.TryGetPackIdForItem(num, component.typeID, component.id, out var packId))
			{
				BasketView.AddItemToBasket((KitType)component.typeID, new BasketItemPersonKitInfo(0, component.id, (KitType)component.typeID, isLocked: false, packId));
				continue;
			}
			PersonElements.PersonKitData personKitData = personKitConfig.allKits[component.typeID][component.id];
			if (Mathf.Abs(personKitData.obscuredPrice) != Mathf.Abs(personKitData.price2))
			{
				if (Application.isEditor)
				{
					Debug.LogError("price is not the same, check please");
				}
				if (ObscuredPrefs.GetInt("checkMonyCoin") == 1)
				{
					ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version, 1);
				}
			}
			obscuredInt = (int)obscuredInt + Mathf.Abs(personKitData.obscuredPrice);
			if ((int)personKitData.obscuredPrice > 0 || personKitData.AvailableToBuyStatus == OneKitMenuItem.AvailableToBuyType.Locked || !IsItemActive(personKitData.PersonClothsEventType))
			{
				bool isLocked = personKitData.AvailableToBuyStatus == OneKitMenuItem.AvailableToBuyType.Locked || !IsItemActive(personKitData.PersonClothsEventType);
				BasketView.AddItemToBasket((KitType)component.typeID, new BasketItemPersonKitInfo(personKitData.obscuredPrice, component.id, (KitType)component.typeID, isLocked));
			}
		}
		return obscuredInt;
	}

	private void SaveAndContinue()
	{
		if (BasketView.BasketItemsDic.Values.Where((BasketItem item) => item.gameObject.activeSelf).ToArray().Length != 0)
		{
			OpenUnpurchasedWindow(Continue, () =>
			{
				BackButton.gameObject.SetActive(value: true);
			});
		}
		else
		{
			Continue();
		}
	}

	private void Continue()
	{
		SaveAndContinueButton.onClick.RemoveListener(SaveAndContinue);
		_presetGrantSession.Clear();
		_charactersService.Save();
		_guestAccountCreator.CreateGuestAccountIfNeeds().Forget();
		_managerUI.CloseBalanceWindow();
		UnityEngine.Object.Destroy(V.DriverPerson);
		V.CoreGameManager.IsPlayerSetuping = false;
		PlayerPrefs.SetInt("PersonCustomizationCompleted", 1);
		if (V.CoreGameManager.NikNameInputText.Length > 0)
		{
			_registrationController.PlayerNameChanged(V.CoreGameManager.NikNameInputText);
		}
		_characterCreationAnalytic.LogCharacterCreation(CharacterCreationType.Continue);
	}

	private void RemoveBasketItem(BasketItemInfoBase info)
	{
		if (info is BasketItemPersonKitInfo basketItemPersonKitInfo)
		{
			if (equipmentsData.SelectedEquipments[(int)basketItemPersonKitInfo.KitType] < 0)
			{
				DestroyKit(basketItemPersonKitInfo.KitType);
			}
			else
			{
				LoadThisKit(basketItemPersonKitInfo.KitType, equipmentsData.SelectedEquipments[(int)basketItemPersonKitInfo.KitType]);
			}
			updatePrice();
		}
	}

	private void UpdateSavedKits()
	{
		if (mainPerson == null || !mainPerson.EquipmentLoaded)
		{
			return;
		}
		Equipment top = (mainPerson.allKits[5] ? mainPerson.allKits[5].GetComponent<Equipment>() : null);
		Equipment cap = (mainPerson.allKits[3] ? mainPerson.allKits[3].GetComponent<Equipment>() : null);
		for (int i = 0; i < mainPerson.allKits.Length; i++)
		{
			if (i == 0 && HeadwearControlsHairSlot(cap, top))
			{
				continue;
			}
			if (mainPerson.allKits[i] == null)
			{
				equipmentsData.SetSelectedItem(i, -1);
				continue;
			}
			Equipment component = mainPerson.allKits[i].GetComponent<Equipment>();
			PersonKitConfig personKitConfig = (((int)mainPerson.Gender == 0) ? FemaleKitConfig : MaleKitConfig);
			if (equipmentsData.IsItemBought(component.typeID, component.id) || !_bundleOfferCatalog.TryGetPackIdForItem(mainPerson.Gender, component.typeID, component.id, out var _))
			{
				bool num = (int)personKitConfig.allKits[component.typeID][component.id].obscuredPrice > 0;
				bool flag = !equipmentsData.IsItemBought(component.typeID, component.id);
				bool flag2 = !IsItemActive(personKitConfig.allKits[component.typeID][component.id].PersonClothsEventType);
				bool flag3 = personKitConfig.allKits[component.typeID][component.id].AvailableToBuyStatus == OneKitMenuItem.AvailableToBuyType.Locked;
				if (!(((num & flag) || (flag & flag2)) | flag3))
				{
					equipmentsData.SetSelectedItem(component.typeID, component.id);
				}
			}
		}
		foreach (OneKitMenuItem cachedLeftItem in _cachedLeftItems)
		{
			if ((bool)cachedLeftItem && cachedLeftItem.gameObject.activeSelf && cachedLeftItem.kitType != KitType.None)
			{
				cachedLeftItem.checkState(mainPerson.Gender);
			}
		}
		_charactersService.Save();
	}

	private void CompletePurchase(EquipmentsData equipmentsData)
	{
		GameObject gameObject = mainPerson.allKits[5];
		GameObject gameObject2 = mainPerson.allKits[3];
		Equipment component = null;
		Equipment component2 = null;
		if ((bool)gameObject)
		{
			gameObject.TryGetComponent<Equipment>(out component);
		}
		if ((bool)gameObject2)
		{
			gameObject2.TryGetComponent<Equipment>(out component2);
		}
		for (int i = 0; i < mainPerson.allKits.Length; i++)
		{
			if (i == 0 && HeadwearControlsHairSlot(component2, component))
			{
				continue;
			}
			if (!mainPerson.allKits[i])
			{
				equipmentsData.SetSelectedItem(i, -1);
				continue;
			}
			Equipment component3 = mainPerson.allKits[i].GetComponent<Equipment>();
			if (equipmentsData.IsItemBought(component3.typeID, component3.id) || !_bundleOfferCatalog.TryGetPackIdForItem(mainPerson.Gender, component3.typeID, component3.id, out var _))
			{
				if (!equipmentsData.IsItemBought(component3.typeID, component3.id))
				{
					equipmentsData.BuyItem(component3.typeID, component3.id);
				}
				equipmentsData.SetSelectedItem(component3.typeID, component3.id);
			}
		}
		foreach (OneKitMenuItem cachedLeftItem in _cachedLeftItems)
		{
			if ((bool)cachedLeftItem && cachedLeftItem.gameObject.activeSelf && cachedLeftItem.kitType != KitType.None)
			{
				cachedLeftItem.checkState(mainPerson.Gender);
			}
		}
		_charactersService.Save();
		updatePrice();
	}

	private void ChangeCameraTarget(int type)
	{
		if (ScenesHelper.IsMenuSceneActive() && _lastCamIndex != type)
		{
			_lastCamIndex = type;
			switch (type)
			{
			case 0:
				_cameraManager.ChangeTarget(mainPerson.head);
				_cameraManager.ForceSetCameraZoom(25f);
				_freeDriveProvider.CarCameras.Target = mainPerson.head;
				_freeDriveProvider.CarCameras.distance = 1.5f;
				break;
			case 2:
				_cameraManager.ChangeTarget(mainPerson.foot);
				_cameraManager.ForceSetCameraZoom(40f);
				_freeDriveProvider.CarCameras.Target = mainPerson.foot;
				field = 70f;
				_freeDriveProvider.CarCameras.distance = 2.5f;
				break;
			case 1:
				_cameraManager.ChangeTarget(mainPerson.middle);
				_cameraManager.PersonShopCam.ResetDeadZone();
				_cameraManager.ForceSetCameraZoom(50f);
				_freeDriveProvider.CarCameras.Target = mainPerson.middle;
				_freeDriveProvider.CarCameras.distance = 4f;
				break;
			}
		}
	}

	private List<KitType> GetLockedClothIDs()
	{
		List<KitType> list = new List<KitType>();
		int num = mainPerson.Gender;
		PersonKitConfig personKitConfig = ((num == 1) ? MaleKitConfig : FemaleKitConfig);
		equipmentsData = _charactersService.GetEquipmentsData(num);
		GameObject[] allKits = mainPerson.allKits;
		foreach (GameObject gameObject in allKits)
		{
			if (!gameObject)
			{
				continue;
			}
			Equipment component = gameObject.GetComponent<Equipment>();
			if (!equipmentsData.IsItemBought(component.typeID, component.id))
			{
				PersonElements.PersonKitData personKitData = personKitConfig.allKits[component.typeID][component.id];
				if (personKitData.AvailableToBuyStatus == OneKitMenuItem.AvailableToBuyType.Locked || !IsItemActive(personKitData.PersonClothsEventType))
				{
					list.Add((KitType)component.typeID);
				}
			}
		}
		return list;
	}

	private void RemoveLockedClothIgnoreLast()
	{
		List<KitType> lockedClothIDs = GetLockedClothIDs();
		for (int i = 0; i < lockedClothIDs.Count; i++)
		{
			if (!_lastSelectedOneKitMenuItem || _lastSelectedOneKitMenuItem.kitType != lockedClothIDs[i])
			{
				DestroyKit(lockedClothIDs[i]);
			}
		}
	}

	public static bool HeadwearControlsHairSlot(Equipment cap, Equipment top)
	{
		if (!cap || cap.hairToSet == -1)
		{
			if ((bool)top)
			{
				return top.hairToSet != -1;
			}
			return false;
		}
		return true;
	}

	private void DestroyKit(KitType kitType)
	{
		UnityEngine.Object.Destroy(mainPerson.allKits[(int)kitType]);
		mainPerson.allKits[(int)kitType] = null;
		selectedNowKits[kitType] = -1;
		if (kitType == KitType.Caps)
		{
			CharactersService.RestoreHairHiddenByHat(mainPerson);
		}
		if ((kitType == KitType.Face || kitType == KitType.Caps) && (int)mainPerson.Gender == 1 && (bool)mainPerson.allKits[1] && mainPerson.allKits[1].GetComponent<Equipment>().id == 2)
		{
			_charactersService.SetAsianHead(mainPerson, makeBald: true);
		}
		if (kitType == KitType.Top || kitType == KitType.Caps)
		{
			int num = -1;
			if ((bool)mainPerson.allKits[0])
			{
				num = mainPerson.allKits[0].GetComponent<Equipment>().id;
			}
			int num2 = equipmentsData.SelectedEquipments[0];
			if (selectedNowKits.TryGetValue(KitType.Hair, out var value) && value >= 0)
			{
				num2 = value;
			}
			if ((int)mainPerson.Gender == 0 && num2 < 0 && num < 0)
			{
				num2 = 0;
				equipmentsData.SetSelectedItem(0, 0);
				_charactersService.Save();
			}
			if (num != num2 && num2 != -1)
			{
				_charactersService.LoadPersonEquipment(mainPerson.gameObject, 0, num2);
			}
			if (kitType == KitType.Top)
			{
				int num3 = -1;
				if ((bool)mainPerson.allKits[3])
				{
					num3 = mainPerson.allKits[3].GetComponent<Equipment>().id;
				}
				int num4 = equipmentsData.SelectedEquipments[3];
				if (num3 != num4)
				{
					_charactersService.LoadPersonEquipment(mainPerson.gameObject, 3, num4);
				}
			}
		}
		if (kitType == KitType.Mask && (bool)mainPerson.allKits[2])
		{
			mainPerson.allKits[2].SetActive(value: true);
		}
		_charactersService.LoadAllMask(Person);
	}

	private void LogAnalyticEvent(PurchaseStatus purchaseStatus)
	{
		int num = mainPerson.Gender;
		PersonKitConfig personKitConfig = ((num == 1) ? MaleKitConfig : FemaleKitConfig);
		equipmentsData = _charactersService.GetEquipmentsData(num);
		GameObject[] allKits = mainPerson.allKits;
		foreach (GameObject gameObject in allKits)
		{
			if (!(gameObject == null))
			{
				Equipment component = gameObject.GetComponent<Equipment>();
				if (!equipmentsData.IsItemBought(component.typeID, component.id))
				{
					PersonElements.PersonKitData personKitData = personKitConfig.allKits[component.typeID][component.id];
					_virtualPurchaseAnalytic.LogPurchaseVirtual_PersonCustomization(component.typeID, personKitData.obscuredPrice, component.id, purchaseStatus);
				}
			}
		}
	}

	public void BackToPresets()
	{
		if (!BasketView.BasketEmpty)
		{
			OpenUnpurchasedWindow(() =>
			{
				_managerUI.CloseBalanceWindow();
				_managerUI.ShowFirstGameWindow();
			}, () =>
			{
				BackButton.gameObject.SetActive(value: true);
			});
		}
		else
		{
			_managerUI.CloseBalanceWindow();
			_managerUI.ShowFirstGameWindow();
		}
	}

	private void OpenUnpurchasedWindow(Action continueNotBuyButtonAction, Action backButtonAction)
	{
		UnpurchasedItemsWindowView.gameObject.SetActive(value: true);
		UnpurchasedItemsWindowView.Total.SetActive(value: true);
		UnpurchasedItemsWindowView.BuyButton.Enable();
		UnpurchasedItemsWindowView.TotalPriceText.text = BasketView.TotalPriceText.text;
		MainWindow.SetActive(value: false);
		AnimationShop.gameObject.SetActive(value: false);
		_managerUI.OnBackButtonActivated?.Invoke(obj: false);
		BackButton.gameObject.SetActive(value: false);
		UnpurchasedItemsWindowView.UnsubscribeUI();
		UnpurchasedItemsWindowView.BasketItemCreated += (BasketItem item) =>
		{
			item.OnRemoveItem += RemoveBasketItem;
		};
		UnpurchasedItemsWindowView.BackButton.onClick.AddListener(() =>
		{
			_managerUI.OnActiveAllBalanceWindow?.Invoke(obj: true);
			MainWindow.SetActive(value: true);
			AnimationShop.gameObject.SetActive(value: true);
			UnpurchasedItemsWindowView.gameObject.SetActive(value: false);
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		});
		UnpurchasedItemsWindowView.BackButton.onClick.AddListener(() =>
		{
			backButtonAction();
		});
		UnpurchasedItemsWindowView.ContinueNotBuyButton.onClick.AddListener(() =>
		{
			continueNotBuyButtonAction();
		});
		UnpurchasedItemsWindowView.BuyButton.Button.onClick.AddListener(BuyOrSave);
		UnpurchasedItemsWindowView.ContinueNotBuyButton.onClick.AddListener(() =>
		{
			UnpurchasedItemsWindowView.BasketItemsDic.Values.Where((BasketItem item) => item.gameObject.activeSelf).ForEach((BasketItem item) =>
			{
				item.RemoveItemButton.onClick.Invoke();
			});
			UnpurchasedItemsWindowView.UnsubscribeUI();
		});
		BasketItem[] array = BasketView.BasketItemsDic.Values.Where((BasketItem item) => item.gameObject.activeSelf).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			if (array[num].Info is BasketItemPersonKitInfo basketItemPersonKitInfo)
			{
				Texture texture = Resources.Load(CharactersService.GetPathForKit(_characterEquipmentDataService.Gender, (int)basketItemPersonKitInfo.KitType) + "icons/" + basketItemPersonKitInfo.Id + "_icon", typeof(Texture)) as Texture;
				UnpurchasedItemsWindowView.AddItem(basketItemPersonKitInfo.KitType, basketItemPersonKitInfo, texture);
			}
		}
		_managerUI.SetCurrentWindow(this);
	}

	private bool IsItemActive(PersonClothsEventType personClothsEventType)
	{
		if (personClothsEventType != PersonClothsEventType.None)
		{
			return _personClothesEventChecker.IsEventActive(personClothsEventType);
		}
		return true;
	}
}
