using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Steam.Base;

public class LemanRaceController : MonoBehaviour
{
	private RaceMapModel _raceMapModel;

	[SerializeField]
	private CircuitRacingUIControl CircuitRacingUIControl;

	[SerializeField]
	private CircuitRaceControlller CircuitRacingController;

	private int _secoundsLeft;

	private ManagerUI _managerUI;

	private ISteamAchievementService _steamAchievementService;

	private CancellationTokenSource _countdownCts;

	[SerializeField]
	private CircuitAiCarStateChanger[] LemanAiCarStateChangers;

	[SerializeField]
	private PitStopTrigger[] PitStopTriggers;

	[Inject]
	public void Constuct(ManagerUI managerUI, ISteamAchievementService steamAchievementService)
	{
		_managerUI = managerUI;
		_steamAchievementService = steamAchievementService;
	}

	public void Init(RaceMapModel raceMapModel)
	{
		_raceMapModel = raceMapModel;
	}

	public void InitPitStopTriggers()
	{
		if (LemanAiCarStateChangers != null)
		{
			for (int i = 0; i < LemanAiCarStateChangers.Length; i++)
			{
				CircuitAiCarStateChanger circuitAiCarStateChanger = LemanAiCarStateChangers[i];
				if (!(circuitAiCarStateChanger == null))
				{
					circuitAiCarStateChanger.CheckDirection();
				}
			}
		}
		if (PitStopTriggers == null || CircuitRacingController.circuitCarsControl.GetRaceCars().Count > PitStopTriggers.Length)
		{
			return;
		}
		List<LapRaceCar> raceCars = CircuitRacingController.circuitCarsControl.GetRaceCars();
		for (int j = 0; j < raceCars.Count; j++)
		{
			if (!(PitStopTriggers[j] == null))
			{
				PitStopTriggers[j].Init(raceCars[j].GetRaceCarId(), raceCars[j].IsUser());
			}
		}
	}

	public void SetTime(int _time)
	{
		CancelCountdown();
		_countdownCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
		CountdownAsync(_time, _countdownCts.Token).Forget();
	}

	private void CancelCountdown()
	{
		if (_countdownCts != null)
		{
			_countdownCts.Cancel();
			_countdownCts.Dispose();
			_countdownCts = null;
		}
	}

	private async UniTaskVoid CountdownAsync(int minutesCount, CancellationToken ct)
	{
		try
		{
			for (int totalSeconds = minutesCount * 60; totalSeconds > 0; totalSeconds = (_secoundsLeft = totalSeconds - 1))
			{
				int num = totalSeconds / 60;
				int num2 = totalSeconds % 60;
				CircuitRacingUIControl.SetLap($"{num:00}:{num2:00}", "");
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void StopLemanCountdown()
	{
		CancelCountdown();
	}

	public void LeaderFinishLeman()
	{
		_managerUI.ShowNotificationWindow("Leader finished race. This lap is Final", 3f);
		CancelCountdown();
		FinishRaceMembersAfterLeader();
		CircuitRacingUIControl.SetLap("Final Lap", "");
	}

	public async UniTaskVoid FinishRace(LapRaceCar lapRaceCar, string secs)
	{
		if (lapRaceCar.IsUser())
		{
			CircuitRacingController.MarkUserFinishSequenceStarted();
		}
		while (FlagsStorage.IsPhotoMode)
		{
			await UniTask.Yield();
		}
		if (lapRaceCar.IsUser())
		{
			CircuitRacingController.CurrentRaceState = RaceState.RaceFinished;
			CircuitRacingUIControl.ShowChartList(state: false);
		}
		lapRaceCar.FinishRace(stopCar: false);
		lapRaceCar.SetFinalDistance(lapRaceCar.GetTotalDistanceForLeman());
		if (lapRaceCar.IsUser())
		{
			if (IsTimeExpired())
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQKA", "le_mans_race");
			}
			if (IsTimeExpired())
			{
				_steamAchievementService?.Unlock("ACH_TWENTY_FOUR_MINUTES");
			}
			CircuitRacingController.circuitCarsControl.SetIgnoreCollisionWithCollider(lapRaceCar, ignore: true);
			_managerUI.CloseCircuitPlayWindow();
			_managerUI.ShowCircuitFinishSplashWindow();
			CircuitRacingController.ResetRace();
			await UniTask.Delay(TimeSpan.FromSeconds(5.0));
			_managerUI.CloseCircuitFinishSplashWindow();
			await CircuitRacingUIControl.OnRaceFinish(CircuitRacingController.circuitCarsControl.GetRaceCars(), CircuitRacingController.GetRaceMapModel().RaceConfig.IsLeman);
			lapRaceCar.SetLemanAchievment();
		}
		CircuitRacingController.circuitCarsControl.GetUserLapRaceCar().SetLemanAchievment();
	}

	public void SetAchievments()
	{
		CircuitRacingController.circuitCarsControl.GetUserLapRaceCar().SetLemanAchievment();
	}

	public bool IsTimeExpired()
	{
		return _secoundsLeft <= 1;
	}

	public bool IsNeedAddBonusTime()
	{
		return _secoundsLeft <= CircuitRacingController.GetRaceMapModel().RaceConfig.BonusLapCoeficentSeconds;
	}

	public int GetSecoundsLeft()
	{
		return _secoundsLeft;
	}

	public bool IsFinished()
	{
		return _secoundsLeft <= 0;
	}

	public void Reset()
	{
		CancelCountdown();
	}

	public void SetPenaltyLap(LapRaceCar lapRaceCar)
	{
		string text = (lapRaceCar.IsUser() ? "You finished before time expired! Added Final Lap" : "Leader Ffnished before time expired! Added Final Lap");
		_managerUI.ShowNotificationWindow(text, 3f);
	}

	public void FinishRaceMembersAfterLeader()
	{
		foreach (LapRaceCar raceCar in CircuitRacingController.circuitCarsControl.GetRaceCars())
		{
			raceCar.IsHasBonusLap = false;
		}
	}

	public void AddBonusLap(int index)
	{
		foreach (LapRaceCar raceCar in CircuitRacingController.circuitCarsControl.GetRaceCars())
		{
			if (raceCar.GetRaceCarId() != index)
			{
				raceCar.IsHasBonusLap = true;
			}
		}
	}

	private void OnDestroy()
	{
		CancelCountdown();
		_raceMapModel = null;
	}
}
