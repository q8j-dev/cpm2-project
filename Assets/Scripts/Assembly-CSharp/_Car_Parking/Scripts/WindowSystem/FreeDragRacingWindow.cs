using System;
using System.Collections.Generic;
using CPM.PlayerData;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class FreeDragRacingWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		[Header("Header Info")]
		private Image RankIcon;

		[SerializeField]
		private TextMeshProUGUI PlayerName;

		[SerializeField]
		private TextMeshProUGUI PlayerId;

		[SerializeField]
		private TextMeshProUGUI Money;

		[SerializeField]
		private TextMeshProUGUI Coins;

		[SerializeField]
		[Header("Left Side")]
		private Image LeftRankIcon;

		[SerializeField]
		private Image LeftCarIcon;

		[SerializeField]
		private TextMeshProUGUI LeftName;

		[SerializeField]
		private TextMeshProUGUI LeftPower;

		[SerializeField]
		private TextMeshProUGUI LeftTurbo;

		[SerializeField]
		[Header("Right Side")]
		private Image RightRankIcon;

		[SerializeField]
		private Image RightCarIcon;

		[SerializeField]
		private TextMeshProUGUI RightName;

		[SerializeField]
		private TextMeshProUGUI RightPower;

		[SerializeField]
		private TextMeshProUGUI RightTurbo;

		[Header("Distance Elements")]
		[SerializeField]
		private TextMeshProUGUI WinMoney;

		[SerializeField]
		private TextMeshProUGUI LoseMoney;

		[SerializeField]
		private Toggle[] LeftToggles;

		[SerializeField]
		private Toggle[] RightToggles;

		[SerializeField]
		private TextMeshProUGUI[] LeftTogglesTexts;

		[SerializeField]
		private TextMeshProUGUI[] RightToggleTexts;

		[SerializeField]
		private Button CancelButton;

		[SerializeField]
		private GameObject TimerObject;

		[SerializeField]
		private TextMeshProUGUI Timer;

		[SerializeField]
		private Image FilledImage;

		[Header("Result Elements")]
		[SerializeField]
		private TextMeshProUGUI ResultMoney;

		[SerializeField]
		private TextMeshProUGUI environmentTittle;

		[SerializeField]
		private TextMeshProUGUI Resultitle;

		[SerializeField]
		private TextMeshProUGUI LeftTime;

		[SerializeField]
		private TextMeshProUGUI RightTime;

		[SerializeField]
		private TextMeshProUGUI Left400;

		[SerializeField]
		private TextMeshProUGUI Right400;

		[SerializeField]
		private TextMeshProUGUI Left100;

		[SerializeField]
		private TextMeshProUGUI Right100;

		[SerializeField]
		private TextMeshProUGUI LeftMaxSpeed;

		[SerializeField]
		private TextMeshProUGUI RightMaxSpeed;

		[SerializeField]
		private TextMeshProUGUI LeftReaction;

		[SerializeField]
		private TextMeshProUGUI RightReaction;

		[SerializeField]
		private TextMeshProUGUI LeftResult;

		[SerializeField]
		private TextMeshProUGUI RightResult;

		[SerializeField]
		private Toggle[] LeftResultToggles;

		[SerializeField]
		private Toggle[] RightResultToggles;

		[SerializeField]
		private Color ColorToMore;

		[SerializeField]
		private Color ColorToLess;

		[SerializeField]
		private Color ColorToWin;

		[SerializeField]
		private Color ColorToLose;

		[SerializeField]
		private GameObject[] ChooseDistanceObjects;

		[SerializeField]
		private GameObject[] ResultObjects;

		[SerializeField]
		private TextMeshProUGUI distanceOwn1;

		[SerializeField]
		private TextMeshProUGUI distanceOwn2;

		[SerializeField]
		private TextMeshProUGUI distanceOwn3;

		[SerializeField]
		private TextMeshProUGUI distanceOther1;

		[SerializeField]
		private TextMeshProUGUI distanceOther2;

		[SerializeField]
		private TextMeshProUGUI distanceOther3;

		[SerializeField]
		private TextMeshProUGUI maxSpeedTittle;

		private IDisposable _timer;

		private PlayerDataStorage _playerDataStorage;

		private CurrencyManagerController _currencyManagerController;

		private bool _isClickCancelButton;

		private IPricesService _pricesService;

		private IEarnMoneyService _earnMoneyService;

		private FreeDriveProvider _freeDriveProvider;

		private MultiPlayer _multiPlayer;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage, CurrencyManagerController currencyManagerController, IPricesService pricesService, IEarnMoneyService earnMoneyService, FreeDriveProvider freeDriveProvider, MultiPlayer multiPlayer)
		{
			_freeDriveProvider = freeDriveProvider;
			_earnMoneyService = earnMoneyService;
			_pricesService = pricesService;
			_currencyManagerController = currencyManagerController;
			_playerDataStorage = playerDataStorage;
			_multiPlayer = multiPlayer;
		}

		protected override void Awake()
		{
			base.Awake();
			_freeDriveProvider.FreeDriveControl.newNegotiationWindow = this;
		}

		protected override void Start()
		{
			base.Start();
			_freeDriveProvider.DragRacingControlMultiplayer.lastInput = -1;
			CancelButton.onClick.AddListener(Back);
			SetUserInfo();
			SetWinLoseMoney(_earnMoneyService.MultiplayerRaceWin, _pricesService.MultiplayerRaceLose);
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
		}

		public void Back()
		{
			_isClickCancelButton = true;
			SetOwnDistanceChanged(nowOn: true);
			Close();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				if (_isClickCancelButton)
				{
					_freeDriveProvider.FreeDriveControl.CancelButtonNegWindow();
				}
			}
		}

		public void ThisCloseFinishResult()
		{
			_freeDriveProvider.FreeDriveControl.CloseFinishMenu();
		}

		public void replayNoForReplay()
		{
			_freeDriveProvider.DragRacingControlMultiplayer.ReplayNORestart();
		}

		public void replayYesForReplay()
		{
			if ((int)CurrencyManagerController.Money < (int)_pricesService.MultiplayerRaceLose)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.YouDonTHaveEnoughMoney"), 3f);
			}
			else
			{
				_freeDriveProvider.DragRacingControlMultiplayer.ReplayYesRestart();
			}
		}

		public void otherCarRepliedYES()
		{
			RightResultToggles[0].isOn = true;
			RightResultToggles[1].isOn = false;
		}

		public void otherCarRepliedNO()
		{
			RightResultToggles[0].isOn = false;
			RightResultToggles[1].isOn = true;
		}

		protected override void OnDestroy()
		{
			CancelButton.onClick.RemoveAllListeners();
			base.OnDestroy();
		}

		private void SetUserInfo()
		{
			Sprite sprite = UtilsRank.LoadMyRankSpriteFromResources();
			RankIcon.sprite = sprite;
			LeftRankIcon.sprite = sprite;
			LeftName.text = _playerDataStorage.PlayerName;
			PlayerName.text = _playerDataStorage.PlayerName;
			PlayerId.text = _playerDataStorage.PlayerID;
			Money.text = CurrencyManagerController.Money.ToString();
			Coins.text = CurrencyManagerController.Coins.ToString();
		}

		private void ActivateResult(bool isResult)
		{
			GameObject[] resultObjects = ResultObjects;
			for (int i = 0; i < resultObjects.Length; i++)
			{
				resultObjects[i].SetActive(isResult);
			}
			resultObjects = ChooseDistanceObjects;
			for (int i = 0; i < resultObjects.Length; i++)
			{
				resultObjects[i].SetActive(!isResult);
			}
		}

		private void TogglesToDefault()
		{
			for (int i = 0; i < LeftToggles.Length; i++)
			{
				if (!LeftToggles[i].isOn)
				{
					LeftToggles[i].interactable = true;
					LeftTogglesTexts[i].color = Color.white;
				}
			}
			for (int j = 0; j < RightToggles.Length; j++)
			{
				if (!RightToggles[j].isOn)
				{
					RightToggleTexts[j].color = Color.white;
				}
			}
		}

		private void FillBothSideInfo(bool useCached)
		{
			VehicleData component = V.LastUsedCar.GetComponent<VehicleData>();
			V.LastUsedCar.GetComponent<Powertrain>().CalcMaxPowerMaxTorque(out var maxHp, out var maxHpRpm, out var maxTq, out var maxTqRpm, out var maxWhp);
			int rank = ObscuredPrefs.GetInt("rank" + DateTime.Now.Day);
			SetPlayerInfo(_freeDriveProvider.DragRacingControlMultiplayer.playerDataStorage.PlayerName, rank, component.CarId, maxHp, maxTq);
			if (useCached)
			{
				NewRacingMultiplayerController.EnemyInfo cachedLastEnemy = _freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.cachedLastEnemy;
				SetEnemyInfo(cachedLastEnemy.PlayerName, cachedLastEnemy.Rank, cachedLastEnemy.CarId, cachedLastEnemy.MaxPower, cachedLastEnemy.MaxTorque);
			}
			else
			{
				component = _freeDriveProvider.RacingCar2.GetComponent<VehicleData>();
				MultiPersonControl multiPersonControl = _multiPlayer.GetMultiPersonControl(component.photonView.CreatorActorNr, checkVDriverPerson: true);
				if (!multiPersonControl)
				{
					Close();
				}
				_freeDriveProvider.RacingCar2.GetComponent<Powertrain>().CalcMaxPowerMaxTorque(out maxHp, out maxWhp, out maxTq, out maxTqRpm, out maxHpRpm);
				SetEnemyInfo(multiPersonControl.ThisPlayerName, multiPersonControl.rank2, component.CarId, maxHp, maxTq);
			}
			environmentTittle.text = GetEnvironmentTittle(_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.CurrentRacePlace);
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
		}

		private string GetEnvironmentTittle(V.placeToSpawn placeToSpawn)
		{
			return placeToSpawn switch
			{
				V.placeToSpawn.MainCity => new LocalizedString("FreeDragRacingWindow.City"), 
				V.placeToSpawn.Desert => new LocalizedString("FreeDragRacingWindow.Desert"), 
				V.placeToSpawn.City2 => new LocalizedString("FreeDragRacingWindow.City2"), 
				V.placeToSpawn.Racing => new LocalizedString("FreeDragRacingWindow.RaceTrack"), 
				V.placeToSpawn.Highway => new LocalizedString("FreeDragRacingWindow.Highway"), 
				V.placeToSpawn.Offroad => new LocalizedString("DragRacingCarSettingsUI.Offroad"), 
				V.placeToSpawn.JapanSerpentine => new LocalizedString("FreeDragRacingWindow.JapanSerpentine"), 
				V.placeToSpawn.JapanSuburban => new LocalizedString("FreeDragRacingWindow.JapanSuburban"), 
				_ => "", 
			};
		}

		public void ShowChooseDistance()
		{
			ActivateResult(isResult: false);
			TogglesToDefault();
			if (!(_freeDriveProvider.RacingCar2 == null))
			{
				FillBothSideInfo(useCached: false);
				SetDistanceOrLap(_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.CurrentRunDistances);
				VehicleData component = _freeDriveProvider.RacingCar2.GetComponent<VehicleData>();
				MultiPersonControl multiPersonControl = _multiPlayer.GetMultiPersonControl(component.photonView.CreatorActorNr, checkVDriverPerson: true);
				if (!multiPersonControl)
				{
					Close();
				}
				_freeDriveProvider.RacingCar2.GetComponent<Powertrain>().CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
				_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.cachedLastEnemy = new NewRacingMultiplayerController.EnemyInfo(multiPersonControl.ThisPlayerName, multiPersonControl.rank2, component.CarId, maxHp, maxTq);
			}
		}

		private void SetDistanceOrLap(DragRacingController.RunDistance[] runDistances)
		{
			bool invitationHost = _freeDriveProvider.FreeDriveControl.InvitationHost;
			List<string> list = AddCounter(BuildDistanceLabels(runDistances, invitationHost));
			List<string> list2 = AddCounter(BuildDistanceLabels(runDistances, !invitationHost));
			distanceOwn1.text = list[0];
			distanceOwn2.text = list[1];
			distanceOwn3.text = list[2];
			distanceOther1.text = list2[0];
			distanceOther2.text = list2[1];
			distanceOther3.text = list2[2];
			int num = ((runDistances != null) ? Mathf.Min(runDistances.Length, 3) : 0);
			for (int i = 0; i < LeftToggles.Length; i++)
			{
				LeftToggles[i].interactable = i < num;
			}
			for (int j = 0; j < RightToggles.Length; j++)
			{
				RightToggles[j].interactable = j < num;
			}
		}

		private List<string> BuildDistanceLabels(DragRacingController.RunDistance[] runDistances, bool sideIsInitiator)
		{
			List<string> list = new List<string>();
			list.Add("");
			list.Add("");
			list.Add("");
			for (int i = 0; i <= 2; i++)
			{
				if (runDistances == null || i >= runDistances.Length)
				{
					continue;
				}
				if (runDistances[i] == DragRacingController.RunDistance.QuaterMile)
				{
					if (FlagsStorage.Kph)
					{
						list[i] = "402m";
					}
					else
					{
						list[i] = "1/4 mile";
					}
				}
				else if (runDistances[i] == DragRacingController.RunDistance.Lap)
				{
					list[i] = new LocalizedString("FreeDragRacingWindow.Lap");
				}
				else if (runDistances[i] == DragRacingController.RunDistance.Rally)
				{
					list[i] = new LocalizedString("FreeDragRacingWindow.Rally");
				}
				else if (runDistances[i] == DragRacingController.RunDistance.Pursuit)
				{
					list[i] = new LocalizedString(sideIsInitiator ? "FreeDragRacingWindow.Escape" : "FreeDragRacingWindow.Pursuit");
				}
				else if (runDistances[i] == DragRacingController.RunDistance.OneMile)
				{
					if (FlagsStorage.Kph)
					{
						list[i] = "1600m";
					}
					else
					{
						list[i] = "1 mile";
					}
				}
				else if (runDistances[i] == DragRacingController.RunDistance.HalfMile)
				{
					if (FlagsStorage.Kph)
					{
						list[i] = "800m";
					}
					else
					{
						list[i] = "1/2 mile";
					}
				}
			}
			return list;
		}

		private List<string> AddCounter(List<string> input)
		{
			List<string> list = new List<string>(input);
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			foreach (string item in input)
			{
				int num = 0;
				if (list3.Contains(item))
				{
					continue;
				}
				list3.Add(item);
				list.Remove(item);
				foreach (string item2 in list)
				{
					if (item == item2)
					{
						num++;
					}
				}
				if (num == 0)
				{
					list2.Add(item);
					continue;
				}
				for (int i = 0; i <= num; i++)
				{
					list2.Add(item + " " + (i + 1));
				}
			}
			return list2;
		}

		private void MakeStringsUnique(List<string> strings)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < strings.Count; i++)
			{
				int num = 1;
				for (int j = 0; j < strings.Count; j++)
				{
					if (strings[i] == strings[j] && i != j)
					{
						if (num == 1)
						{
							list.Add(strings[i] + " " + num);
							num++;
						}
						else if (j > i)
						{
							list.Add(strings[j] + " " + num);
							num++;
						}
					}
				}
				if (num == 1 || !list.Contains(strings[i]))
				{
					list.Add(strings[i] + ((num > 1) ? (" " + num) : ""));
				}
			}
			strings.Clear();
			strings.AddRange(list);
		}

		public void ShowResult()
		{
			ActivateResult(isResult: true);
		}

		public void SetPlayerInfo(string enemyName, int rank, int carId, float power, float torque)
		{
			Sprite sprite = UtilsRank.LoadRankSpriteFromResources((UtilsRank.RankType)rank);
			LeftRankIcon.sprite = sprite;
			LeftName.text = enemyName;
			LeftPower.text = Utils.GetPower(power);
			LeftTurbo.text = Utils.GetTorque(torque) + " " + Utils.GetTorquePostfix;
			Sprite sprite2 = Resources.Load<Sprite>($"UI/CarImages/b{carId}");
			LeftCarIcon.sprite = sprite2;
		}

		public void SetEnemyInfo(string enemyName, int rank, int carId, float power, float torque)
		{
			Sprite sprite = UtilsRank.LoadRankSpriteFromResources((UtilsRank.RankType)rank);
			RightRankIcon.sprite = sprite;
			RightName.text = enemyName;
			RightPower.text = Utils.GetPower(power);
			RightTurbo.text = Utils.GetTorque(torque) + " " + Utils.GetTorquePostfix;
			Sprite sprite2 = Resources.Load<Sprite>("UI/CarImages/b" + carId);
			RightCarIcon.sprite = sprite2;
		}

		public int GetPlayerDistance()
		{
			if (_isClickCancelButton)
			{
				return -1;
			}
			for (int i = 0; i < LeftToggles.Length; i++)
			{
				if (LeftToggles[i].isOn)
				{
					return i;
				}
			}
			return 0;
		}

		public void SetEnemyDistance(int distanceId)
		{
			MonoBehaviour.print("set enemy distance: " + distanceId);
			Toggle[] rightToggles = RightToggles;
			for (int i = 0; i < rightToggles.Length; i++)
			{
				rightToggles[i].isOn = false;
			}
			if (distanceId >= 0)
			{
				RightToggles[distanceId].isOn = true;
				RightToggles[0].transform.parent.GetComponent<ToggleGroup>().allowSwitchOff = false;
			}
		}

		public void SetOwnDistanceChanged(bool nowOn)
		{
			if (!nowOn)
			{
				return;
			}
			int i = 0;
			LeftToggles[0].transform.parent.GetComponent<ToggleGroup>().allowSwitchOff = false;
			for (int j = 0; j < LeftToggles.Length; j++)
			{
				if (LeftToggles[j].isOn)
				{
					i = j;
				}
			}
			if (_isClickCancelButton)
			{
				i = -1;
			}
			DisableEnableTemp().Forget();
			_freeDriveProvider.DragRacingControlMultiplayer.EnableOwnToggleDistacne(i);
			CancelButton.interactable = false;
			Observable.Timer(TimeSpan.FromSeconds(1.75)).Subscribe((long _) =>
			{
				CancelButton.interactable = true;
			}).AddTo(this);
		}

		public async UniTask DisableEnableTemp()
		{
			Toggle[] leftToggles = LeftToggles;
			foreach (Toggle obj in leftToggles)
			{
				obj.transform.GetChild(0).transform.GetComponent<Image>().enabled = false;
				obj.interactable = false;
			}
			await UniTask.Delay(1000);
			leftToggles = LeftToggles;
			foreach (Toggle toggle in leftToggles)
			{
				if ((bool)toggle)
				{
					toggle.transform.GetChild(0).transform.GetComponent<Image>().enabled = true;
					toggle.interactable = true;
				}
			}
		}

		public void SetWinLoseMoney(int winMoney, int loseMoney)
		{
			WinMoney.text = "+" + winMoney;
			LoseMoney.text = "-" + loseMoney;
		}

		public void DisplayFinishResult(float[] left, float[] right, DragRacingController.RunDistance runDistance, bool resultPendingOpponentPresent = false)
		{
			FillBothSideInfo(useCached: true);
			bool flag = runDistance == DragRacingController.RunDistance.QuaterMile;
			bool flag2 = left[0] == 1f;
			float num = left[1];
			float num2 = left[2];
			float num3 = left[3];
			float left2 = left[4];
			float left3 = left[5];
			bool flag3 = right != null && right[0] == 1f;
			float num4 = ((right != null) ? (right[1] * 0.001f) : 0f);
			float num5 = ((right != null) ? (right[2] * 0.001f) : 0f);
			float num6 = ((right != null) ? (right[3] * 0.001f) : 111f);
			float right2 = ((right != null) ? right[4] : 0f);
			float right3 = ((right != null) ? (right[5] * 0.001f) : 0f);
			maxSpeedTittle.text = new LocalizedString("FreeDragRacingWindow.ExitSpeed");
			if (flag)
			{
				float left4 = ((num3 < 0f) ? num2 : (num2 + num3));
				float right4 = 0f;
				if (right != null)
				{
					right4 = ((num6 < 0f) ? num5 : (num5 + num6));
				}
				SetTimeResult(left4, right4);
			}
			else
			{
				float left5 = ((num3 < 0f) ? num : (num + num3));
				float right5 = 0f;
				if (right != null)
				{
					right5 = ((num6 < 0f) ? num4 : (num4 + num6));
				}
				SetTimeResult(left5, right5);
			}
			Set100Result(left3, right3);
			Set400Result(num2, num5);
			SetMaxSpeedResult(left2, right2);
			SetReactionResult(num3, num6);
			string text = "#1";
			bool flag4 = true;
			if (num3 < 0f)
			{
				text = new LocalizedString("FreeDragRacingWindow.FalseStart");
				flag4 = false;
			}
			else if (flag2)
			{
				text = new LocalizedString("FreeDragRacingWindow.OutOfRoad");
				flag4 = false;
			}
			else if (right != null)
			{
				if (num6 < 0f)
				{
					flag4 = true;
				}
				else if (flag)
				{
					if (num2 + num3 > num5 + num6)
					{
						text = "#2";
						flag4 = false;
						if (flag3)
						{
							flag4 = true;
						}
					}
				}
				else if (num + num3 > num4 + num6)
				{
					text = "#2";
					flag4 = false;
					if (flag3)
					{
						flag4 = true;
					}
				}
			}
			if ((right == null) & resultPendingOpponentPresent & flag4)
			{
				LeftResult.text = text;
				LeftResult.color = ColorToWin;
				Resultitle.text = new LocalizedString("NewRacingMultiplayerController.WaitingOtherRacerResult");
				Resultitle.color = Color.white;
				ResultMoney.text = "";
				RightResult.text = "-";
				return;
			}
			LeftResult.text = text;
			Resultitle.text = (flag4 ? new LocalizedString("FreeDragRacingWindow.YouAreWinner") : new LocalizedString("RaceFinishUI.YouLost"));
			Resultitle.color = (flag4 ? ColorToWin : ColorToLose);
			LeftResult.color = (flag4 ? ColorToWin : ColorToLose);
			ResultMoney.text = (flag4 ? ("+" + _earnMoneyService.MultiplayerRaceWin.ToString()) : ("-" + _pricesService.MultiplayerRaceLose.ToString()));
			ResultMoney.color = (flag4 ? ColorToWin : ColorToLose);
			RecordsResults(flag4, runDistance);
			string text2 = "-";
			if (right != null)
			{
				text2 = "#2";
				if (num6 < 0f)
				{
					text2 = new LocalizedString("FreeDragRacingWindow.FalseStart");
				}
				else if (flag3)
				{
					text2 = new LocalizedString("FreeDragRacingWindow.OutOfRoad");
				}
				else if (num3 < 0f)
				{
					text2 = "#1";
				}
				else if (flag)
				{
					if (num2 + num3 > num5 + num6)
					{
						text2 = "#1";
					}
				}
				else if (num + num3 > num4 + num6)
				{
					text2 = "#1";
				}
			}
			RightResult.text = text2;
		}

		private string ConvertToMinutesSecondsMilliseconds(float seconds)
		{
			int num = (int)seconds;
			int num2 = num / 60;
			int num3 = num % 60;
			int num4 = (int)((seconds - (float)num) * 1000f);
			return $"{num2:D2}:{num3:D2}:{num4:D3}";
		}

		public void DisplayFinishResultLap(float[] left, float[] right, DragRacingController.RunDistance runDistance)
		{
			FillBothSideInfo(useCached: true);
			float num = left[1];
			float left2 = left[0];
			float num2 = ((right != null) ? (right[1] * 0.001f) : 0f);
			float right2 = ((right != null) ? right[0] : 0f);
			maxSpeedTittle.text = new LocalizedString("FreeDragRacingWindow.MaxSpeed");
			Set100Result(0f, 0f);
			Set400Result(0f, 0f);
			SetReactionResult(0f, 0f);
			SetTimeResult(num, num2);
			SetMaxSpeedResult(left2, right2);
			bool flag = true;
			if (right != null)
			{
				flag = num < num2;
			}
			string text = (flag ? "#1" : "#2");
			string text2 = (flag ? "#2" : "#1");
			LeftResult.text = text;
			Resultitle.text = (flag ? ((string)new LocalizedString("FreeDragRacingWindow.YouAreWinner")) : text);
			Resultitle.color = (flag ? ColorToWin : ColorToLose);
			LeftResult.color = (flag ? ColorToWin : ColorToLose);
			ResultMoney.text = (flag ? ("+" + _earnMoneyService.MultiplayerRaceWin.ToString()) : ("-" + _pricesService.MultiplayerRaceLose.ToString()));
			ResultMoney.color = (flag ? ColorToWin : ColorToLose);
			RecordsResults(flag, runDistance);
			RightResult.text = text2;
		}

		public void StartTimer(int time)
		{
			for (int i = 0; i < LeftToggles.Length; i++)
			{
				if (!LeftToggles[i].isOn)
				{
					LeftToggles[i].interactable = false;
					LeftTogglesTexts[i].color = Color.grey;
				}
			}
			for (int j = 0; j < RightToggles.Length; j++)
			{
				if (!RightToggles[j].isOn)
				{
					RightToggles[j].interactable = false;
					RightToggleTexts[j].color = Color.grey;
				}
			}
			TimerObject.SetActive(value: true);
			FilledImage.DOFillAmount(0f, time).OnComplete(() =>
			{
				Close();
			});
			_timer = Observable.Timer(TimeSpan.FromSeconds(1.0)).Repeat().Subscribe((long _) =>
			{
				if (time <= 0)
				{
					TimerObject.SetActive(value: false);
					StartRacing();
					_timer.Dispose();
				}
				Timer.text = time.ToString();
				time--;
			})
				.AddTo(this);
		}

		public void StartRacing()
		{
		}

		public void SetMoneyResult(int money, bool isWin)
		{
			Color color = (isWin ? ColorToMore : ColorToLose);
			string text = (isWin ? "+" : "-");
			ResultMoney.color = color;
			ResultMoney.text = text + money + "$";
		}

		public void SetTimeResult(float left, float right)
		{
			if (left == 0f && right == 0f)
			{
				LeftTime.transform.parent.gameObject.SetActive(value: false);
			}
			else if (right == 0f)
			{
				LeftTime.text = ((left > 60f) ? ConvertToMinutesSecondsMilliseconds(left) : left.ToString("F2"));
				LeftTime.color = ColorToMore;
				RightTime.text = "-";
			}
			else
			{
				bool flag = left > right;
				LeftTime.color = ((!flag) ? ColorToMore : ColorToLess);
				RightTime.color = ((!flag) ? ColorToLess : ColorToMore);
				LeftTime.text = ((left > 60f) ? ConvertToMinutesSecondsMilliseconds(left) : left.ToString("F2"));
				RightTime.text = ((right > 60f) ? ConvertToMinutesSecondsMilliseconds(right) : right.ToString("F2"));
			}
		}

		public void Set400Result(float left, float right)
		{
			if (left == 0f && right == 0f)
			{
				Left400.transform.parent.gameObject.SetActive(value: false);
			}
			else if (right == 0f)
			{
				Left400.text = left.ToString("F2");
				Left400.color = ColorToMore;
				Right400.text = "-";
			}
			else
			{
				bool flag = left > right;
				Left400.color = ((!flag) ? ColorToMore : ColorToLess);
				Right400.color = ((!flag) ? ColorToLess : ColorToMore);
				Left400.text = left.ToString("F2");
				Right400.text = right.ToString("F2");
			}
		}

		public void Set100Result(float left, float right)
		{
			if (left == 0f && right == 0f)
			{
				Left100.transform.parent.gameObject.SetActive(value: false);
			}
			else if (right == 0f)
			{
				Left100.text = left.ToString("F2");
				Left100.color = ColorToMore;
				Right100.text = "-";
			}
			else
			{
				bool flag = left > right;
				Left100.color = ((!flag) ? ColorToMore : ColorToLess);
				Right100.color = ((!flag) ? ColorToLess : ColorToMore);
				Left100.text = left.ToString("F2");
				Right100.text = right.ToString("F2");
			}
		}

		public void SetMaxSpeedResult(float left, float right)
		{
			if (left == 0f && right == 0f)
			{
				LeftMaxSpeed.transform.parent.gameObject.SetActive(value: false);
			}
			else if (right == 0f)
			{
				LeftMaxSpeed.text = left.ToString("N0");
				LeftMaxSpeed.color = ColorToMore;
				RightMaxSpeed.text = "-";
			}
			else
			{
				bool flag = left > right;
				LeftMaxSpeed.color = (flag ? ColorToMore : ColorToLess);
				RightMaxSpeed.color = (flag ? ColorToLess : ColorToMore);
				LeftMaxSpeed.text = left.ToString("N0");
				RightMaxSpeed.text = right.ToString("N0");
			}
		}

		public void SetReactionResult(float left, float right)
		{
			if (left == 0f && right == 0f)
			{
				LeftReaction.transform.parent.gameObject.SetActive(value: false);
			}
			else if (right == 111f)
			{
				LeftReaction.text = left.ToString("F2");
				LeftReaction.color = ColorToMore;
				RightReaction.text = "-";
			}
			else
			{
				bool flag = left > right;
				LeftReaction.color = ((!flag) ? ColorToMore : ColorToLess);
				RightReaction.color = ((!flag) ? ColorToLess : ColorToMore);
				LeftReaction.text = left.ToString("F2");
				RightReaction.text = right.ToString("F2");
			}
		}

		public void SetWinLoseResult(string left, string right, bool isWon)
		{
			LeftReaction.color = (isWon ? ColorToWin : ColorToLose);
			RightReaction.color = (isWon ? ColorToLose : ColorToWin);
			LeftResult.text = left;
			RightResult.text = right;
		}

		public int GetPlayerReplayToggle()
		{
			for (int i = 0; i < LeftResultToggles.Length; i++)
			{
				if (LeftResultToggles[i].isOn)
				{
					return i;
				}
			}
			return 0;
		}

		public void SetEnemyReplayToggle(int distanceId)
		{
			RightResultToggles[distanceId].isOn = true;
		}

		private void RecordsResults(bool weWon, DragRacingController.RunDistance runDistance)
		{
			if (runDistance == DragRacingController.RunDistance.Pursuit)
			{
				Debug.Log("Pursuit race, dont count");
			}
			else if (!_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.rewarded)
			{
				_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.rewarded = true;
				if (weWon)
				{
					_currencyManagerController.AddMoney(new AdvancedInt(_earnMoneyService.MultiplayerRaceWin));
					ObscuredPrefs.SetInt("RaceWin", ObscuredPrefs.GetInt("RaceWin") + 1);
				}
				else
				{
					AdvancedInt price = new AdvancedInt(_pricesService.MultiplayerRaceLose);
					_currencyManagerController.TryMoneySpend(out var _, price, showShop: false, spendAnyway: true);
					ObscuredPrefs.SetInt("RaceLose", ObscuredPrefs.GetInt("RaceLose") + 1);
				}
				Money.text = CurrencyManagerController.Money.ToString();
				if ((int)CurrencyManagerController.Money < (int)_pricesService.MultiplayerRaceLose)
				{
					LeftResultToggles[0].graphic = null;
				}
				if (CheckCarAfterRace() && weWon && !UtilsVehicles.IsBlackVehicle() && runDistance != DragRacingController.RunDistance.Pursuit)
				{
					++_freeDriveProvider.FreeDriveDB.dragWins;
					_freeDriveProvider.FreeDriveDB.SendRating(toMainMenu: false).Forget();
				}
			}
		}

		private bool CheckCarAfterRace()
		{
			if (!V.Car)
			{
				return false;
			}
			return true;
		}
	}
}
