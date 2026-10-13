using System;
using System.Globalization;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.Base;

public class RewardBySpeed : MonoBehaviour
{
	private readonly int _maxRewardSpeed = 499;

	private readonly int _nextRewardSpeed = 100;

	public int id;

	public TextMesh currentSpeedText;

	public TextMesh kmORmph;

	private CurrencyManagerController _currencyManagerController;

	private IEarnMoneyService _earnMoneyService;

	private ManagerUI _managerUI;

	private ISteamAchievementService _steamAchievementService;

	[Inject]
	public void Construct(CurrencyManagerController currencyManagerController, IEarnMoneyService earnMoneyService, ManagerUI managerUI, ISteamAchievementService steamAchievementService)
	{
		_managerUI = managerUI;
		_earnMoneyService = earnMoneyService;
		_currencyManagerController = currencyManagerController;
		_steamAchievementService = steamAchievementService;
	}

	private void Start()
	{
		Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			SetSpeedAfter();
		}).AddTo(this);
	}

	public void SetSpeedAfter()
	{
		int num = PlayerPrefs.GetInt($"SpeedReward_{id}", _nextRewardSpeed);
		if (num == 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if ((bool)currentSpeedText)
		{
			currentSpeedText.text = FormatSpeed(num);
		}
		if ((bool)kmORmph)
		{
			kmORmph.text = (FlagsStorage.Kph ? "KPH" : "MPH");
		}
	}

	private static string FormatSpeed(int speedKmh)
	{
		return Mathf.RoundToInt(Utils.GetConvertedMileage(speedKmh)).ToString(CultureInfo.InvariantCulture);
	}

	public void CheckSpeed(float speed)
	{
		int num = Mathf.RoundToInt(speed);
		int num2 = PlayerPrefs.GetInt("SpeedReward_" + id, _nextRewardSpeed);
		SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.SpeedAchiveSwitch, base.gameObject);
		if (num >= num2)
		{
			ObscuredInt value = (int)_earnMoneyService.SpeedBannerReward * num2;
			_currencyManagerController.AddMoney(new AdvancedInt(value));
			string format = new LocalizedString("RewardBySpeed.WellDoneMoney");
			_managerUI.ShowNotificationWindow(string.Format(format, value.ToString()), 3f).Forget();
			SetNext();
			GooglePlayAchievements.ReportProgress("CgkIuKqW0PcZEAIQFg", "gpg_ach_speed_banners", 8, 1, "speed_banner");
			int current = _steamAchievementService.GetStatInt("steam_speed_banners") + 1;
			_steamAchievementService.ReportProgress("ACH_SMILE_FOR_THE_RADAR", "steam_speed_banners", 8, current);
		}
		else
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("RewardBySpeed.Faster"), 1f).Forget();
		}
	}

	private void SetNext()
	{
		int num = PlayerPrefs.GetInt("SpeedReward_" + id, _nextRewardSpeed) + _nextRewardSpeed;
		if (num > _maxRewardSpeed)
		{
			num = 0;
		}
		else
		{
			currentSpeedText.text = FormatSpeed(num);
		}
		PlayerPrefs.SetInt("SpeedReward_" + id, num);
		if (num == 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
