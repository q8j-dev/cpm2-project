using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

public class LapRaceLeaderbordUiElement : MonoBehaviour
{
	[SerializeField]
	private Text MemberName;

	[SerializeField]
	private RawImage CarImage;

	[SerializeField]
	private RawImage RangImage;

	[SerializeField]
	private TextMeshProUGUI AllTimeText;

	[SerializeField]
	private TextMeshProUGUI BestLapTimeText;

	[SerializeField]
	private TextMeshProUGUI PenaltyTimeText;

	[SerializeField]
	private TextMeshProUGUI PlaceText;

	[SerializeField]
	private Image PlaceImage;

	[SerializeField]
	private TextMeshProUGUI LapText;

	[SerializeField]
	private GameObject LapSeparator;

	[SerializeField]
	private GameObject IsUserPanel;

	public float TotalTime;

	public int index;

	private LapRaceLeaderboardUIPanel _lapRaceLeaderboardUIPanel;

	public bool IsDisqualified;

	public void Init(LapRaceCar lapRaceCar, LapRaceLeaderboardUIPanel lapRaceLeaderboardUIPanel)
	{
		_lapRaceLeaderboardUIPanel = lapRaceLeaderboardUIPanel;
		SetPlace(-1);
		IsUserPanel.SetActive(lapRaceCar.IsUser());
		MemberName.text = lapRaceCar.GetUserName().ToUpper();
		CarImage.texture = lapRaceCar.GetCarIcon();
		RangImage.texture = lapRaceCar.GetRankIcon();
		AllTimeText.text = Utils.FormatTime(lapRaceCar.GetTotalTime());
		BestLapTimeText.text = Utils.FormatTime(lapRaceCar.GetBestLapTime());
		string arg = new LocalizedString("Freedragracingwindow.Sec");
		PenaltyTimeText.text = $" +{lapRaceCar.GetPenaltyTime()} {arg}";
		LapInfoActive(isActive: false);
	}

	public void InitLeman(LapRaceCar lapRaceCar, LapRaceLeaderboardUIPanel lapRaceLeaderboardUIPanel)
	{
		PenaltyTimeText.text = "-";
		_lapRaceLeaderboardUIPanel = lapRaceLeaderboardUIPanel;
		SetPlace(-1);
		IsUserPanel.SetActive(lapRaceCar.IsUser());
		MemberName.text = lapRaceCar.GetUserName().ToUpper();
		CarImage.texture = lapRaceCar.GetCarIcon();
		RangImage.texture = lapRaceCar.GetRankIcon();
		float kmDistance = GetKmDistance(lapRaceCar.GetTotalDistanceForLeman());
		GetKmDistance(lapRaceCar.GetLemanTotalDistance());
		float kmDistance2 = GetKmDistance(lapRaceCar.GetPenaltyDistance());
		AllTimeText.text = $"{kmDistance:F2} {Utils.CurrentMetric.ToUpper()}";
		BestLapTimeText.text = Utils.FormatTime(lapRaceCar.GetBestLap());
		if (kmDistance2 > 0f)
		{
			PenaltyTimeText.text = $" - {kmDistance2:F2} {Utils.CurrentMetric.ToUpper()}";
		}
		LapInfoActive(isActive: true, lapRaceCar);
	}

	private void LapInfoActive(bool isActive, LapRaceCar lapRaceCar = null)
	{
		if ((bool)LapSeparator)
		{
			LapSeparator.SetActive(isActive);
		}
		if ((bool)LapText)
		{
			LapText.transform.parent.gameObject.SetActive(isActive);
			if (isActive && (bool)lapRaceCar)
			{
				LapText.text = lapRaceCar.GetTotalLaps().ToString();
			}
		}
	}

	private float GetKmDistance(float distance)
	{
		if (distance > 0f)
		{
			distance /= 1000f;
		}
		return distance;
	}

	public void SetPlace(int index)
	{
		int place = ((index == -1) ? 3 : index);
		PlaceImage.color = _lapRaceLeaderboardUIPanel.GetPlaceColor(place);
		string text = ((index == -1) ? "-" : (index + 1).ToString());
		PlaceText.text = text;
	}

	public void Disqualified()
	{
		AllTimeText.text = new LocalizedString("CircuitPlayWindow.Disqualified");
		BestLapTimeText.text = "-";
		PenaltyTimeText.text = "-";
		IsDisqualified = true;
	}
}
