using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Rally.Data;

public class RallyTrajectoryLockableToggle : LockableUiToggleElement
{
	[SerializeField]
	private TextMeshProUGUI BestTimeText;

	[SerializeField]
	private TextMeshProUGUI TimeText;

	[SerializeField]
	private Image CupImage;

	[SerializeField]
	private Image TimeImage;

	[SerializeField]
	private Color[] CupColor;

	[SerializeField]
	private Sprite NightIcon;

	[SerializeField]
	private Sprite SunIcon;

	[SerializeField]
	private Sprite RainIcon;

	[SerializeField]
	private Sprite SnowIcon;

	[SerializeField]
	private Image StarIcon;

	[SerializeField]
	private GameObject RewardRoot;

	[SerializeField]
	private Image RewardIcon;

	[SerializeField]
	private GameObject RewardClaimed;

	public int Progress;

	public string BestTimeTextString => BestTimeText.text;

	protected override void Awake()
	{
		CupImage.color = CupColor[0];
	}

	public void UpdateUI(RallySequenceData rallySequenceData, int index, WeatherType weatherType, DayTimeType dayTimeType)
	{
		if (rallySequenceData != null)
		{
			StarIcon.gameObject.SetActive(rallySequenceData.IsSpecial);
			SetNight(dayTimeType);
			SetWeather(weatherType);
			SetSprite(rallySequenceData.TrajectoryLogo);
			string arg = new LocalizedString("Rally.Trajectory");
			SetText($"{arg} {index + 1}");
			UpdateRewardUI(rallySequenceData);
		}
	}

	public void SetBestTime(float time, int Progress, RallySequenceData rallySequenceData)
	{
		this.Progress = Progress;
		BestTimeText.text = Utils.FormatTime(time);
		CupImage.color = CupColor[Progress];
		UpdateRewardUI(rallySequenceData);
	}

	private void SetNight(DayTimeType dayTimeType)
	{
		TimeText.text = $"{(int)dayTimeType}:00 |";
	}

	private void SetWeather(WeatherType weatherType)
	{
		Sprite sprite = SunIcon;
		switch (weatherType)
		{
		case WeatherType.Rain:
			sprite = RainIcon;
			break;
		case WeatherType.Snow:
			sprite = SnowIcon;
			break;
		}
		TimeImage.sprite = sprite;
	}

	private void UpdateRewardUI(RallySequenceData rallySequenceData)
	{
		if (rallySequenceData == null)
		{
			return;
		}
		RewardRoot.SetActive(rallySequenceData.HaveReward);
		if (rallySequenceData.HaveReward)
		{
			RewardClaimed.SetActive(Progress == 3);
			CupImage.color = new Color(1f, 1f, 1f, 0f);
			if ((bool)rallySequenceData.Reward.Icon)
			{
				RewardIcon.sprite = rallySequenceData.Reward.Icon;
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		BestTimeText.text = "--:--:--";
		CupImage.color = CupColor[0];
		Progress = 0;
	}
}
