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
	private TextMeshProUGUI BestTimeText = default;

	[SerializeField]
	private TextMeshProUGUI TimeText = default;

	[SerializeField]
	private Image CupImage = default;

	[SerializeField]
	private Image TimeImage = default;

	[SerializeField]
	private Color[] CupColor = default;

	[SerializeField]
	private Sprite SunIcon = default;

	[SerializeField]
	private Sprite RainIcon = default;

	[SerializeField]
	private Sprite SnowIcon = default;

	[SerializeField]
	private Image StarIcon = default;

	[SerializeField]
	private GameObject RewardRoot = default;

	[SerializeField]
	private Image RewardIcon = default;

	[SerializeField]
	private GameObject RewardClaimed = default;

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
