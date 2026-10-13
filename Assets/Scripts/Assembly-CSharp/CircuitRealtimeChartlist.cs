using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Racing_Scripts;

public class CircuitRealtimeChartlist : MonoBehaviour
{
	private const string TimeForPlayFinalSeconds = "00:10";

	[SerializeField]
	private CircuitRealtimeChartlistElement[] CircuitRealtimeChartlistElements;

	[SerializeField]
	private TextMeshProUGUI PlaceText;

	[SerializeField]
	private TextMeshProUGUI AllPlaceCount;

	[SerializeField]
	private TextMeshProUGUI AllLapCount;

	[SerializeField]
	private TextMeshProUGUI LapText;

	public GameObject Panel;

	private CircuitRaceControlller _circuitRaceController;

	public const int HideTimeVariable = 400;

	private CircuitRealtimeChartlistElement LastCircuitRealtimeChartlistElement
	{
		get
		{
			CircuitRealtimeChartlistElement[] circuitRealtimeChartlistElements = CircuitRealtimeChartlistElements;
			return circuitRealtimeChartlistElements[circuitRealtimeChartlistElements.Length - 1];
		}
	}

	public void Init(CircuitRaceControlller circuitRaceController)
	{
		_circuitRaceController = circuitRaceController;
	}

	private void Start()
	{
		PlaceText.ForceMeshUpdate();
		AllPlaceCount.ForceMeshUpdate();
		AllLapCount.ForceMeshUpdate();
		LapText.ForceMeshUpdate();
	}

	public void ParseList(List<LapRaceCar> lapRaceCars, int userIndex)
	{
		if (userIndex < 0 || userIndex >= lapRaceCars.Count)
		{
			return;
		}
		int count = lapRaceCars.Count;
		int num = 2;
		int num2 = 1;
		switch (userIndex)
		{
		case 0:
			num = 0;
			num2 = 3;
			break;
		case 1:
			num = 1;
			num2 = 2;
			break;
		default:
			if (userIndex == count - 1)
			{
				num = 3;
				num2 = 0;
			}
			else if (userIndex == count - 2)
			{
				num = 2;
				num2 = 1;
			}
			break;
		}
		int num3 = Math.Max(0, userIndex - num);
		int num4 = Math.Min(count - 1, userIndex + num2);
		for (int i = num3; i <= num4; i++)
		{
			float num5 = -1f;
			if (i != userIndex)
			{
				float num6 = lapRaceCars[i].GetTotalDistance() - lapRaceCars[userIndex].GetTotalDistance();
				float speed = lapRaceCars[userIndex].GetSpeed();
				num5 = ((speed > 0f) ? (num6 / speed) : (-1f));
				if (num5 > 400f)
				{
					num5 = -1f;
				}
			}
			CircuitRealtimeChartlistElements[i - num3].Init(lapRaceCars[i], num5, lapRaceCars[i].place + 1, _circuitRaceController.IsCurrentMapLeman());
		}
		for (int j = 0; j < lapRaceCars.Count && j != userIndex; j++)
		{
			float num7 = lapRaceCars[j].GetTotalDistance() - lapRaceCars[userIndex].GetTotalDistance();
			float num8 = 0f;
			if (num7 < 30f)
			{
				num8 = 0f;
			}
			else if (!(num7 >= 30f) || !(num7 <= 60f))
			{
				num8 = ((num7 > 60f && num7 <= 80f) ? 1f : ((!(num7 > 80f) || !(num7 <= 100f)) ? 0f : Mathf.InverseLerp(100f, 80f, num7)));
			}
			else
			{
				lapRaceCars[j].ShowLabel(state: true);
				num8 = Mathf.InverseLerp(30f, 60f, num7);
			}
			lapRaceCars[j].SetTopMeshAlpha(num8);
			int num9 = lapRaceCars[j].place + 1;
			lapRaceCars[j].SetPlaceLabel(num9.ToString());
		}
	}

	public void SetLap(string lap, string lapsCount)
	{
		LapText.text = lap;
		AllLapCount.text = lapsCount;
		if (lap == "00:10")
		{
			_circuitRaceController.RaceCrewTalkSoundController.PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Final_Seconds);
		}
	}

	public void SetProgress(string progress, string placesCount)
	{
		PlaceText.text = progress;
		AllPlaceCount.text = placesCount;
	}
}
