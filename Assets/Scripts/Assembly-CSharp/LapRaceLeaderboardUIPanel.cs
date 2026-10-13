using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

public class LapRaceLeaderboardUIPanel : WindowScript
{
	[SerializeField]
	private LapRaceLeaderbordUiElement LeaderbordUiElementPrefab;

	[SerializeField]
	private RectTransform leaderboardParent;

	private IPrefabFactory _prefabFactory;

	private CircuitRaceControlller _circuitRaceControlller;

	private List<LapRaceLeaderbordUiElement> _lapRaceLeaderbordUiElements = new List<LapRaceLeaderbordUiElement>();

	[SerializeField]
	private TextMeshProUGUI TitleText;

	[SerializeField]
	private TextMeshProUGUI ResultText;

	[SerializeField]
	private RectTransform TitleImg;

	[SerializeField]
	[Header("Buttons")]
	private Button ExitButton;

	[SerializeField]
	private Button MenuButton;

	[SerializeField]
	private Button RestartButton;

	[SerializeField]
	private Button NextButton;

	[SerializeField]
	private CanvasGroup TopButtonsGroup;

	[SerializeField]
	private CanvasGroup BottomButtonsGroup;

	[SerializeField]
	[Header("Status")]
	private RawImage StatusPanel;

	[SerializeField]
	private TextMeshProUGUI StatusText;

	[SerializeField]
	private Texture2D WinBorderSprite;

	[SerializeField]
	private Texture2D LooseBorderSprite;

	[Header("Reward")]
	[SerializeField]
	private CircuitRewardUiElement[] RewardUiElements;

	[SerializeField]
	private Slider RewardProgressbar;

	[SerializeField]
	private float RewardProgressbarDuration;

	[SerializeField]
	private Color[] RewardColors;

	[SerializeField]
	private Color LooseColor;

	[SerializeField]
	private TextMeshProUGUI AllTimeText;

	[SerializeField]
	private TextMeshProUGUI BestTimeText;

	[SerializeField]
	private TextMeshProUGUI PenaltyText;

	[SerializeField]
	private TextMeshProUGUI LapsText;

	private bool _isDestroyed;

	private LevelScript _levelScript;

	private CircuitRacingUIControl _circuitRacingUIControl;

	private IRacingRewardAdService _racingRewardAdService;

	private const float SizeRightOn = -60f;

	private const float SizeRightOff = -130f;

	private const float RacersWaitMaxTime = 5f;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

	protected override bool ChangeRootGameInputGroup => true;

	[UnityEngine.Scripting.Preserve]
	[Inject]
	public void Construct(IRacingRewardAdService racingRewardAdService)
	{
		_racingRewardAdService = racingRewardAdService;
	}

	public void Init(IPrefabFactory prefabFactory, CircuitRaceControlller circuitRaceControlller, ManagerUI managerUI)
	{
		_prefabFactory = prefabFactory;
		_circuitRaceControlller = circuitRaceControlller;
		_circuitRacingUIControl = circuitRaceControlller.CircuitRacingUIControll;
		_managerUI = managerUI;
		EnableButtons(isEnabled: false);
		if (!_circuitRaceControlller.IsCurrentMapLeman())
		{
			FinishInit(_circuitRaceControlller.circuitCarsControl.GetRaceCars()).Forget();
		}
		_circuitRaceControlller.CircuitRacingUIControll.ShowChartList(state: false);
		CircuitRaceControlller circuitRaceControlller2 = _circuitRaceControlller;
		circuitRaceControlller2.OnRewardCalculated = (Action)Delegate.Combine(circuitRaceControlller2.OnRewardCalculated, new Action(OnRewardCalculated));
		_ = _managerUI.ShowUserBalanceOnlyWindow();
		SubscribeButtons();
	}

	private void SubscribeButtons()
	{
		RestartButton.onClick.AddListener(Restart);
		MenuButton.onClick.AddListener(OnGarage);
		ExitButton.onClick.AddListener(_circuitRaceControlller.GoMainMenu);
		NextButton.onClick.AddListener(StartNextRace);
		Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			EnableButtons(isEnabled: true);
		}).AddTo(this);
		base.Start();
	}

	protected override void OnDestroy()
	{
		_isDestroyed = true;
		CircuitRaceControlller circuitRaceControlller = _circuitRaceControlller;
		circuitRaceControlller.OnRewardCalculated = (Action)Delegate.Remove(circuitRaceControlller.OnRewardCalculated, new Action(OnRewardCalculated));
		_managerUI.CloseSeveralWindows();
		base.OnDestroy();
	}

	private void OnRewardCalculated()
	{
		EnableButtons(isEnabled: true);
		ShowRewardAdWindow().Forget();
	}

	private async UniTaskVoid ShowRewardAdWindow()
	{
		await UniTask.NextFrame();
		if (!_isDestroyed)
		{
			int place = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().place;
			int softReward = CircuitRaceControlller.SoftReward;
			int hardCurrency = CircuitRaceControlller.HardCurrency;
			if (place < 3 && softReward + hardCurrency > 0 && _racingRewardAdService != null && _racingRewardAdService.CanOfferForNewRace() && await _managerUI.ShowWindow("RacingRewardAdWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false) is RacingRewardAdWindow racingRewardAdWindow)
			{
				racingRewardAdWindow.InitReward(RacingRewardMode.Circuit, _circuitRaceControlller.GetCurrentRewards().Reverse().ToArray(), 3 - place);
			}
		}
	}

	private void EnableButtons(bool isEnabled)
	{
		TopButtonsGroup.interactable = isEnabled;
		TopButtonsGroup.alpha = (isEnabled ? 1f : 0.5f);
		BottomButtonsGroup.interactable = isEnabled;
		BottomButtonsGroup.alpha = (isEnabled ? 1f : 0.5f);
	}

	private void SetHeaderText(bool isLeman)
	{
		AllTimeText.text = (isLeman ? new LocalizedString("LapRaceLeaderboardUIPanel.Distance") : new LocalizedString("Circuitraceresultwindow.Time"));
		BestTimeText.text = (isLeman ? new LocalizedString("LapRaceLeaderboardUIPanel.BestDistance") : new LocalizedString("LapRaceLeaderboardUIPanel.BestTime"));
		PenaltyText.text = new LocalizedString("Circuitraceresultwindow.Penalty");
	}

	public async UniTaskVoid FinishInit(List<LapRaceCar> lapRaceCars)
	{
		SetHeaderText(isLeman: false);
		StatusPanel.SetActive(active: false);
		LapInfoActive(isActive: false);
		string format = new LocalizedString("LapRaceLeaderboardUIPanel.Track");
		SetTitleText(string.Format(format, _circuitRaceControlller.GetRaceMapModel().RaceConfig.TrackName));
		InitRewards(_circuitRaceControlller.GetCurrentRewards());
		if (_lapRaceLeaderbordUiElements == null)
		{
			_lapRaceLeaderbordUiElements = new List<LapRaceLeaderbordUiElement>();
		}
		foreach (LapRaceCar item in from lapRaceCar in lapRaceCars
			where lapRaceCar.IsFinishRace()
			where !IsLapElementExists(lapRaceCar)
			select lapRaceCar)
		{
			AddLeaderboardElement(item);
		}
		UIHelper.DelayedRebuildLayout(leaderboardParent).Forget();
	}

	private void SetTitleText(string trackName)
	{
		TitleText.text = trackName;
		ResultText.text = "/" + new LocalizedString("LapRaceLeaderboardUIPanel.Result");
	}

	public void FinishInitLeman(List<LapRaceCar> lapRaceCars)
	{
		SetHeaderText(isLeman: true);
		StatusPanel.SetActive(active: false);
		LapInfoActive(isActive: true);
		SetTitleText(new LocalizedString("LapRaceLeaderboardUIPanel.LeMans"));
		InitRewards(_circuitRaceControlller.GetCurrentRewards());
		bool flag = false;
		if (_lapRaceLeaderbordUiElements == null)
		{
			_lapRaceLeaderbordUiElements = new List<LapRaceLeaderbordUiElement>();
		}
		for (int i = 0; i < lapRaceCars.Count; i++)
		{
			if (!lapRaceCars[i].IsLemanFinished && !lapRaceCars[i].IsBroken)
			{
				lapRaceCars[i].FinishRace();
				lapRaceCars[i].SetFinalDistance(lapRaceCars[i].GetTotalDistanceForLeman());
			}
		}
		LapRaceCar lapRaceCar = null;
		lapRaceCars = lapRaceCars.OrderByDescending((LapRaceCar car) => car.GetFinalDistance()).ToList();
		for (int num = 0; num < lapRaceCars.Count; num++)
		{
			if (lapRaceCars[num].IsUser())
			{
				lapRaceCar = lapRaceCars[num];
			}
			lapRaceCars[num].place = num;
			if (!IsLapElementExists(lapRaceCars[num]))
			{
				LapRaceLeaderbordUiElement lapRaceLeaderbordUiElement = AddLeaderboardElement(lapRaceCars[num]);
				lapRaceLeaderbordUiElement.SetPlace(lapRaceCars[num].place);
				if (!lapRaceCars[num].IsLemanFinished && lapRaceCars[num].IsBroken)
				{
					lapRaceLeaderbordUiElement.Disqualified();
					lapRaceCars[num].IsDisqualified = true;
					flag = true;
				}
			}
		}
		ShowRaceStatus(_circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().place, base.destroyCancellationToken).Forget();
		int place = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().place;
		if (place < 3)
		{
			_circuitRaceControlller.Calculate(place).Forget();
		}
		else
		{
			EnableButtons(isEnabled: true);
		}
		if (flag)
		{
			SortLemanLiderBoard();
		}
		SortUsersFinal();
		if ((bool)lapRaceCar)
		{
			_circuitRaceControlller.RaceCrewTalkSoundController.PlayFinishLemanSoundEvent(lapRaceCar, isTimeExpired: true);
		}
		_ = UIHelper.DelayedRebuildLayout(leaderboardParent);
	}

	private void LapInfoActive(bool isActive)
	{
		if ((bool)LapsText)
		{
			LapsText.transform.parent.gameObject.SetActive(isActive);
		}
		float x = (isActive ? (-60f) : (-130f));
		if ((bool)TitleImg)
		{
			TitleImg.offsetMax = new Vector2(x, TitleImg.offsetMax.y);
		}
	}

	private void SortLemanLiderBoard()
	{
		List<LapRaceLeaderbordUiElement> source = _lapRaceLeaderbordUiElements.Where((LapRaceLeaderbordUiElement board) => !board.IsDisqualified).ToList();
		List<LapRaceLeaderbordUiElement> list = _lapRaceLeaderbordUiElements.Where((LapRaceLeaderbordUiElement board) => board.IsDisqualified).ToList();
		source = source.OrderByDescending((LapRaceLeaderbordUiElement element) => element.TotalTime).ToList();
		for (int num = 0; num < source.Count; num++)
		{
			source[num].transform.SetSiblingIndex(num);
			source[num].SetPlace(num);
		}
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			list[num2].transform.SetSiblingIndex(num2 + source.Count);
		}
	}

	private LapRaceLeaderbordUiElement AddLeaderboardElement(LapRaceCar lapRaceCar)
	{
		LapRaceLeaderbordUiElement lapRaceLeaderbordUiElement = _prefabFactory.Instantiate(LeaderbordUiElementPrefab, leaderboardParent);
		lapRaceLeaderbordUiElement.index = lapRaceCar.GetRaceCarId();
		if (_circuitRaceControlller.IsCurrentMapLeman())
		{
			lapRaceLeaderbordUiElement.InitLeman(lapRaceCar, this);
			lapRaceLeaderbordUiElement.TotalTime = lapRaceCar.GetTotalDistanceForLeman();
		}
		else
		{
			lapRaceLeaderbordUiElement.Init(lapRaceCar, this);
			lapRaceLeaderbordUiElement.TotalTime = lapRaceCar.GetTotalTime();
		}
		_lapRaceLeaderbordUiElements.Add(lapRaceLeaderbordUiElement);
		return lapRaceLeaderbordUiElement;
	}

	public Color GetPlaceColor(int place)
	{
		if (place < 3)
		{
			return RewardColors[place];
		}
		return LooseColor;
	}

	public void Reset()
	{
		_lapRaceLeaderbordUiElements = null;
	}

	public async UniTaskVoid FinishInit(LapRaceCar lapRaceCar)
	{
		if (_lapRaceLeaderbordUiElements != null && !IsLapElementExists(lapRaceCar) && !IsLapElementExists(lapRaceCar))
		{
			LapRaceLeaderbordUiElement userLeaderbordUiElement = AddLeaderboardElement(lapRaceCar);
			SortUsers(userLeaderbordUiElement);
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
			_ = UIHelper.DelayedRebuildLayout(leaderboardParent);
		}
	}

	public void SortUsersFinal()
	{
		if (_lapRaceLeaderbordUiElements == null)
		{
			return;
		}
		_lapRaceLeaderbordUiElements = ((!_circuitRaceControlller.IsCurrentMapLeman()) ? _lapRaceLeaderbordUiElements.OrderBy((LapRaceLeaderbordUiElement element) => element.TotalTime).ToList() : _lapRaceLeaderbordUiElements.OrderByDescending((LapRaceLeaderbordUiElement element) => element.TotalTime).ToList());
		LapRaceCar userLapRaceCar = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar();
		bool flag = false;
		for (int num = 0; num < _lapRaceLeaderbordUiElements.Count; num++)
		{
			_lapRaceLeaderbordUiElements[num].transform.SetSiblingIndex(num);
			_lapRaceLeaderbordUiElements[num].SetPlace(num);
			if (!flag && _lapRaceLeaderbordUiElements[num].index == userLapRaceCar.GetRaceCarId())
			{
				userLapRaceCar.place = num;
				flag = true;
			}
		}
		ShowRaceStatus(userLapRaceCar.place, base.destroyCancellationToken).Forget();
	}

	private bool IsLapElementExists(LapRaceCar lapRaceCar)
	{
		foreach (LapRaceLeaderbordUiElement lapRaceLeaderbordUiElement in _lapRaceLeaderbordUiElements)
		{
			if (lapRaceLeaderbordUiElement.index == lapRaceCar.GetRaceCarId())
			{
				return true;
			}
		}
		return false;
	}

	private void SortUsers(LapRaceLeaderbordUiElement userLeaderbordUiElement)
	{
		if (_lapRaceLeaderbordUiElements == null)
		{
			return;
		}
		int siblingIndex = 0;
		foreach (LapRaceLeaderbordUiElement lapRaceLeaderbordUiElement in _lapRaceLeaderbordUiElements)
		{
			if (userLeaderbordUiElement.TotalTime < lapRaceLeaderbordUiElement.TotalTime)
			{
				siblingIndex = lapRaceLeaderbordUiElement.transform.GetSiblingIndex();
			}
		}
		userLeaderbordUiElement.gameObject.transform.SetSiblingIndex(siblingIndex);
	}

	public void Restart()
	{
		_lapRaceLeaderbordUiElements = null;
		_circuitRaceControlller.RestartRace();
		_managerUI.ShowCircuitPlayWindow();
		Close();
	}

	public void OnGarage()
	{
		_lapRaceLeaderbordUiElements = null;
		_circuitRaceControlller.GoGarageWithoutFinish(showMenu: false);
		_managerUI.CloseCircuitPlayWindow();
		_managerUI.ShowCircuitMapWindow(cleanScreen: false);
		_managerUI.CloseBalanceOnlyWindow();
		base.gameObject.SetActive(value: false);
	}

	private void StartNextRace()
	{
		_managerUI.CloseSeveralWindows(userBalanceWindowOnly: true);
		_circuitRaceControlller.StartNextTrack();
		_managerUI.ShowCircuitPlayWindow();
		Close();
	}

	public void InitRewards(int[] rewards)
	{
		RewardProgressbar.value = 0f;
		rewards = rewards.Reverse().ToArray();
		for (int i = 0; i < rewards.Length; i++)
		{
			RewardUiElements[i].Init(rewards[i]);
		}
	}

	private async UniTask ShowRaceStatus(int userPlace, CancellationToken cancellationToken)
	{
		_ = 11;
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			bool flag = userPlace < 3;
			StatusText.text = (flag ? new LocalizedString("LapRaceLeaderboardUIPanel.YouWin") : new LocalizedString("LapRaceLeaderboardUIPanel.YouLost"));
			StatusPanel.texture = (flag ? WinBorderSprite : LooseBorderSprite);
			await UniTask.NextFrame(cancellationToken);
			StatusPanel.SetActive(active: true);
			switch (userPlace)
			{
			case 0:
				await RewardUiElements[0].WinAnimation(cancellationToken);
				RewardProgressbar.DOValue(1f, RewardProgressbarDuration).SetLink(base.gameObject);
				await UniTask.Delay(TimeSpan.FromSeconds(RewardProgressbarDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				await RewardUiElements[1].WinAnimation(cancellationToken);
				RewardProgressbar.DOValue(2f, RewardProgressbarDuration).SetLink(base.gameObject);
				await UniTask.Delay(TimeSpan.FromSeconds(RewardProgressbarDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				await RewardUiElements[2].WinAnimation(cancellationToken);
				break;
			case 1:
				await RewardUiElements[0].WinAnimation(cancellationToken);
				RewardProgressbar.DOValue(1f, RewardProgressbarDuration).SetLink(base.gameObject);
				await UniTask.Delay(TimeSpan.FromSeconds(RewardProgressbarDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				await RewardUiElements[1].WinAnimation(cancellationToken);
				break;
			case 2:
				await RewardUiElements[0].WinAnimation(cancellationToken);
				break;
			default:
			{
				CircuitRewardUiElement[] rewardUiElements = RewardUiElements;
				for (int i = 0; i < rewardUiElements.Length; i++)
				{
					await rewardUiElements[i].LoseAnimation(cancellationToken);
				}
				break;
			}
			}
		}
		catch (OperationCanceledException)
		{
			Debug.Log("Race status sequence aborted because the panel was destroyed.");
		}
	}
}
