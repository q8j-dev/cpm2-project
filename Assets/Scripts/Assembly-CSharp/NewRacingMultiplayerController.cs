using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Steam.Base;

public class NewRacingMultiplayerController : MonoBehaviourPunCallbacks
{
	public class EnemyInfo
	{
		public string PlayerName;

		public int Rank;

		public int CarId;

		public float MaxPower;

		public float MaxTorque;

		public EnemyInfo(string playerName, int rank, int carId, float maxPower, float maxTorque)
		{
			PlayerName = playerName;
			Rank = rank;
			CarId = carId;
			MaxPower = maxPower;
			MaxTorque = maxTorque;
		}
	}

	private const float PhotoFinishGapSeconds = 0.25f;

	public bool multiPlayerRacingNow;

	public DragRacingMultiplayerTree CurrentRacingTree;

	public JoinDragRacingTrigger lastOwnTrigger;

	public List<DragRacingMultiplayerTree> TreeList;

	public List<MultiplayerRaceLocationConfig> raceLocationConfigs;

	public bool testRun;

	public DragRacingMultiplayerTree.PositionOnTree ourPositionOnTree;

	public DragRacingMultiplayerTree.PositionOnTree otherCarPositionOnTree;

	[HideInInspector]
	public bool recordFinishResult;

	public DragRacingController.RunDistance runDistance = DragRacingController.RunDistance.OneMile;

	public List<JoinDragRacingTrigger> listTriggers;

	[CanBeNull]
	public int[] otherCarResult;

	public bool OtherCarResultReceived;

	public Button exitRace;

	public float angleCheck = 5f;

	public bool fromTrigger;

	private bool updateAfterOtherReceived;

	public TextMeshProUGUI racingTimerLabel;

	public TextMeshProUGUI lapCountLabel;

	public ObscuredFloat fineToAdd;

	public EnemyInfo cachedLastEnemy;

	public bool rewarded;

	public podium Podium;

	private FreeDriveControl _freeDriveControl;

	private InputItemsProvider _controlItemsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private CancellationTokenSource _cancellationTokenSourceEnablePropsForRacingPlace;

	private CarInputManager _carInputManager;

	private bool _opponentCancelledMidRace;

	private bool _opponentForfeitedAfterFinish;

	private Player _opponentPlayer;

	private int[] _ownFinishResult;

	private FreeTimeAttackController _timeAttackController;

	private bool _lastPvpRaceWasLoss;

	private bool _replayFollowsLoss;

	private bool _pvpResultEvaluated;

	private bool _pendingResultValid;

	private float _pendingResultReactionTime;

	private Dragy _pendingResultDragy;

	private VehicleData _pendingResultVehicleData;

	private DragRacingController.RunDistance _pendingResultDistance;

	private ISteamAchievementService _steamAchievementService;

	private static float EffectivePhotoFinishGapSeconds => 0.25f;

	public int LastResolvedRaceIndex { get; private set; } = -1;

	public DragRacingController.RunDistance[] CurrentRunDistances
	{
		get
		{
			if (!CurrentRacingTree)
			{
				return null;
			}
			return CurrentRacingTree.GetDistinctRaceTypes();
		}
	}

	public V.placeToSpawn CurrentRacePlace
	{
		get
		{
			if (!CurrentRacingTree)
			{
				return V.placeToSpawn.House;
			}
			return CurrentRacingTree.PlaceToSpawn;
		}
	}

	[Inject]
	public void Construct(FreeDriveControl freeDriveControl, InputItemsProvider controlItemsProvider, FreeDriveProvider freeDriveProvider, CarInputManager carInputManager, FreeTimeAttackController timeAttackController, ISteamAchievementService steamAchievementService)
	{
		_steamAchievementService = steamAchievementService;
		_timeAttackController = timeAttackController;
		_freeDriveProvider = freeDriveProvider;
		_controlItemsProvider = controlItemsProvider;
		_freeDriveControl = freeDriveControl;
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		int num = 0;
		foreach (JoinDragRacingTrigger listTrigger in listTriggers)
		{
			if ((bool)listTrigger)
			{
				listTrigger.newRacingMultiplayerController = this;
				listTrigger.id = num;
				num++;
				listTrigger.SetActive(active: false);
			}
		}
		if (raceLocationConfigs != null)
		{
			foreach (MultiplayerRaceLocationConfig raceLocationConfig in raceLocationConfigs)
			{
				if (!(raceLocationConfig == null) && !(raceLocationConfig.Tree == null))
				{
					raceLocationConfig.Tree.Races = raceLocationConfig.Races;
					raceLocationConfig.Tree.PlaceToSpawn = raceLocationConfig.PlaceToSpawn;
					if (TreeList == null)
					{
						TreeList = new List<DragRacingMultiplayerTree>();
					}
					if (!TreeList.Contains(raceLocationConfig.Tree))
					{
						TreeList.Add(raceLocationConfig.Tree);
					}
				}
			}
		}
		if (TreeList != null)
		{
			foreach (DragRacingMultiplayerTree tree in TreeList)
			{
				if ((bool)tree)
				{
					tree.controller = this;
				}
			}
		}
		if ((bool)exitRace)
		{
			exitRace.onClick.AddListener(ExitRace);
		}
	}

	private void OnDestroy()
	{
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceEnablePropsForRacingPlace);
	}

	public void EnablePropsForRacingPlace(DragRacingMultiplayerTree currentTree)
	{
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceEnablePropsForRacingPlace);
		_cancellationTokenSourceEnablePropsForRacingPlace = new CancellationTokenSource();
		EnablePropsForRacingPlaceAsync(currentTree, _cancellationTokenSourceEnablePropsForRacingPlace.Token).Forget();
	}

	private async UniTaskVoid EnablePropsForRacingPlaceAsync(DragRacingMultiplayerTree currentTree, CancellationToken cancellationToken)
	{
		await UniTask.WaitForFixedUpdate();
		if (cancellationToken.IsCancellationRequested)
		{
			return;
		}
		EnablePropsForRacingPlace(state: false);
		foreach (DragRacingMultiplayerTree.PropsToEnableOnRacing item in currentTree.propsToEnable)
		{
			if (item.indexRunDistance != _freeDriveProvider.DragRacingControlMultiplayer.SelectedDistance)
			{
				continue;
			}
			foreach (GameObject prop in item.props)
			{
				if ((bool)prop)
				{
					prop.SetActive(value: true);
				}
			}
		}
		GlobalEventSystem.InvokeDragRacingPropsActivate(state: true, _freeDriveProvider.DragRacingControlMultiplayer.SelectedDistance, currentTree.gameObject.name);
	}

	public void EnablePropsForRacingPlace(bool state)
	{
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceEnablePropsForRacingPlace);
		foreach (DragRacingMultiplayerTree tree in TreeList)
		{
			if (tree == null)
			{
				continue;
			}
			foreach (DragRacingMultiplayerTree.PropsToEnableOnRacing item in tree.propsToEnable)
			{
				foreach (GameObject prop in item.props)
				{
					if ((bool)prop)
					{
						prop.SetActive(state);
					}
				}
				if (state)
				{
					return;
				}
				SwitchStateInnerTree(state: false, tree);
			}
		}
		GlobalEventSystem.InvokeDragRacingPropsActivate(state: false, -1);
	}

	private static void SwitchStateInnerTree(bool state, DragRacingMultiplayerTree tree)
	{
		if (tree.otherDistanceTree == null)
		{
			return;
		}
		foreach (DragRacingMultiplayerTree.PropsToEnableOnRacing item in tree.otherDistanceTree.propsToEnable)
		{
			foreach (GameObject prop in item.props)
			{
				if ((bool)prop)
				{
					prop.SetActive(state);
				}
			}
		}
	}

	private void Reset()
	{
		if ((bool)CurrentRacingTree)
		{
			CurrentRacingTree.ResetTree();
			CurrentRacingTree.SetDefaultActiveSelf();
			_timeAttackController.TryShowHiddenStartTriggers();
			CurrentRacingTree.ownCar = null;
			CurrentRacingTree.otherCar = null;
		}
		CurrentRacingTree = null;
		otherCarResult = null;
		OtherCarResultReceived = false;
		updateAfterOtherReceived = false;
		ClearPendingPvpResult();
	}

	public void DisableAllCheckPoints()
	{
		foreach (DragRacingMultiplayerTree tree in TreeList)
		{
			if ((bool)tree)
			{
				tree.DisableAllCheckPoints();
				if ((bool)tree.otherDistanceTree)
				{
					tree.otherDistanceTree.DisableAllCheckPoints();
				}
			}
		}
	}

	public void OnBothCarOnTrigger()
	{
		if (!(CurrentRacingTree.carWaitingNewRace[0] == null))
		{
			CurrentRacingTree.otherCar = CurrentRacingTree.carWaitingNewRace[0];
			otherCarPositionOnTree = CurrentRacingTree.carWaitingNewRace[1].GetComponent<JoinDragRacingTrigger>().PositionOnTree;
			PhotonView component = CurrentRacingTree.otherCar.GetComponent<PhotonView>();
			_freeDriveControl.LastInvitationReceivedId = component.Owner.ActorNumber;
			_freeDriveProvider.RacingCar2 = CurrentRacingTree.otherCar;
			_freeDriveControl.ShowNegotiationWindow();
			fromTrigger = true;
		}
	}

	public void SetDistance()
	{
		int selectedDistance = _freeDriveProvider.DragRacingControlMultiplayer.SelectedDistance;
		LastResolvedRaceIndex = -1;
		if (CurrentRacingTree != null)
		{
			if (CurrentRacingTree.Races != null && CurrentRacingTree.Races.Count > 0)
			{
				int resolved = CurrentRacingTree.ResolveRaceIndex(selectedDistance, InviterPosition());
				ApplyResolvedRaceIndex(resolved);
			}
			else if (CurrentRacingTree.RacingRunDistances != null && selectedDistance >= 0 && selectedDistance < CurrentRacingTree.RacingRunDistances.Length)
			{
				runDistance = CurrentRacingTree.RacingRunDistances[selectedDistance];
			}
		}
	}

	public void ApplyResolvedRaceIndex(int resolved)
	{
		if (!(CurrentRacingTree == null) && CurrentRacingTree.Races != null && resolved >= 0 && resolved < CurrentRacingTree.Races.Count)
		{
			LastResolvedRaceIndex = resolved;
			CurrentRacingTree.ApplyRaceEntry(resolved);
			runDistance = CurrentRacingTree.Races[resolved].Type;
		}
	}

	private Vector3 InviterPosition()
	{
		if (_freeDriveControl.InvitationHost || _freeDriveProvider.RacingCar2 == null)
		{
			return V.DriverPerson.transform.position;
		}
		return _freeDriveProvider.RacingCar2.transform.position;
	}

	public void OnRaceStarted()
	{
		CurrentRacingTree.ownCar = V.Car;
		CurrentRacingTree.ResetTree();
		if (!fromTrigger)
		{
			MonoBehaviour.print(_freeDriveProvider.RacingCar2?.ToString() + " " + CurrentRacingTree.otherCar);
			CurrentRacingTree.otherCar = _freeDriveProvider.RacingCar2;
			if (_freeDriveControl.InvitationHost)
			{
				otherCarPositionOnTree = DragRacingMultiplayerTree.PositionOnTree.right;
				ourPositionOnTree = DragRacingMultiplayerTree.PositionOnTree.left;
			}
			else
			{
				otherCarPositionOnTree = DragRacingMultiplayerTree.PositionOnTree.left;
				ourPositionOnTree = DragRacingMultiplayerTree.PositionOnTree.right;
			}
		}
		rewarded = false;
		_opponentCancelledMidRace = false;
		_opponentForfeitedAfterFinish = false;
		if ((bool)CurrentRacingTree.otherCar)
		{
			setOurCarRotPos();
			otherCarResult = null;
			OtherCarResultReceived = false;
			_ownFinishResult = null;
			_opponentPlayer = (CurrentRacingTree.otherCar.TryGetComponent<PhotonView>(out var component) ? component.Controller : null);
			ClearPendingPvpResult();
			SetupLaneSteering();
			CurrentRacingTree.OnRaceStarted();
			V.CoreGameManager.SetMessage(new LocalizedString("NewRacingMultiplayerController.RaceWillStart"), 3f);
			UpdateColliderMeshRenderer(on: false);
			TextMeshProUGUI mono = racingTimerLabel;
			DragRacingController.RunDistance runDistance = this.runDistance;
			mono.SetActive(runDistance == DragRacingController.RunDistance.Lap || runDistance == DragRacingController.RunDistance.Rally);
			racingTimerLabel.text = "";
			fineToAdd = 0f;
			_freeDriveControl.DisableToRoadButton();
			_freeDriveProvider.QuickCarSettings.DisableKeepLane(message: true);
		}
	}

	private void Update()
	{
		multiPlayerRacingNow = FlagsStorage.ChasingRaceModeStarted;
	}

	private void FixedUpdate()
	{
		if ((bool)CurrentRacingTree)
		{
			CurrentRacingTree.CheckCarsMove();
		}
	}

	public void OnMissedCheckPoint()
	{
		fineToAdd = (float)fineToAdd + 30f;
		V.CoreGameManager.SetMessage(new LocalizedString("NewRacingMultiplayerController.MissedCheckPointN30sec"), 3f);
	}

	public void OnCarEnterRacingTrigger(GameObject car, JoinDragRacingTrigger trigger)
	{
	}

	public void OnCarExitRacingTrigger(GameObject car, DragRacingMultiplayerTree.PositionOnTree pos, JoinDragRacingTrigger trigger)
	{
		if (car != V.LastUsedCar)
		{
			return;
		}
		DragRacingMultiplayerTree tree = trigger.tree;
		if (!(tree.ownCar != car))
		{
			int viewID = car.GetComponent<PhotonView>().ViewID;
			int[] data = new int[2] { viewID, trigger.id };
			CallRPC(null, 1, data);
			if (!FlagsStorage.ChasingRaceModeStarted)
			{
				tree.ownCar = null;
				CurrentRacingTree = null;
				ourPositionOnTree = DragRacingMultiplayerTree.PositionOnTree.None;
			}
		}
	}

	private void OnOtherCarEnteredRacingTrigger(GameObject car, int idTrigger)
	{
	}

	private void OnOtherCarExitRacingTrigger(GameObject car, int idTrigger)
	{
	}

	private void CallRPC(Player target, int eventIndex, int[] data)
	{
		if (target == null)
		{
			MonoBehaviour.print("target is null " + eventIndex);
			base.photonView.RpcToDefaultGroup("MultiplayerRacingEvents", RpcTarget.Others, (byte)eventIndex, data);
		}
		else
		{
			MonoBehaviour.print(target?.ToString() + " " + eventIndex);
			base.photonView.RPC("MultiplayerRacingEvents", target, (byte)eventIndex, data);
		}
	}

	[PunRPC]
	private void MultiplayerRacingEvents(byte eventID, int[] data)
	{
		string text = eventID + " ";
		if (data != null)
		{
			foreach (int num in data)
			{
				text = text + " " + num;
			}
		}
		MonoBehaviour.print("racing rpc received: " + text);
		racingEvents(eventID, data);
	}

	private void racingEvents(byte eventID, int[] data)
	{
		switch (eventID)
		{
		case 0:
		{
			PhotonView photonView = PhotonView.Find(data[0]);
			if ((bool)photonView)
			{
				int idTrigger = data[1];
				OnOtherCarEnteredRacingTrigger(photonView.gameObject, idTrigger);
			}
			break;
		}
		case 1:
		{
			PhotonView photonView2 = PhotonView.Find(data[0]);
			if ((bool)photonView2)
			{
				int idTrigger2 = data[1];
				OnOtherCarExitRacingTrigger(photonView2.gameObject, idTrigger2);
			}
			break;
		}
		case 2:
			if ((bool)CurrentRacingTree)
			{
				CurrentRacingTree.onOtherCarStaged();
			}
			break;
		case 3:
			if ((bool)CurrentRacingTree)
			{
				CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
				CurrentRacingTree.willTurnOnGreen(sendForOther: false, cancellationTokenOnDestroy).Forget();
			}
			break;
		case 4:
			if ((bool)CurrentRacingTree)
			{
				OnFinishResultReceivedOtherCar(data);
			}
			break;
		case 6:
			if (_ownFinishResult != null)
			{
				CallRPC(ResolveOpponentPlayer(), 4, _ownFinishResult);
			}
			break;
		case 5:
			break;
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if ((bool)CurrentRacingTree && CurrentRacingTree.ownCar == V.LastUsedCar)
		{
			SendOtherOwnTriggerState(newPlayer);
		}
	}

	private async UniTask SendOtherOwnTriggerState(Player newPlayer)
	{
		int num;
		if (num == 0)
		{
			UniTask.Awaiter awaiter2 = default;
			UniTask.Awaiter awaiter = awaiter2;
			awaiter.GetResult();
			if (!(this == null) && (bool)CurrentRacingTree && CurrentRacingTree.ownCar == V.LastUsedCar)
			{
				int viewID = V.LastUsedCar.GetComponent<PhotonView>().ViewID;
				int[] data = new int[2] { viewID, lastOwnTrigger.id };
				CallRPC(newPlayer, 0, data);
			}
		}
	}

	private void OnFinishResultReceivedOtherCar(int[] result)
	{
		VehicleData component = V.LastUsedCar.GetComponent<VehicleData>();
		otherCarResult = result;
		OtherCarResultReceived = true;
		if (component.racingState != RacingState.Finished)
		{
			return;
		}
		EvaluatePvpResult();
		if (updateAfterOtherReceived && _freeDriveProvider.RacingCar2 != null)
		{
			Dragy component2 = V.LastUsedCar.GetComponent<Dragy>();
			if (runDistance == DragRacingController.RunDistance.Lap || runDistance == DragRacingController.RunDistance.Rally)
			{
				ShowResultUILap(component2, otherCarResult);
			}
			else
			{
				ShowResultUI(component2, otherCarResult);
			}
		}
	}

	private void RecordPvpResult(bool playerWon)
	{
		if (playerWon && _replayFollowsLoss)
		{
			_steamAchievementService?.Unlock("ACH_RUN_IT_BACK");
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQLg", "pvp_race");
		}
		_lastPvpRaceWasLoss = !playerWon;
		_replayFollowsLoss = false;
	}

	private (float PlayerTotal, float OtherTotal, bool OtherInvalid) GetPvpTotals(Dragy dragy, float ourReactionTime)
	{
		bool flag = runDistance == DragRacingController.RunDistance.QuaterMile;
		float item = dragy.GetResult(flag) + ourReactionTime;
		bool flag2 = otherCarResult[0] == 1;
		float num = (float)otherCarResult[2] * 0.001f;
		float num2 = ((flag || otherCarResult.Length <= 6) ? ((float)otherCarResult[1] * 0.001f) : ((float)otherCarResult[6] * 0.001f));
		float item2 = ((num < 0f) ? num2 : (num2 + num));
		return (PlayerTotal: item, OtherTotal: item2, OtherInvalid: flag2 || num < 0f);
	}

	public void NotifyReplayRestart()
	{
		_replayFollowsLoss = _lastPvpRaceWasLoss;
	}

	private void ReportPvpFinishedForSteam()
	{
		int current = _steamAchievementService.GetStatInt("steam_pvp_finished") + 1;
		_steamAchievementService.ReportProgress("ACH_FIRST_CHALLENGE", "steam_pvp_finished", 1, current);
	}

	public async UniTask OnCarFinish(GameObject car)
	{
		bool winner = true;
		Dragy dragy = car.GetComponent<Dragy>();
		VehicleData vehicleData = car.GetComponent<VehicleData>();
		car.GetComponent<Rigidbody>();
		_carInputManager.State = 1;
		vehicleData.racingState = RacingState.Finished;
		ReportPvpFinishedForSteam();
		float RTOur = dragy.reactionTime;
		if (RTOur < 0f || dragy.reason == Dragy.LostReason.outOfRoad)
		{
			winner = false;
		}
		_ownFinishResult = GenerateDataResult(dragy, runDistance);
		CallRPC(ResolveOpponentPlayer(), 4, _ownFinishResult);
		int waitCount = 0;
		if (!OtherCarResultReceived)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("NewRacingMultiplayerController.WaitingOtherRacerResult"), 5f);
		}
		updateAfterOtherReceived = false;
		_freeDriveProvider.QuickCarSettings.SetGearSelf(3);
		while (!OtherCarResultReceived && waitCount <= 10 && !(_freeDriveProvider.RacingCar2 == null))
		{
			await UniTask.Delay(1000);
			if (!FlagsStorage.RaceStarted && !_opponentForfeitedAfterFinish)
			{
				return;
			}
			waitCount++;
			if (!OtherCarResultReceived && waitCount % 3 == 0)
			{
				CallRPC(ResolveOpponentPlayer(), 6, null);
			}
		}
		V.CoreGameManager.SetMessage("", 0f);
		GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQIg", "pvp_race");
		_pendingResultDragy = dragy;
		_pendingResultVehicleData = vehicleData;
		_pendingResultReactionTime = RTOur;
		_pendingResultValid = winner;
		_pendingResultDistance = runDistance;
		_pvpResultEvaluated = false;
		if (OtherCarResultReceived)
		{
			EvaluatePvpResult();
			ShowResultUI(dragy, otherCarResult);
		}
		else
		{
			ShowResultUI(dragy, otherCarResult);
			updateAfterOtherReceived = true;
		}
	}

	private void ClearPendingPvpResult()
	{
		_pendingResultDragy = null;
		_pendingResultVehicleData = null;
		_pvpResultEvaluated = false;
	}

	private void EvaluatePvpResult()
	{
		if (_pvpResultEvaluated || !OtherCarResultReceived || _pendingResultDragy == null || _pendingResultDistance != runDistance || otherCarResult == null || otherCarResult.Length < 3)
		{
			return;
		}
		_pvpResultEvaluated = true;
		if (!_pendingResultValid)
		{
			RecordPvpResult(playerWon: false);
			return;
		}
		(float, float, bool) pvpTotals = GetPvpTotals(_pendingResultDragy, _pendingResultReactionTime);
		bool flag = pvpTotals.Item3 || pvpTotals.Item1 < pvpTotals.Item2;
		if (flag)
		{
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQNg", "pvp_race");
			GooglePlayAchievements.TryUnlockDealerLuck((!_pendingResultVehicleData) ? null : _pendingResultVehicleData.carInfo?.CarInstanceId, "pvp_race");
			_steamAchievementService?.Unlock("ACH_CLEAN_RIVALRY");
			if (_pendingResultVehicleData != null && _pendingResultVehicleData.carInfo != null)
			{
				_steamAchievementService?.TryUnlockDealerLuck(_pendingResultVehicleData.carInfo.CarID);
			}
		}
		if (!pvpTotals.Item3 && pvpTotals.Item1 < pvpTotals.Item2 && pvpTotals.Item2 - pvpTotals.Item1 <= EffectivePhotoFinishGapSeconds)
		{
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQNw", "pvp_race");
			_steamAchievementService?.Unlock("ACH_PHOTO_FINISH");
		}
		RecordPvpResult(flag);
	}

	public async UniTask OnCarFinishLapRace(GameObject car)
	{
		Dragy dragy = car.GetComponent<Dragy>();
		dragy.CheckMaxSpeed = false;
		dragy.lapTime = Time.time - CurrentRacingTree.startTime + (float)fineToAdd;
		ReportPvpFinishedForSteam();
		car.GetComponent<Rigidbody>().velocity *= 0.6f;
		_freeDriveProvider.QuickCarSettings.SetGearSelf(3);
		if ((bool)CurrentRacingTree.otherCar)
		{
			Player controller = CurrentRacingTree.otherCar.GetComponent<PhotonView>().Controller;
			int[] data = GenerateDataResult(dragy, runDistance);
			CallRPC(controller, 4, data);
		}
		int waitCount = 0;
		updateAfterOtherReceived = false;
		float timeToWait = 10 + (int)(float)fineToAdd;
		if (!OtherCarResultReceived)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("NewRacingMultiplayerController.WaitingOtherCarResult"), (int)timeToWait);
		}
		while (!OtherCarResultReceived && (float)waitCount <= timeToWait)
		{
			MonoBehaviour.print(waitCount + " " + _freeDriveProvider.RacingCar2);
			if (_freeDriveProvider.RacingCar2 == null)
			{
				break;
			}
			await UniTask.Delay(1000);
			if (!FlagsStorage.RaceStarted && !_opponentForfeitedAfterFinish)
			{
				return;
			}
			waitCount++;
		}
		V.CoreGameManager.SetMessage("", 0f);
		if (OtherCarResultReceived)
		{
			ShowResultUILap(dragy, otherCarResult);
			return;
		}
		updateAfterOtherReceived = true;
		ShowResultUILap(dragy, null);
	}

	private int[] GenerateDataResult(Dragy dragy, DragRacingController.RunDistance runDistance)
	{
		int num = ((dragy.reason == Dragy.LostReason.outOfRoad) ? 1 : 0);
		switch (runDistance)
		{
		case DragRacingController.RunDistance.QuaterMile:
		{
			int num10 = (int)Math.Round(dragy.quarterMileResult * 1000f);
			int num11 = (int)Math.Round(dragy.reactionTime * 1000f);
			int num12 = (int)Math.Round(dragy.kmh100Result * 1000f);
			int num13 = (int)Math.Round(dragy.et1_8 * 1000f);
			int num14 = (int)Math.Round(dragy.exitSpeed);
			return new int[6] { num, num10, num11, num12, num13, num14 };
		}
		case DragRacingController.RunDistance.OneMile:
		case DragRacingController.RunDistance.HalfMile:
		{
			int num4 = (int)Math.Round(dragy.quarterMileResult * 1000f);
			int num5 = (int)Math.Round(dragy.reactionTime * 1000f);
			int num6 = (int)Math.Round(dragy.kmh100Result * 1000f);
			int num7 = (int)Math.Round(dragy.et1_8 * 1000f);
			int num8 = (int)Math.Round(dragy.exitSpeed);
			int num9 = (int)Math.Round(dragy.halfMileResult * 1000f);
			return new int[7] { num, num4, num5, num6, num7, num8, num9 };
		}
		default:
		{
			int num2 = (int)Math.Round(dragy.lapTime * 1000f);
			int num3 = (int)Math.Round(dragy.maxSpeed);
			return new int[2] { num3, num2 };
		}
		}
	}

	private async UniTask ShowResultUILap(Dragy ourResult, int[] otherCarResult)
	{
		if (_opponentCancelledMidRace)
		{
			return;
		}
		if (_freeDriveControl.newNegotiationWindow == null)
		{
			V.CoreGameManager.ManagerUI.ShowFreeDragRacingWindow();
			while (_freeDriveControl.newNegotiationWindow == null)
			{
				await UniTask.Delay(10);
			}
		}
		_freeDriveControl.newNegotiationWindow.ShowResult();
		_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
		float[] left = new float[2] { ourResult.maxSpeed, ourResult.lapTime };
		if (OtherCarResultReceived)
		{
			float[] right = new float[2]
			{
				otherCarResult[0],
				otherCarResult[1]
			};
			_freeDriveControl.newNegotiationWindow.DisplayFinishResultLap(left, right, DragRacingController.RunDistance.Lap);
		}
		else
		{
			_freeDriveControl.newNegotiationWindow.DisplayFinishResultLap(left, null, DragRacingController.RunDistance.Lap);
		}
		_freeDriveControl.ShowFinishMenu();
	}

	private async UniTask ShowResultUI(Dragy ourResult, int[] otherCarResult)
	{
		Debug.Log("Show result UI " + ourResult);
		if (_opponentCancelledMidRace)
		{
			return;
		}
		if (_freeDriveControl.newNegotiationWindow == null)
		{
			V.CoreGameManager.ManagerUI.ShowFreeDragRacingWindow();
			while (_freeDriveControl.newNegotiationWindow == null)
			{
				await UniTask.Delay(10);
			}
		}
		_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
		_freeDriveControl.newNegotiationWindow.ShowResult();
		float num = ((ourResult.reason == Dragy.LostReason.outOfRoad) ? 1 : 0);
		float[] left = new float[6] { num, ourResult.halfMileResult, ourResult.quarterMileResult, ourResult.reactionTime, ourResult.exitSpeed, ourResult.kmh100Result };
		if (OtherCarResultReceived)
		{
			float num2 = ((runDistance == DragRacingController.RunDistance.OneMile || runDistance == DragRacingController.RunDistance.HalfMile) ? otherCarResult[6] : 0);
			float[] right = new float[6]
			{
				otherCarResult[0],
				num2,
				otherCarResult[1],
				otherCarResult[2],
				otherCarResult[5],
				otherCarResult[3]
			};
			_freeDriveControl.newNegotiationWindow.DisplayFinishResult(left, right, runDistance);
		}
		else
		{
			bool resultPendingOpponentPresent = _freeDriveProvider.RacingCar2 != null;
			_freeDriveControl.newNegotiationWindow.DisplayFinishResult(left, null, runDistance, resultPendingOpponentPresent);
		}
		_freeDriveControl.ShowFinishMenu();
	}

	private Player ResolveOpponentPlayer()
	{
		if ((bool)CurrentRacingTree && (bool)CurrentRacingTree.otherCar && CurrentRacingTree.otherCar.TryGetComponent<PhotonView>(out var component))
		{
			return component.Controller;
		}
		return _opponentPlayer;
	}

	public void OnOurCarStaged()
	{
		CallRPC(ResolveOpponentPlayer(), 2, null);
	}

	public void OnGreenLightWillOn()
	{
		CallRPC(ResolveOpponentPlayer(), 3, null);
	}

	public void outOfTriggerBeforeStage()
	{
		Player controller = CurrentRacingTree.otherCar.GetComponent<PhotonView>().Controller;
		_freeDriveProvider.MultiPlayer.SendValueStrToPlayer(44, "", "", controller);
		OnRaceFinished();
		V.CoreGameManager.SetMessage(new LocalizedString("NewRacingMultiplayerController.YouFreakedOutRaceCancelled"), 5f);
	}

	public void OtherCarCancelledRaceDuringRun()
	{
		if (!(_opponentForfeitedAfterFinish = (bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<VehicleData>(out var component) && component.racingState == RacingState.Finished))
		{
			_opponentCancelledMidRace = true;
			rewarded = true;
			OtherCarResultReceived = false;
			updateAfterOtherReceived = false;
			otherCarResult = null;
			ClearPendingPvpResult();
		}
		OnRaceFinished();
	}

	public void OtherCarCancelledRace()
	{
		if ((bool)CurrentRacingTree && CurrentRacingTree.GreenLightWillOn)
		{
			return;
		}
		OnRaceFinished();
		if (!(lastOwnTrigger == null))
		{
			bool num = lastOwnTrigger.CheckCarInsideTrigger(V.LastUsedCar);
			bool flag = lastOwnTrigger.currentCar == V.Car || lastOwnTrigger == null;
			if (num & flag)
			{
				OnCarEnterRacingTrigger(V.LastUsedCar, lastOwnTrigger);
			}
		}
	}

	private void SetupLaneSteering()
	{
		if ((bool)_carInputManager)
		{
			_carInputManager.IgnoreSteerControl(state: false);
		}
	}

	private void ClearLaneSteering()
	{
		if ((bool)_carInputManager)
		{
			if (_carInputManager.KeepRowComposition != null)
			{
				_carInputManager.KeepRowComposition.FollowOnRacing = null;
			}
			_carInputManager.IgnoreSteerControl(state: false);
		}
	}

	public void OnRaceFinished()
	{
		FlagsStorage.ChasingRaceModeStarted = false;
		UpdateColliderMeshRenderer(on: true);
		ClearLaneSteering();
		Reset();
		_freeDriveProvider.DragRacingControlMultiplayer.FinishRace();
		if ((bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<Dragy>(out var component))
		{
			component.maxSpeed = 0f;
			component.CheckMaxSpeed = false;
		}
		racingTimerLabel.SetActive(active: false);
		racingTimerLabel.text = "";
		lapCountLabel.SetActive(active: false);
		_freeDriveControl.EnableToRoadButton();
		_freeDriveProvider.QuickCarSettings.SetGearSelf(4);
	}

	public void ExitRace()
	{
		OnRaceFinished();
	}

	public bool AreLookingInSameDirection(Transform ObjectA, Transform ObjectB, float angleTolerance = 30f)
	{
		return Mathf.Acos(Vector3.Dot(ObjectA.forward, ObjectB.forward)) * 57.29578f < angleTolerance;
	}

	private void setOutCarRot()
	{
		if (ourPositionOnTree == DragRacingMultiplayerTree.PositionOnTree.left)
		{
			if (!AreLookingInSameDirection(V.Car.transform, CurrentRacingTree.leftCarPos, angleCheck))
			{
				V.Car.transform.rotation = CurrentRacingTree.leftCarPos.rotation;
			}
		}
		else if (!AreLookingInSameDirection(V.Car.transform, CurrentRacingTree.RightCarPos, angleCheck))
		{
			V.Car.transform.rotation = CurrentRacingTree.RightCarPos.rotation;
		}
	}

	private void setOurCarRotPos()
	{
		if (ourPositionOnTree == DragRacingMultiplayerTree.PositionOnTree.left)
		{
			if (Vector3.Distance(V.Car.transform.position, CurrentRacingTree.leftCarPos.position) > 1f)
			{
				V.Car.transform.position = CurrentRacingTree.leftCarPos.position;
			}
		}
		else if (Vector3.Distance(V.Car.transform.position, CurrentRacingTree.RightCarPos.position) > 1f)
		{
			V.Car.transform.position = CurrentRacingTree.RightCarPos.position;
		}
		setOutCarRot();
	}

	private void UpdateColliderMeshRenderer(bool on)
	{
		foreach (JoinDragRacingTrigger listTrigger in listTriggers)
		{
			if (!(listTrigger == null) && !(listTrigger.render == null))
			{
				if (on)
				{
					listTrigger.render.enabled = listTrigger.currentCar == null;
				}
				else
				{
					listTrigger.render.enabled = false;
				}
			}
		}
	}

	private V.placeToSpawn currentPlace()
	{
		return V.SceneControl.getEnvironmentByColliders(V.DriverPerson.transform.position) switch
		{
			0 => V.placeToSpawn.MainCity, 
			1 => V.placeToSpawn.Highway, 
			2 => V.placeToSpawn.Desert, 
			3 => V.placeToSpawn.Mountain, 
			4 => V.placeToSpawn.Racing, 
			5 => V.placeToSpawn.City2, 
			11 => V.placeToSpawn.Offroad, 
			13 => V.placeToSpawn.JapanSerpentine, 
			14 => V.placeToSpawn.JapanSuburban, 
			_ => V.placeToSpawn.House, 
		};
	}

	public int getClosestTree(Vector3 targetPoint)
	{
		V.placeToSpawn placeToSpawn = currentPlace();
		int num = 0;
		List<DragRacingMultiplayerTree> treeList = TreeList;
		if (placeToSpawn != V.placeToSpawn.House)
		{
			foreach (DragRacingMultiplayerTree tree in TreeList)
			{
				if (tree == null)
				{
					num++;
					continue;
				}
				if (tree.PlaceToSpawn == placeToSpawn)
				{
					if (tree.PlaceToSpawn == V.placeToSpawn.MainCity)
					{
						break;
					}
					return num;
				}
				num++;
			}
		}
		float num2 = float.MaxValue;
		int num3 = 0;
		num = 0;
		foreach (DragRacingMultiplayerTree item in treeList)
		{
			if (item == null || item.StartCollider == null)
			{
				num3++;
				continue;
			}
			Vector3 position = item.StartCollider.transform.position;
			float num4 = Vector3.SqrMagnitude(targetPoint - position);
			if (num4 < num2)
			{
				num2 = num4;
				num = num3;
			}
			num3++;
		}
		return num;
	}
}
