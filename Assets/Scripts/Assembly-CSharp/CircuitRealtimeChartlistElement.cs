using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

public class CircuitRealtimeChartlistElement : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI PlaceNumberText;

	[SerializeField]
	private Text UserNameText;

	[SerializeField]
	private TextMeshProUGUI DistanceText;

	[SerializeField]
	private RawImage CarImage;

	[SerializeField]
	private GameObject UsersCarPanel;

	[SerializeField]
	private CanvasGroup UsersCarGroup;

	private Texture rankImage;

	[SerializeField]
	private int PlaceIndex;

	private bool _isDestroyed;

	private void Start()
	{
		rankImage = UtilsRank.LoadRankTextureFromResources(UtilsRank.RankType.Beginner);
	}

	private void OnEnable()
	{
		_isDestroyed = false;
	}

	public void Init(LapRaceCar lapRaceCar, float distance, int placeNumber, bool isLeman = false)
	{
		if (_isDestroyed)
		{
			return;
		}
		UsersCarGroup.alpha = 1f;
		UserNameText.text = lapRaceCar.GetUserName().ToUpper();
		PlaceNumberText.text = placeNumber.ToString();
		CarImage.texture = (lapRaceCar.IsUser() ? lapRaceCar.GetRankIcon() : rankImage);
		UsersCarPanel.SetActive(lapRaceCar.IsUser());
		ChangeColor(lapRaceCar.IsUser());
		if (!DistanceText)
		{
			return;
		}
		DistanceText.gameObject.SetActive(value: true);
		if (lapRaceCar.IsUser() & isLeman)
		{
			int num = lapRaceCar.GetCurrentLap() + 1;
			DistanceText.text = string.Concat(new LocalizedString("CircuitRealtimeChartlist.Lap"), ": ", num.ToString());
			return;
		}
		if (float.IsInfinity(distance))
		{
			distance = -1f;
		}
		bool flag = distance < 0f && distance != -1f;
		if (flag)
		{
			distance *= -1f;
			if (distance > 400f)
			{
				distance = -1f;
			}
		}
		string text = ((Mathf.Abs(distance) < 0.0001f) ? "" : ((!flag) ? "-" : "+"));
		string text2 = ((distance == -1f) ? "" : (text + " " + Utils.FormatTime3(distance)));
		DistanceText.text = text2;
	}

	private void ChangeColor(bool state)
	{
		PlaceNumberText.color = ((!state) ? Color.white : Color.black);
		UserNameText.color = ((!state) ? Color.white : Color.black);
		DistanceText.color = ((!state) ? Color.white : Color.black);
	}

	public void ShowPanel(bool state)
	{
		UsersCarGroup.alpha = (state ? 1 : 0);
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
	}
}
