using System;
using System.Collections.Generic;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Cargo;
using Service.Delivery;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Animations.Emotes;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Multiplayer.Treasure;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Offers.Reward;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.WorldInteraction;

public class AnimationShop : WindowScript, IBackableWindow
{
	private const string BundleNotAvailableKey = "SuitPack.PackNotAvailable";

	public const string CurrentAnimationPrefs = "currentAnimation_";

	public AnimationDataSO animationDataSo;

	public GameObject shopUi;

	public GameObject garderobUi;

	public GameObject buyButton;

	public GameObject setButtonRoot;

	public ToggleGroup ToggleGroup;

	public AnimationShopItem itemPrefab;

	[SerializeField]
	private AnimationShopItem MovesPackItemPrefab;

	public GameObject itemsParent;

	public Camera previewCamera;

	public RawImage previewTexture;

	public GameObject previewPerson;

	private PersonAnimationsController _previewPersonAnimationsController;

	private WorldInteractionController _previewWic;

	private AnimationShopItem currentAnimationItem;

	private RenderTexture previewRenderTexture;

	private Quaternion previewPersonDefaultRotation = Quaternion.identity;

	private int _currentGender = 1;

	public PersonWindow PersonWindow;

	private List<AnimationShopItem> _cachedAnimationShopItems = new List<AnimationShopItem>();

	private List<AnimationShopItem> _cachedPoseShopItems = new List<AnimationShopItem>();

	private List<AnimationShopItem> _cachedDanceShopItems = new List<AnimationShopItem>();

	private IPrefabFactory _prefabFactory;

	private const int AnimationPresetsCount = 8;

	private CurrencyManagerController _currencyManagerController;

	private NewMoveController _newMoveController;

	private IDisposable _delayedReset;

	private IDisposable _turnOffAnimationDisposable;

	[SerializeField]
	private TextMeshProUGUI TextPrice;

	[SerializeField]
	private GameObject BuyForLabel;

	[SerializeField]
	private GameObject CoinIcon;

	[SerializeField]
	private Button EquipButton;

	[SerializeField]
	private AnimationShopTabCategory[] TabCategories;

	[SerializeField]
	private GameObject TabRowRoot;

	[SerializeField]
	private ScrollRect ItemsScrollRect;

	private AnimationShopSection _currentSection;

	private AnimationShopTab _currentTab;

	private int _pendingFocusBundleId = -1;

	private IEmoteEntry _pendingBundleEntry;

	private EmoteCatalog _catalog;

	private BundleOfferCatalog _bundleOfferCatalog;

	private AnimationShopOpenRequest _openRequest;

	private bool _initialized;

	private ParkingMenuUI _parkingMenuUI;

	private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private Action _turnOffAction;

	private bool HasTabs
	{
		get
		{
			if (TabCategories != null)
			{
				return TabCategories.Length != 0;
			}
			return false;
		}
	}

	protected override InputGroupsEnum WindowInputGroup
	{
		get
		{
			if (!FlagsStorage.IsMainMenu)
			{
				return InputGroupsEnum.Ui;
			}
			return InputGroupsEnum.None;
		}
	}

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, CurrencyManagerController currencyManagerController, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, VirtualPurchaseAnalytic virtualPurchaseAnalytic, OptionalDependency<ParkingMenuUI> parkingMenuUI, BundleOfferCatalog bundleOfferCatalog, AnimationShopOpenRequest openRequest, EmoteCatalog emoteCatalog)
	{
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_parkingMenuUIDep = parkingMenuUI;
		_prefabFactory = prefabFactory;
		_currencyManagerController = currencyManagerController;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		_bundleOfferCatalog = bundleOfferCatalog;
		_openRequest = openRequest;
		_catalog = emoteCatalog;
	}

	protected override void Awake()
	{
		base.Awake();
		_parkingMenuUI = _parkingMenuUIDep?.Value;
		_turnOffAction = TurnOffAnimation;
	}

	protected override void Start()
	{
		base.Start();
		InitStart();
		_delayedReset?.Dispose();
		if (!FlagsStorage.IsMainMenu)
		{
			OpenShop();
			_delayedReset = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				InitBackButton();
			}).AddTo(this);
		}
	}

	public override void ReloadWindow()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		CloseShop();
		if (FlagsStorage.IsMainMenu)
		{
			_managerUI.ShowMainMenuWindow();
		}
		else
		{
			_managerUI.CloseSeveralWindows();
		}
		Close();
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			if ((bool)_newMoveController)
			{
				_newMoveController.Input.Emotion = 0;
			}
			base.Close();
		}
	}

	private new void OnDestroy()
	{
		StopCurrentPreview();
	}

	private void InitStart()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		shopUi.SetActive(value: false);
		buyButton.SetActive(value: false);
		EquipButton.SetActive(active: false);
		if ((bool)setButtonRoot)
		{
			setButtonRoot.SetActive(value: false);
		}
		if (HasTabs)
		{
			AnimationShopTabCategory[] tabCategories = TabCategories;
			foreach (AnimationShopTabCategory obj in tabCategories)
			{
				obj.Init();
				obj.OnSelectedTab += SelectTab;
			}
		}
	}

	public static bool IsAnimationOwned(int y)
	{
		if (y > 4)
		{
			return PlayerPrefs.GetInt("animationBought" + y, 0) == 1;
		}
		return true;
	}

	public static void SetAnimationOwned(int y, bool owned)
	{
		PlayerPrefs.SetInt("animationBought" + y, owned ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static long GetBoughtAnimationsByPrefs()
	{
		List<int> list = new List<int>();
		int num = 64;
		for (int i = 0; i < num; i++)
		{
			int item = PlayerPrefs.GetInt("animationBought" + i, 0);
			list.Add(item);
		}
		return BitMaskToListConverter.GetBitMaskFromList(list);
	}

	public static List<int> GetSelectedAnimationsByPrefs()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < 8; i++)
		{
			int item = PlayerPrefs.GetInt("currentAnimation_" + i, i + 1);
			list.Add(item);
		}
		return list;
	}

	public static void SetSelectedAnimationsByPrefs(List<int> selectedAnimations)
	{
		if (selectedAnimations.Count == 0)
		{
			Debug.LogError("SetSelectedAnimation: selectedAnimations.Count == 0");
			return;
		}
		for (int i = 0; i < selectedAnimations.Count; i++)
		{
			int value = selectedAnimations[i];
			PlayerPrefs.SetInt("currentAnimation_" + i, value);
		}
	}

	public static void SetBoughtAnimationsByPrefs(long boughtAnimationsNumber)
	{
		List<int> listFromBitMask = BitMaskToListConverter.GetListFromBitMask(boughtAnimationsNumber);
		for (int i = 0; i < listFromBitMask.Count; i++)
		{
			int num = listFromBitMask[i];
			if (num != 0)
			{
				PlayerPrefs.SetInt("animationBought" + i, num);
			}
		}
	}

	public void OpenShop(AnimationShopSection section)
	{
		_currentSection = section;
		OpenShop();
	}

	private void OpenShop()
	{
		InitStart();
		ResetPreviewToIdle();
		_pendingBundleEntry = null;
		buyButton.SetActive(value: false);
		shopUi.SetActive(value: true);
		DeselectAllItems();
		ResolveOpeningSectionAndTab();
		if (FlagsStorage.IsMainMenu)
		{
			OpenShopMainMenu();
		}
		else
		{
			OpenShopFreeDrive();
		}
		TryScrollToPendingBundle();
	}

	private void ResolveOpeningSectionAndTab()
	{
		AnimationShopTab tab = AnimationShopTab.Animations;
		int focusBundleId = -1;
		bool flag = _openRequest != null && _openRequest.TryConsume(out tab, out focusBundleId);
		_pendingFocusBundleId = (flag ? focusBundleId : (-1));
		if (flag)
		{
			AnimationShopSection currentSection = AnimationShopRouting.SectionFor(tab);
			_ = _currentSection;
			_currentSection = currentSection;
			if (tab == AnimationShopTab.Animations && focusBundleId >= 0)
			{
				tab = AnimationShopTab.DanceAnimations;
			}
		}
		_currentTab = (((HasTabs & flag) && tab != AnimationShopTab.Poses) ? tab : AnimationShopTab.Animations);
		ApplyTabRowVisibility();
		if (HasTabs)
		{
			AnimationShopTabCategory[] tabCategories = TabCategories;
			foreach (AnimationShopTabCategory obj in tabCategories)
			{
				obj.SetSelectedSilent(obj.Tab == _currentTab);
			}
		}
	}

	private void ApplyTabRowVisibility()
	{
		bool active = _currentSection == AnimationShopSection.Animations;
		if ((bool)TabRowRoot)
		{
			TabRowRoot.SetActive(active);
		}
		else
		{
			if (!HasTabs)
			{
				return;
			}
			AnimationShopTabCategory[] tabCategories = TabCategories;
			foreach (AnimationShopTabCategory animationShopTabCategory in tabCategories)
			{
				if ((bool)animationShopTabCategory)
				{
					animationShopTabCategory.gameObject.SetActive(active);
				}
			}
		}
	}

	private void SelectTab(AnimationShopTab tab)
	{
		if (_currentSection == AnimationShopSection.Animations)
		{
			_currentTab = tab;
			_turnOffAnimationDisposable?.Dispose();
			StopCurrentPreview();
			_pendingBundleEntry = null;
			buyButton.SetActive(value: false);
			DeselectAllItems();
			CreateItems();
		}
	}

	private void DeselectAllItems()
	{
		foreach (AnimationShopItem cachedAnimationShopItem in _cachedAnimationShopItems)
		{
			cachedAnimationShopItem.Toggle.SetIsOnWithoutNotify(value: false);
			cachedAnimationShopItem.OnPressed(state: false);
		}
		foreach (AnimationShopItem cachedPoseShopItem in _cachedPoseShopItems)
		{
			cachedPoseShopItem.Toggle.SetIsOnWithoutNotify(value: false);
			cachedPoseShopItem.OnPressed(state: false);
		}
		foreach (AnimationShopItem cachedDanceShopItem in _cachedDanceShopItems)
		{
			cachedDanceShopItem.Toggle.SetIsOnWithoutNotify(value: false);
			cachedDanceShopItem.OnPressed(state: false);
		}
	}

	public void ResetPreviewToIdle()
	{
		AnimationShopItem animationShopItem = currentAnimationItem;
		_turnOffAnimationDisposable?.Dispose();
		StopCurrentPreview();
		if ((bool)animationShopItem)
		{
			animationShopItem.Toggle.SetIsOnWithoutNotify(value: false);
			animationShopItem.OnPressed(state: false);
		}
	}

	private EmotePreviewContext PreviewContext()
	{
		bool driverRagdolled = !FlagsStorage.IsMainMenu && RagdollCPM.IsDriverRagdolled();
		return new EmotePreviewContext(_previewPersonAnimationsController, _previewWic, _newMoveController, FlagsStorage.IsMainMenu, FlagsStorage.IsMultiFree, driverRagdolled, _turnOffAction);
	}

	private void StopCurrentPreview()
	{
		if ((bool)currentAnimationItem && _catalog != null)
		{
			_catalog.GetById(currentAnimationItem.id)?.StopPreview(PreviewContext());
		}
		currentAnimationItem = null;
		if ((bool)_previewPersonAnimationsController)
		{
			_previewPersonAnimationsController.StopSpecialAnimation();
		}
		if ((bool)_newMoveController)
		{
			_newMoveController.Input.Emotion = 0;
		}
		if ((bool)previewPerson)
		{
			previewPerson.transform.rotation = previewPersonDefaultRotation;
		}
	}

	public void CloseShop()
	{
		_turnOffAnimationDisposable?.Dispose();
		TurnOffAnimation();
		shopUi.SetActive(value: false);
		buyButton.SetActive(value: false);
		if (FlagsStorage.IsMainMenu)
		{
			CloseShopMainMenu();
		}
		else
		{
			CloseShopFreeDrive();
		}
	}

	public void ItemPressed(AnimationShopItem item)
	{
		IEmoteEntry byId = _catalog.GetById(item.id);
		if (byId == null)
		{
			return;
		}
		bool flag = byId.BuyFlow == EmoteBuyFlow.Coins && byId.Price == 0;
		bool flag2 = byId.IsOwned | flag;
		ShowBuyButtons(byId.BuyFlow == EmoteBuyFlow.Coins && !flag2, item.price);
		if ((bool)currentAnimationItem && (bool)_previewPersonAnimationsController && _previewPersonAnimationsController.CurrentSpecialAnimationId == item.id)
		{
			return;
		}
		EmotePreviewContext ctx = PreviewContext();
		if ((bool)currentAnimationItem)
		{
			_catalog.GetById(currentAnimationItem.id)?.StopPreview(in ctx);
		}
		currentAnimationItem = item;
		byId.Preview(in ctx);
		if (!ctx.DriverRagdolled && (bool)previewPerson)
		{
			previewPerson.transform.rotation = previewPersonDefaultRotation;
		}
		_turnOffAnimationDisposable?.Dispose();
		float? previewAutoStopSeconds = byId.PreviewAutoStopSeconds;
		if (previewAutoStopSeconds.HasValue)
		{
			float valueOrDefault = previewAutoStopSeconds.GetValueOrDefault();
			_turnOffAnimationDisposable = Observable.Timer(TimeSpan.FromSeconds(valueOrDefault)).Subscribe((long _) =>
			{
				TurnOffAnimation();
			}).AddTo(this);
		}
		UpdateLockedBundleIntent(byId, flag2);
	}

	private bool TryGetActiveBundleOfferForEntry(IEmoteEntry entry, out OfferModel offer)
	{
		if (entry.ShopTab == AnimationShopTab.Poses)
		{
			return _bundleOfferCatalog.TryGetActiveOfferForIkBundle(entry.BundleId, out offer);
		}
		return _bundleOfferCatalog.TryGetActiveOfferForAnimationBundle(entry.BundleId, out offer);
	}

	private void UpdateLockedBundleIntent(IEmoteEntry entry, bool owned)
	{
		_pendingBundleEntry = null;
		if (!((entry.BuyFlow != EmoteBuyFlow.Bundle) | owned))
		{
			if (entry.BundleId < 0 || _bundleOfferCatalog == null || !TryGetActiveBundleOfferForEntry(entry, out var _))
			{
				ShowBundleUnavailableMessage();
				return;
			}
			_pendingBundleEntry = entry;
			ShowBuyButtons(toShow: true, 0);
			SetBuyButtonBundleMode(bundleMode: true);
			LayoutRebuilder.ForceRebuildLayoutImmediate(buyButton.GetComponent<RectTransform>());
		}
	}

	private void ShowBundleUnavailableMessage()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("SuitPack.PackNotAvailable"), 2f).Forget();
	}

	private void BuyLockedEntryViaBundle()
	{
		IEmoteEntry pendingBundleEntry = _pendingBundleEntry;
		if (pendingBundleEntry == null)
		{
			return;
		}
		if (!TryGetActiveBundleOfferForEntry(pendingBundleEntry, out var offer))
		{
			ShowBundleUnavailableMessage();
			ShowBuyButtons(toShow: false, 0);
			return;
		}
		_pendingBundleEntry = null;
		if (FlagsStorage.IsMainMenu)
		{
			if ((bool)PersonWindow)
			{
				PersonWindow.CloseToBundleOffer(offer);
			}
		}
		else
		{
			CloseShop();
			_managerUI.CloseSeveralWindows();
			Close();
			_ = _managerUI.ShowBundleOfferWindow(offer);
		}
	}

	public void BuyButtonPressed()
	{
		if (!currentAnimationItem)
		{
			return;
		}
		IEmoteEntry byId = _catalog.GetById(currentAnimationItem.id);
		if (byId == null)
		{
			return;
		}
		if (byId.BuyFlow == EmoteBuyFlow.Bundle)
		{
			BuyLockedEntryViaBundle();
			return;
		}
		buyButton.SetActive(value: false);
		if (currentAnimationItem.price > 0)
		{
			LogAnalyticEvent(PurchaseStatus.PURCHASE_CLICKED);
			if ((bool)PersonWindow)
			{
				PersonWindow.canvasGroup.alpha = 0f;
			}
			_ = _managerUI.ShowConfirmWindow(() =>
			{
				TryBuy().Forget();
			}, new LocalizedString("Playerprofilewindow.Message"), string.Format(new LocalizedString("VisualTuningWindow.Buythisitemfor"), currentAnimationItem.price), () =>
			{
				if ((bool)PersonWindow)
				{
					PersonWindow.canvasGroup.alpha = 1f;
				}
				LogAnalyticEvent(PurchaseStatus.PURCHASE_CANCELED_BY_USER);
			});
		}
		else
		{
			TryBuy().Forget();
		}
	}

	private async UniTaskVoid TryBuy()
	{
		if ((bool)PersonWindow)
		{
			PersonWindow.canvasGroup.alpha = 1f;
		}
		AdvancedInt price = new AdvancedInt(currentAnimationItem.price);
		LogAnalyticEvent(PurchaseStatus.PURCHASE_CONFIRMED);
		await _currencyManagerController.TryCoinsSpend(SpendCoinsType.Animations, price, () =>
		{
			_ = _managerUI.ShowNotificationWindow(new LocalizedString("AnimationShop.Success"), 2f);
			PlayerPrefs.SetInt("animationBought" + currentAnimationItem.id, 1);
			CreateItems();
			LogAnalyticEvent(PurchaseStatus.PURCHASE_COMPLETED);
		}, () =>
		{
			LogAnalyticEvent(PurchaseStatus.INSUFFICIENT_FUNDS);
		});
	}

	private void OpenShopMainMenu()
	{
		_currentGender = PersonWindow.mainPerson.Gender;
		_previewPersonAnimationsController = PersonWindow.mainPerson.PersonAnimationsController;
		previewPerson = _previewPersonAnimationsController.gameObject;
		_previewWic = previewPerson.GetComponentInParent<WorldInteractionController>();
		Rigidbody rigidbody = previewPerson.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = previewPerson.AddComponent<Rigidbody>();
		}
		rigidbody.constraints = RigidbodyConstraints.FreezePosition;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		previewPersonDefaultRotation = previewPerson.transform.rotation;
		CreateItems();
	}

	private void OpenShopFreeDrive()
	{
		MainPerson component = V.DriverPerson.GetComponent<MainPerson>();
		_currentGender = component.Gender;
		_newMoveController = V.DriverPerson.GetComponent<NewMoveController>();
		_previewPersonAnimationsController = component.PersonAnimationsController;
		previewPerson = _previewPersonAnimationsController.gameObject;
		_previewWic = previewPerson.GetComponentInParent<WorldInteractionController>();
		CreateItems();
		if ((bool)_parkingMenuUI)
		{
			_parkingMenuUI.Hide();
		}
		_levelsProvider.LevelScript.CleanScreen(canMute: false);
		Time.timeScale = 1f;
		_previewPersonAnimationsController.StopSpecialAnimation();
	}

	private void CloseShopMainMenu()
	{
		PersonWindow.OnSelectCategory(0);
	}

	private void CloseShopFreeDrive()
	{
		if ((bool)_parkingMenuUI)
		{
			_parkingMenuUI.Show();
		}
		_levelsProvider.LevelScript.UnCleanScreen();
	}

	private void ShowBuyButtons(bool toShow, int price)
	{
		SetBuyButtonBundleMode(bundleMode: false);
		buyButton.SetActive(toShow);
		TextPrice.text = price.ToString();
		LayoutRebuilder.ForceRebuildLayoutImmediate(buyButton.GetComponent<RectTransform>());
	}

	private void SetBuyButtonBundleMode(bool bundleMode)
	{
		if ((bool)BuyForLabel)
		{
			BuyForLabel.SetActive(!bundleMode);
		}
		if ((bool)CoinIcon)
		{
			CoinIcon.SetActive(!bundleMode);
		}
		if (bundleMode)
		{
			TextPrice.text = new LocalizedString("Selectapartmentwindow.Buy");
		}
	}

	private void ClonePerson()
	{
		if (previewPerson == null)
		{
			Transform parent = _freeDriveProvider.MapCamera.transform.parent.transform;
			previewPerson = UnityEngine.Object.Instantiate(V.DriverPerson, parent, worldPositionStays: false);
			_previewPersonAnimationsController = previewPerson.GetComponent<DriverActionController>().PersonAnimationsController;
			_previewPersonAnimationsController.Animator.applyRootMotion = false;
			SetLayerAndMaterialRecursively(previewPerson, LayerMask.NameToLayer("TransparentFX"));
			previewCamera.transform.parent = parent;
			previewCamera.allowHDR = false;
			_previewPersonAnimationsController.SetBoolIsMoving(value: false);
			_previewPersonAnimationsController.StopMoveAnimation();
			Rigidbody component = previewPerson.GetComponent<Rigidbody>();
			component.constraints = RigidbodyConstraints.FreezePosition;
			component.useGravity = false;
			component.isKinematic = true;
			component.detectCollisions = false;
			SkinnedMeshRenderer skinnedMeshRender = previewPerson.GetComponent<MainPerson>().SkinnedMeshRender;
			skinnedMeshRender.receiveShadows = false;
			skinnedMeshRender.shadowCastingMode = ShadowCastingMode.Off;
			UnityEngine.Object.Destroy(previewPerson.GetComponent<DriverActionController>().MapPersonIcon);
			UnityEngine.Object.Destroy(previewPerson.GetComponent<DriverActionController>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<PhotonView>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<MultiPersonControl>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<NewMoveController>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<TouchMovePerson>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<Emoji>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<BannerManager>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<MyTreasureManager>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<TaxiController>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<DeliveryController>());
			UnityEngine.Object.Destroy(previewPerson.GetComponent<CargoController>());
		}
		Vector3 localPosition = V.DriverPerson.transform.localPosition;
		localPosition.y = 500f;
		previewPerson.transform.localPosition = localPosition;
		localPosition.z += 2.5f;
		localPosition.y = 501.5f;
		previewCamera.transform.localPosition = localPosition;
		previewCamera.transform.LookAt(previewPerson.transform);
		previewCamera.transform.Rotate(new Vector3(-15f, 0f, 0f));
		previewPerson.transform.rotation = previewPersonDefaultRotation;
		if (previewRenderTexture == null)
		{
			previewRenderTexture = new RenderTexture(700, 1000, 16, RenderTextureFormat.ARGB32);
			previewCamera.targetTexture = previewRenderTexture;
			previewTexture.texture = previewRenderTexture;
		}
		_previewPersonAnimationsController.StopSpecialAnimation();
	}

	private void TurnOffAnimation()
	{
		if ((bool)currentAnimationItem)
		{
			IEmoteEntry byId = _catalog.GetById(currentAnimationItem.id);
			if (byId != null)
			{
				if (byId.KeepsRunningOnTurnOff)
				{
					return;
				}
				byId.StopPreview(PreviewContext());
			}
		}
		if ((bool)_previewPersonAnimationsController)
		{
			_previewPersonAnimationsController.StopSpecialAnimation();
		}
		if ((bool)_newMoveController)
		{
			_newMoveController.Input.Emotion = 0;
		}
		if ((bool)previewPerson)
		{
			previewPerson.transform.rotation = previewPersonDefaultRotation;
		}
	}

	private void SetLayerAndMaterialRecursively(GameObject obj, int newLayer)
	{
		if (obj == null)
		{
			return;
		}
		obj.layer = newLayer;
		CheckTheMaterial(obj);
		foreach (Transform item in obj.transform)
		{
			if (!(null == item))
			{
				SetLayerAndMaterialRecursively(item.gameObject, newLayer);
			}
		}
	}

	private static void CheckTheMaterial(GameObject obj)
	{
		if (obj.TryGetComponent<SkinnedMeshRenderer>(out var component))
		{
			Material material = component.material;
			if (material.shader.name == "Legacy Shaders/Transparent/Cutout/Bumped Diffuse")
			{
				Material material2 = new Material(material);
				material2.name = component.name + " _ material";
				Shader shader = Shader.Find("Custom/DiffuseCutoff");
				Debug.Log("shader was changed to " + shader);
				Debug.Log("obj " + obj.name);
				material2.shader = shader;
				component.material = material2;
				material2.SetColor("_MainColor", new Color(0.75f, 0.75f, 0.75f, 1f));
			}
		}
	}

	private void CreateItems()
	{
		foreach (AnimationShopItem cachedAnimationShopItem in _cachedAnimationShopItems)
		{
			cachedAnimationShopItem.gameObject.SetActive(value: false);
		}
		foreach (AnimationShopItem cachedPoseShopItem in _cachedPoseShopItems)
		{
			cachedPoseShopItem.gameObject.SetActive(value: false);
		}
		foreach (AnimationShopItem cachedDanceShopItem in _cachedDanceShopItems)
		{
			cachedDanceShopItem.gameObject.SetActive(value: false);
		}
		if (_catalog == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (IEmoteEntry item in _catalog.GridOrder)
		{
			if (item.Id == 0)
			{
				continue;
			}
			bool flag = !PoseAudienceRules.Matches(item.Audience, _currentGender);
			if (flag && item.GenderMismatchPolicy == GenderMismatchPolicy.Hide)
			{
				continue;
			}
			bool flag2 = item.ShopTab == AnimationShopTab.Poses;
			if (flag2 != (_currentSection == AnimationShopSection.Poses))
			{
				continue;
			}
			bool flag3 = !flag2 && item.BuyFlow == EmoteBuyFlow.Bundle;
			if (!flag2 && HasTabs && flag3 != (_currentTab == AnimationShopTab.DanceAnimations))
			{
				continue;
			}
			List<AnimationShopItem> list = (flag2 ? _cachedPoseShopItems : (flag3 ? _cachedDanceShopItems : _cachedAnimationShopItems));
			int num4 = (flag2 ? num2++ : (flag3 ? num3++ : num++));
			AnimationShopItem animationShopItem;
			if (list.Count > num4)
			{
				animationShopItem = list[num4];
			}
			else
			{
				AnimationShopItem movesPackItemPrefab = itemPrefab;
				if ((flag2 | flag3) && (bool)MovesPackItemPrefab)
				{
					movesPackItemPrefab = MovesPackItemPrefab;
				}
				animationShopItem = _prefabFactory.Instantiate(movesPackItemPrefab, itemsParent.transform);
				animationShopItem.Toggle.group = ToggleGroup;
				list.Add(animationShopItem);
			}
			animationShopItem.Init(item, this, flag);
			animationShopItem.gameObject.SetActive(value: true);
			animationShopItem.transform.SetAsLastSibling();
		}
	}

	public void RefreshShopItems()
	{
		CreateItems();
	}

	private void TryScrollToPendingBundle()
	{
		int pendingFocusBundleId = _pendingFocusBundleId;
		_pendingFocusBundleId = -1;
		if (pendingFocusBundleId < 0 || !ItemsScrollRect)
		{
			return;
		}
		AnimationShopItem target = FindFirstVisibleBundleItem(pendingFocusBundleId);
		if (!target)
		{
			return;
		}
		Observable.TimerFrame(1).Subscribe((long _) =>
		{
			if (shopUi.activeInHierarchy && (bool)target && target.gameObject.activeInHierarchy)
			{
				SnapScrollToItem((RectTransform)target.transform);
			}
		}).AddTo(this);
	}

	private AnimationShopItem FindFirstVisibleBundleItem(int bundleId)
	{
		if (_currentSection == AnimationShopSection.Poses)
		{
			return FindFirstVisibleBundleItem(_cachedPoseShopItems, bundleId);
		}
		AnimationShopItem animationShopItem = FindFirstVisibleBundleItem(_cachedDanceShopItems, bundleId);
		if (!animationShopItem)
		{
			return FindFirstVisibleBundleItem(_cachedAnimationShopItems, bundleId);
		}
		return animationShopItem;
	}

	private AnimationShopItem FindFirstVisibleBundleItem(List<AnimationShopItem> cache, int bundleId)
	{
		foreach (AnimationShopItem item in cache)
		{
			if (item.gameObject.activeSelf)
			{
				IEmoteEntry byId = _catalog.GetById(item.id);
				if (byId != null && byId.BundleId == bundleId)
				{
					return item;
				}
			}
		}
		return null;
	}

	private void SnapScrollToItem(RectTransform item)
	{
		RectTransform content = ItemsScrollRect.content;
		RectTransform rectTransform = (ItemsScrollRect.viewport ? ItemsScrollRect.viewport : ((RectTransform)ItemsScrollRect.transform));
		LayoutRebuilder.ForceRebuildLayoutImmediate(content);
		Vector2 vector = content.InverseTransformPoint(item.TransformPoint(item.rect.center));
		if (ItemsScrollRect.vertical && content.rect.height > rectTransform.rect.height)
		{
			float num = content.rect.height - rectTransform.rect.height;
			float num2 = content.rect.yMax - vector.y - rectTransform.rect.height * 0.5f;
			ItemsScrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp01(num2 / num);
		}
		if (ItemsScrollRect.horizontal && content.rect.width > rectTransform.rect.width)
		{
			float num3 = content.rect.width - rectTransform.rect.width;
			float num4 = vector.x - content.rect.xMin - rectTransform.rect.width * 0.5f;
			ItemsScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(num4 / num3);
		}
		ItemsScrollRect.velocity = Vector2.zero;
	}

	public void LogAnalyticEvent(PurchaseStatus purchaseStatus)
	{
		_virtualPurchaseAnalytic.LogPurchaseVirtual_PersonCustomization(-1, currentAnimationItem.price, currentAnimationItem.id, purchaseStatus);
	}
}
