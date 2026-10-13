using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using CPM.PlayerData;
using CPM.Rating;
using CPM.Rating.Entities;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.CPMRating.AchievementsService;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

public class AchievementsWindow : WindowScript, IBackableWindow
{
	private const int DelayToNextAchievement = 4;

	public Action OnAchievementsWindowClosed;

	[Header("Account")]
	[SerializeField]
	private Text NicknameText;

	[SerializeField]
	private TextMeshProUGUI IdText;

	[SerializeField]
	private TextMeshProUGUI EmailText;

	[SerializeField]
	private TextMeshProUGUI labelName;

	[SerializeField]
	private TextMeshProUGUI labelLevel;

	[SerializeField]
	private TextMeshProUGUI labelRank;

	[SerializeField]
	private TextMeshProUGUI labelID;

	[SerializeField]
	private TextMeshProUGUI labelGameTime;

	[SerializeField]
	private TextMeshProUGUI labelEXP;

	[SerializeField]
	private Slider sliderEXP;

	[SerializeField]
	private TextMeshProUGUI labelAllAchieveCount;

	[SerializeField]
	private RawImage textureRankIcon;

	[SerializeField]
	private SmoothFill fillGameTime;

	[SerializeField]
	private SmoothFill fillDistance;

	[SerializeField]
	private SmoothFill fillDragWins;

	[SerializeField]
	private SmoothFill fillLevelDone;

	[SerializeField]
	private SmoothFill overallAchievementsFill;

	[SerializeField]
	private Slider sliderRun;

	[SerializeField]
	private Slider sliderSpeedBunner;

	[SerializeField]
	private Slider sliderGifts;

	[SerializeField]
	private Slider sliderTreasure;

	[SerializeField]
	private Slider sliderCars;

	[SerializeField]
	private Slider sliderDrift;

	[SerializeField]
	private Slider sliderPolice;

	[SerializeField]
	private Slider sliderBlockPost;

	[SerializeField]
	private Slider sliderRealEstate;

	[SerializeField]
	private Slider sliderFuel;

	[SerializeField]
	private Slider sliderTrade;

	[SerializeField]
	private Slider sliderExchange;

	[SerializeField]
	private Slider sliderBurntTyre;

	[SerializeField]
	private Slider sliderPassangerDistance;

	[SerializeField]
	private Slider sliderReactions;

	[SerializeField]
	private Slider sliderCarFix;

	[SerializeField]
	private Slider sliderCarWash;

	[SerializeField]
	private Slider sliderOffroad;

	[SerializeField]
	private Slider sliderDriftMax;

	[SerializeField]
	private Slider sliderTaxi;

	[SerializeField]
	private Slider sliderDelivery;

	[SerializeField]
	private Slider sliderCargo;

	[SerializeField]
	private CanvasGroup panelMain;

	[SerializeField]
	private GameObject loadingSign;

	[SerializeField]
	private RectTransform parentAchievements;

	[SerializeField]
	private achievmentItemUI oneItemPrefab;

	[SerializeField]
	private List<AchievementItemData> allAchievementList;

	[SerializeField]
	private int countChild;

	[SerializeField]
	private int countAllAchievments;

	[SerializeField]
	private Scrollbar bar;

	[SerializeField]
	private Slider achievementListBar;

	[SerializeField]
	private GameObject SignInWindowRequired;

	[SerializeField]
	private GameObject bottomWindow;

	[SerializeField]
	private TextMeshProUGUI targetRankText;

	[SerializeField]
	private RawImage achievDetailsBackground;

	[SerializeField]
	private RawImage achievDetailsMain;

	[SerializeField]
	private TextMeshProUGUI achievDetailsText;

	[SerializeField]
	private TextMeshProUGUI achievDetailsTitle;

	[SerializeField]
	private Button ManageAccountButton;

	[SerializeField]
	private Button CloseSelectedButton;

	[SerializeField]
	private Button BackButton;

	[SerializeField]
	private GameObject AchievmentsGridPanel;

	[SerializeField]
	private GameObject SelectedAchievmentPanel;

	[SerializeField]
	private GameObject NoInternetConnectionPanel;

	private bool _isDestroyed;

	private int windowOpenCount;

	private CancellationTokenSource _cts;

	private PlayerDataStorage playerDataStorage;

	private RatingDataCollection ratingDataCollection;

	private IPrefabFactory _prefabFactory;

	private IPlayerStorageService _playerStorageService;

	private IAccountController _accountController;

	private FreeDriveProvider _freeDriveProvider;

	private IOfflineModeSystem _offlineModeSystem;

	private IAchievementsService _achievementsService;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider, RatingDataCollection ratingDataCollection, IPrefabFactory prefabFactory, IPlayerStorageService playerStorageService, IAccountController accountController, IOfflineModeSystem offlineModeSystem, IAchievementsService achievementsService)
	{
		_freeDriveProvider = freeDriveProvider;
		this.playerDataStorage = playerDataStorage;
		this.ratingDataCollection = ratingDataCollection;
		_prefabFactory = prefabFactory;
		_playerStorageService = playerStorageService;
		_accountController = accountController;
		_offlineModeSystem = offlineModeSystem;
		_achievementsService = achievementsService;
	}

	public void InitBackButton()
	{
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		Close();
	}

	protected override void OnEnable()
	{
		BackButton.gameObject.SetActive(value: false);
		base.OnEnable();
	}

	private void ShowAutorizationWindow()
	{
		_managerUI.ShowAchievmentsAuthorizationWindow();
		base.Close();
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
			OnAchievementsWindowClosed?.Invoke();
			_managerUI.OnAchievmentsButtonActivated?.Invoke(obj: true);
			_managerUI.OnSettingsButtonActivated?.Invoke(obj: true);
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ShowMainMenuWindow();
			}
			base.Close();
		}
	}

	public void onRankReceived()
	{
		Texture2D texture2D = UtilsRank.LoadMyRankTextureFromResources();
		if (!texture2D)
		{
			texture2D = UtilsRank.LoadRankTextureFromResources(UtilsRank.RankType.Beginner);
		}
		textureRankIcon.texture = texture2D;
		textureRankIcon.gameObject.SetActive(value: true);
		GameOverlayWindow.SetRank(texture2D, UtilsRank.GetMyRankName());
		labelRank.text = string.Concat(new LocalizedString("achievmentItemUI.Rank"), UtilsRank.GetMyRankName());
		if ((bool)V.CoreGameManager.GarageNameLabel)
		{
			V.CoreGameManager.GarageNameLabel.text = playerDataStorage.PlayerName + " is " + UtilsRank.GetMyRankName();
		}
	}

	public override void ReloadWindow()
	{
	}

	protected override void Start()
	{
		InitBackButton();
		_managerUI.OnAchievmentsButtonActivated?.Invoke(obj: false);
		_managerUI.OnSettingsButtonActivated?.Invoke(obj: false);
		_managerUI.AchievementsWindow = this;
		if (ScenesHelper.IsMenuSceneActive())
		{
			OnAchievementsWindowOpen();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		ManageAccountButton.onClick.AddListener(ShowAutorizationWindow);
		CloseSelectedButton.onClick.AddListener(HideDetailDescription);
		bool active = ScenesHelper.IsMenuSceneActive();
		ManageAccountButton.gameObject.SetActive(active);
		HideDetailDescription();
		NoInternetConnectionPanel.SetActive(_offlineModeSystem.Enabled);
		base.Start();
	}

	public void OnAchievementsWindowOpen()
	{
		allAchievementList = new List<AchievementItemData>();
		labelName.text = playerDataStorage.PlayerName;
		labelID.text = $"ID: {playerDataStorage.PlayerID}";
		NicknameText.text = _playerStorageService.Name;
		IdText.text = _playerStorageService.LocalID;
		EmailText.text = _accountController.Email;
		if (AccountSystemV2Extension.IsGuestAccount(_accountController.Email))
		{
			EmailText.transform.parent.gameObject.SetActive(value: false);
		}
		setZeroAll();
		if (_accountController.HasEmail || ManagerKeysForPrefs.IsSteamLinked)
		{
			int ownRank = PlayersListControl.GetOwnRank();
			RequestPlayerRating(playerDataStorage.AccountID, ownRank).Forget();
			bottomWindow.SetActive(value: true);
			SignInWindowRequired.SetActive(value: false);
		}
		else
		{
			loadingSign.gameObject.SetActive(value: false);
			SignInWindowRequired.SetActive(value: true);
			bottomWindow.SetActive(value: false);
		}
	}

	public async UniTaskVoid OnAchievementsWindowOpenFreeDrive(OnePlayerInListData onePlayerInListData, bool fromPlayerList)
	{
		if (_isDestroyed)
		{
			return;
		}
		labelName.text = onePlayerInListData.PlayerName;
		NicknameText.text = onePlayerInListData.PlayerName;
		labelID.text = "ID: " + onePlayerInListData.PlayerId;
		allAchievementList = new List<AchievementItemData>();
		EmailText.transform.parent.gameObject.SetActive(value: false);
		if (!onePlayerInListData.Person)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("AchievementsWindow.Playerleft!"), 3f).Forget();
			Back();
			return;
		}
		MultiPersonControl multiPersonControl = onePlayerInListData.Person.GetComponent<MultiPersonControl>();
		while (!multiPersonControl.rank.HasValue)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
		}
		if (!_isDestroyed)
		{
			int? num = multiPersonControl.rank;
			Texture texture = UtilsRank.LoadRankTextureFromResources((UtilsRank.RankType)num.Value);
			textureRankIcon.texture = texture;
			textureRankIcon.gameObject.SetActive(value: true);
			labelRank.text = Utils.GetRankNameByIndex(num.GetValueOrDefault());
			if (onePlayerInListData.AccountId == (ObscuredString)"")
			{
				Back();
				return;
			}
			base.gameObject.SetActive(value: true);
			setZeroAll();
			int valueOrDefault = num.GetValueOrDefault();
			RequestPlayerRating(onePlayerInListData.AccountId, valueOrDefault).Forget();
			bottomWindow.SetActive(value: true);
			SignInWindowRequired.SetActive(value: false);
		}
	}

	private void setZeroAll()
	{
		panelMain.alpha = 0f;
		fillDistance.SetValue(0f, 0 + Utils.CurrentMetric);
		fillGameTime.SetValue(0f, 0 + Utils.CurrentMetric);
		fillDragWins.SetValue(0f, 0 + Utils.CurrentMetric);
		fillLevelDone.SetValue(0f, "0");
		sliderRun.value = 0f;
		sliderSpeedBunner.value = 0f;
		sliderTreasure.value = 0f;
		sliderDrift.value = 0f;
		sliderCars.value = 0f;
		sliderGifts.value = 0f;
		sliderPassangerDistance.value = 0f;
		sliderPolice.value = 0f;
		sliderBlockPost.value = 0f;
		sliderRealEstate.value = 0f;
		sliderFuel.value = 0f;
		sliderTrade.value = 0f;
		sliderBurntTyre.value = 0f;
		sliderExchange.value = 0f;
		sliderCarFix.value = 0f;
		sliderCarWash.value = 0f;
		sliderOffroad.value = 0f;
		sliderReactions.value = 0f;
		sliderDriftMax.value = 0f;
		sliderTaxi.value = 0f;
		if ((bool)sliderCargo)
		{
			sliderDelivery.value = 0f;
			sliderCargo.value = 0f;
		}
		sliderEXP.value = 0f;
		labelEXP.text = "";
		labelGameTime.text = string.Concat(0.ToString(), new LocalizedString("AchievementsWindow.H"), 0.ToString(), new LocalizedString("AchievementsWindow.Min"));
		labelAllAchieveCount.text = "0";
		foreach (Transform parentAchievement in parentAchievements)
		{
			if ((bool)parentAchievement)
			{
				UnityEngine.Object.Destroy(parentAchievement.gameObject);
			}
		}
		bar.gameObject.SetActive(value: false);
		fillGameTime.gameObject.SetActive(value: false);
		fillDistance.gameObject.SetActive(value: false);
		fillDragWins.gameObject.SetActive(value: false);
		fillLevelDone.gameObject.SetActive(value: false);
		overallAchievementsFill.gameObject.SetActive(value: false);
	}

	private async UniTaskVoid RequestPlayerRating(string uid, int rank)
	{
		loadingSign.SetActive(value: true);
		await _freeDriveProvider.FreeDriveDB.SignIN();
		if (_isDestroyed)
		{
			return;
		}
		RatingResultAll ratingResultAll = ((!((ObscuredString)uid == playerDataStorage.AccountID)) ? (await _freeDriveProvider.FreeDriveDB.GetUserRating(uid, base.destroyCancellationToken)) : (await _achievementsService.GetUserRating(base.destroyCancellationToken)));
		if (!_isDestroyed && ratingResultAll != null)
		{
			FillUI(ratingResultAll);
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			if ((bool)this)
			{
				LoadAchievements(ratingResultAll, rank, _cts.Token).Forget();
			}
			if ((bool)this && base.enabled)
			{
				SetAlhaPanelAsync(_cts.Token).Forget();
			}
		}
	}

	public void FillUI(RatingResultAll userRating)
	{
		if (userRating != null)
		{
			float valueOrDefault = (userRating.general?.t_distance).GetValueOrDefault();
			float valueOrDefault2 = (userRating.general?.time).GetValueOrDefault();
			int num = (int)valueOrDefault2 / 60;
			int num2 = (int)valueOrDefault2 % 60;
			float valueOrDefault3 = ((float?)userRating.general?.race_win).GetValueOrDefault();
			float valueOrDefault4 = ((float?)userRating.general?.levels).GetValueOrDefault();
			float valueOrDefault5 = (userRating.general?.run).GetValueOrDefault();
			float valueOrDefault6 = ((float?)userRating.general?.speed_banner).GetValueOrDefault();
			float valueOrDefault7 = ((float?)userRating.general?.treasure).GetValueOrDefault();
			float valueOrDefault8 = ((float?)userRating.general?.cars).GetValueOrDefault();
			float valueOrDefault9 = (userRating.general?.drift).GetValueOrDefault();
			float valueOrDefault10 = ((float?)userRating.general?.gifts).GetValueOrDefault();
			float valueOrDefault11 = (userRating.general?.passanger_distance).GetValueOrDefault();
			float valueOrDefault12 = ((float?)userRating.general?.police).GetValueOrDefault();
			float valueOrDefault13 = ((float?)userRating.general?.block_post).GetValueOrDefault();
			float valueOrDefault14 = ((float?)userRating.general?.real_estate).GetValueOrDefault();
			float valueOrDefault15 = (userRating.general?.fuel).GetValueOrDefault();
			float valueOrDefault16 = ((float?)userRating.general?.car_trade).GetValueOrDefault();
			float valueOrDefault17 = ((float?)userRating.general?.burnt_tire).GetValueOrDefault();
			float valueOrDefault18 = ((float?)userRating.general?.car_exchange).GetValueOrDefault();
			float valueOrDefault19 = ((float?)userRating.general?.car_fix).GetValueOrDefault();
			float valueOrDefault20 = ((float?)userRating.general?.car_wash).GetValueOrDefault();
			float valueOrDefault21 = (userRating.general?.offroad).GetValueOrDefault();
			float valueOrDefault22 = ((float?)userRating.general?.reactions).GetValueOrDefault();
			float valueOrDefault23 = (userRating.general?.drift_max).GetValueOrDefault();
			float valueOrDefault24 = (userRating.general?.taxi).GetValueOrDefault();
			float valueOrDefault25 = (userRating.general?.delivery).GetValueOrDefault();
			float valueOrDefault26 = (userRating.general?.cargo).GetValueOrDefault();
			fillDistance.SetValue(valueOrDefault / 10000f, Utils.GetConvertedMileage(valueOrDefault).ToString("F2") + Utils.CurrentMetric);
			fillGameTime.SetValue(valueOrDefault2 / 30000f, string.Concat(num.ToString(), new LocalizedString("AchievementsWindow.H"), num2.ToString(), new LocalizedString("AchievementsWindow.Min")));
			labelGameTime.text = string.Concat(num.ToString(), new LocalizedString("AchievementsWindow.H"), num2.ToString(), new LocalizedString("AchievementsWindow.Min"));
			fillDragWins.SetValue(valueOrDefault3 / 1000f, valueOrDefault3.ToString() ?? "");
			fillLevelDone.SetValue(valueOrDefault4 / (float)(LevelsConfig.IDOrderLevels.Length - 1), valueOrDefault4.ToString() ?? "");
			sliderRun.value = valueOrDefault5 / 500f;
			sliderSpeedBunner.value = valueOrDefault6 / 1000f;
			sliderTreasure.value = valueOrDefault7 / 100f;
			sliderDrift.value = valueOrDefault9 / 1000f;
			sliderCars.value = valueOrDefault8 / (float)V.CoreGameManager.AllCarNumber;
			sliderGifts.value = valueOrDefault10 / 88f;
			sliderPassangerDistance.value = valueOrDefault11 / 1000f;
			sliderPolice.value = valueOrDefault12 / 1000f;
			sliderBlockPost.value = valueOrDefault13 / 1000f;
			sliderRealEstate.value = valueOrDefault14 / 4f;
			sliderFuel.value = valueOrDefault15 / 10000f;
			sliderTrade.value = valueOrDefault16 / 100f;
			sliderBurntTyre.value = valueOrDefault17 / 100f;
			sliderExchange.value = valueOrDefault18 / 100f;
			sliderCarFix.value = valueOrDefault19 / 100f;
			sliderCarWash.value = valueOrDefault20 / 100f;
			sliderOffroad.value = valueOrDefault21 / 1000f;
			sliderReactions.value = valueOrDefault22 / 2000f;
			sliderDriftMax.value = valueOrDefault23 / 1000f;
			sliderTaxi.value = valueOrDefault24 / 1000f;
			sliderDelivery.value = valueOrDefault25 / 1000f;
			sliderCargo.value = valueOrDefault26 / 1000f;
			labelAllAchieveCount.text = "0";
			overallAchievementsFill.SetValue(0f);
			if ((bool)fillGameTime)
			{
				fillGameTime.gameObject.SetActive(value: true);
			}
			if ((bool)fillDistance)
			{
				fillDistance.gameObject.SetActive(value: true);
			}
			if ((bool)fillDragWins)
			{
				fillDragWins.gameObject.SetActive(value: true);
			}
			if ((bool)fillLevelDone)
			{
				fillLevelDone.gameObject.SetActive(value: true);
			}
			if ((bool)overallAchievementsFill)
			{
				overallAchievementsFill.gameObject.SetActive(value: true);
			}
		}
	}

	private async UniTaskVoid SetAlhaPanelAsync(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			loadingSign.SetActive(value: false);
			while (panelMain.alpha != 1f)
			{
				panelMain.alpha += 0.005f;
				await UniTask.WaitForEndOfFrame(this, ct);
			}
			if (!_isDestroyed)
			{
				bar.gameObject.SetActive(value: true);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask LoadAchievements(RatingResultAll userRating, int rank, CancellationToken ct = default(CancellationToken))
	{
		_ = 1;
		try
		{
			if (userRating == null)
			{
				return;
			}
			allAchievementList = new List<AchievementItemData>();
			countChild = 0;
			countAllAchievments = 0;
			float? num = userRating.achievements?.t_distance;
			int index = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.time;
			int index2 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.race_win;
			int index3 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.levels;
			int index4 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.taxi;
			int index5 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.run;
			int index6 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.speed_banner;
			int index7 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.treasure;
			int index8 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.cars;
			int index9 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.drift;
			int index10 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.gifts;
			int index11 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.passanger_distance;
			int index12 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.police;
			int index13 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.block_post;
			int index14 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.real_estate;
			int index15 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.fuel;
			int index16 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.car_trade;
			int index17 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.burnt_tire;
			int index18 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.car_exchange;
			int index19 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.car_fix;
			int index20 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.car_wash;
			int index21 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.offroad;
			int index22 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.reactions;
			int index23 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.drift_max;
			int index24 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.delivery;
			int index25 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			num = userRating.achievements?.cargo;
			int index26 = ((!num.HasValue) ? (-1) : ((int)num.Value));
			if (num.HasValue)
			{
				_ = num.Value;
			}
			allAchievementList.Add(createOneItemClass("t_distance", index));
			allAchievementList.Add(createOneItemClass("time", index2));
			allAchievementList.Add(createOneItemClass("race_win", index3));
			allAchievementList.Add(createOneItemClass("levels", index4));
			allAchievementList.Add(createOneItemClass("taxi", index5));
			allAchievementList.Add(createOneItemClass("run", index6));
			allAchievementList.Add(createOneItemClass("speed_banner", index7));
			allAchievementList.Add(createOneItemClass("treasure", index8));
			allAchievementList.Add(createOneItemClass("cars", index9));
			allAchievementList.Add(createOneItemClass("drift", index10));
			allAchievementList.Add(createOneItemClass("passanger_distance", index12));
			allAchievementList.Add(createOneItemClass("gifts", index11));
			allAchievementList.Add(createOneItemClass("police", index13));
			allAchievementList.Add(createOneItemClass("block_post", index14));
			allAchievementList.Add(createOneItemClass("real_estate", index15));
			allAchievementList.Add(createOneItemClass("fuel", index16));
			allAchievementList.Add(createOneItemClass("car_trade", index17));
			allAchievementList.Add(createOneItemClass("burnt_tire", index18));
			allAchievementList.Add(createOneItemClass("car_exchange", index19));
			allAchievementList.Add(createOneItemClass("car_fix", index20));
			allAchievementList.Add(createOneItemClass("car_wash", index21));
			allAchievementList.Add(createOneItemClass("offroad", index22));
			allAchievementList.Add(createOneItemClass("reactions", index23));
			allAchievementList.Add(createOneItemClass("drift_max", index24));
			allAchievementList.Add(createOneItemClass("delivery", index25));
			allAchievementList.Add(createOneItemClass("cargo", index26));
			windowOpenCount++;
			int countOpen = windowOpenCount;
			List<AchievementItemData> doNotAchievedYet = new List<AchievementItemData>();
			foreach (AchievementItemData item in allAchievementList)
			{
				for (int i = 0; i < 4; i++)
				{
					await UniTask.Yield(ct);
				}
				if (countOpen != windowOpenCount)
				{
					return;
				}
				if (item.DoNotHaveAtAll)
				{
					doNotAchievedYet.Add(item);
				}
				else
				{
					CreateOneAchievementItem(item);
				}
			}
			if (_isDestroyed)
			{
				return;
			}
			foreach (AchievementItemData item2 in doNotAchievedYet)
			{
				if (countOpen != windowOpenCount)
				{
					return;
				}
				CreateOneAchievementItem(item2);
			}
			achievementListBar.value = 0f;
			SetNextRankProgressBar(userRating, rank);
			HideDetailDescription();
			await UniTask.Yield(ct);
			LayoutRebuilder.ForceRebuildLayoutImmediate(parentAchievements);
		}
		catch (OperationCanceledException)
		{
		}
	}

	private AchievementItemData createOneItemClass(string achievs, int index)
	{
		bool flag = index == -1;
		if (flag)
		{
			index = 0;
		}
		(string, string, string, Texture, string, string) tuple = ratingDataCollection.achievs[achievs].Get(index);
		AchievementItemData obj = new AchievementItemData
		{
			rang = tuple.Item5,
			score = tuple.Item3,
			title = tuple.Item1,
			middleTexture = tuple.Item4,
			mainDescription = tuple.Item6,
			description = tuple.Item2
		};
		int num = int.Parse(tuple.Item5);
		Texture backgroundTexture = Resources.Load<Texture>("icons/Rating/Ach_BG_" + (num + 1));
		obj.backgroundTexture = backgroundTexture;
		obj.DoNotHaveAtAll = flag;
		return obj;
	}

	private void CreateOneAchievementItem(AchievementItemData itemData)
	{
		achievmentItemUI obj = _prefabFactory.Instantiate(oneItemPrefab, parentAchievements);
		obj.SetData(itemData, this);
		obj.SetItem();
		countChild++;
		int num = int.Parse(itemData.rang) + 1;
		if (obj.DoNotHaveAtAll)
		{
			num = 0;
		}
		countAllAchievments = num + countAllAchievments;
		overallAchievementsFill.SetValue((float)countAllAchievments / 142f);
		labelAllAchieveCount.text = countAllAchievments.ToString();
	}

	private void SetNextRankProgressBar(RatingResultAll userRating, int rank)
	{
		int[] batches = userRating.batches;
		if (batches == null)
		{
			return;
		}
		List<int> list = new List<int>(batches);
		list.Add(156);
		countAllAchievments = Mathf.Clamp(countAllAchievments, 0, 156);
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (countAllAchievments < list[i])
			{
				num = list[i];
				break;
			}
			num = 156;
		}
		sliderEXP.value = (float)countAllAchievments / (float)num;
		labelEXP.text = $"{countAllAchievments}/{num}";
		string rankNameByIndex = Utils.GetRankNameByIndex(rank);
		if ((bool)targetRankText)
		{
			if (rank < 6)
			{
				string rankNameByIndex2 = Utils.GetRankNameByIndex(rank + 1);
				int num2 = num - countAllAchievments;
				string text = new LocalizedString("AchievementsWindow.NextCountallachievmentsAchieve");
				targetRankText.text = text.Replace("{0}", num2.ToString()).Replace("{nextRankName}", rankNameByIndex2);
			}
			else
			{
				targetRankText.text = playerDataStorage.PlayerName + " is the " + Utils.GetRankNameByIndex(rank);
			}
			labelRank.text = rankNameByIndex;
			textureRankIcon.texture = UtilsRank.LoadRankTextureFromResources((UtilsRank.RankType)rank);
			textureRankIcon.gameObject.SetActive(value: true);
			labelLevel.text = rank.ToString();
		}
	}

	public void ShowDetailDescription(achievmentItemUI ui)
	{
		if (!(achievDetailsText == null))
		{
			SelectedAchievmentPanel.SetActive(value: true);
			AchievmentsGridPanel.SetActive(value: false);
			achievementListBar.value = (ui.DoNotHaveAtAll ? 0f : ((float)(int.Parse(ui.rang) + 1) / 6f));
			achievDetailsMain.texture = ui.middleTexture;
			achievDetailsBackground.texture = ui.backgroundTexture;
			achievDetailsTitle.text = new LocalizedString(ui.title);
			if (ui.DoNotHaveAtAll)
			{
				achievDetailsText.text = new LocalizedString(ui.mainDescription);
			}
			else
			{
				achievDetailsText.text = new LocalizedString(ui.description);
			}
		}
	}

	public void HideDetailDescription()
	{
		if ((bool)achievDetailsText)
		{
			SelectedAchievmentPanel.SetActive(value: false);
			AchievmentsGridPanel.SetActive(value: true);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;
		_isDestroyed = true;
	}
}
