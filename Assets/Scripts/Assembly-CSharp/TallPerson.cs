using System;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using UnityEngine;
using _Car_Parking.Scripts.Analytics;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Providers;

public class TallPerson : MonoBehaviour
{
	public GameObject person;

	private CancellationTokenSource _timerCts;

	private IEarnMoneyService _earnMoneyService;

	private FreeDriveProvider _freeDriveProvider;

	public void Init(IEarnMoneyService earnMoneyService, FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
		_earnMoneyService = earnMoneyService;
	}

	public void RunTimer()
	{
		if (person != null)
		{
			_timerCts?.Cancel();
			_timerCts?.Dispose();
			_timerCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			StartTimerAsync(_timerCts.Token).Forget();
		}
	}

	private void OnDestroy()
	{
		_timerCts?.Cancel();
		_timerCts?.Dispose();
		_timerCts = null;
	}

	private async UniTaskVoid StartTimerAsync(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			while ((bool)person)
			{
				if ((person.transform.position - base.transform.position).sqrMagnitude > 3f)
				{
					person = null;
					_freeDriveProvider.TallPerson = null;
				}
				await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void OnCarPass(GameObject car)
	{
		if (!(person == V.DriverPerson) || !car)
		{
			return;
		}
		ObscuredInt obscuredInt;
		if (car.TryGetComponent<VehicleComponent>(out var _))
		{
			obscuredInt = _earnMoneyService.GateTrafficCarReward;
		}
		else
		{
			if (!car.TryGetComponent<VehicleData>(out var component2))
			{
				return;
			}
			obscuredInt = _earnMoneyService.GateMultiplayerCarReward;
			if (component2.Driver == null || component2.Driver == person)
			{
				return;
			}
		}
		AnalyticStorage.TallRewardInSession += obscuredInt;
		CoreGameManager coreGameManager = V.CoreGameManager;
		ObscuredInt obscuredInt2 = obscuredInt;
		coreGameManager.SetMessage("+" + obscuredInt2.ToString(), 1f);
		CurrencyManagerController.Instance.AddMoney(new AdvancedInt(obscuredInt));
		++_freeDriveProvider.FreeDriveDB.blockPost;
		int num = ObscuredPrefs.GetInt("blockPostWorkCount");
		ObscuredPrefs.SetInt("blockPostWorkCount", num + 1);
	}
}
