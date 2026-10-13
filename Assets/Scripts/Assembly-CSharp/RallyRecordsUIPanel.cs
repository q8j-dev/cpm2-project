using System;
using System.Collections.Generic;
using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Racing_Scripts;

public class RallyRecordsUIPanel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI GoldTimeText;

	[SerializeField]
	private TextMeshProUGUI SilverTimeText;

	[SerializeField]
	private TextMeshProUGUI CopperTimeText;

	[SerializeField]
	private TextMeshProUGUI RecordText;

	[SerializeField]
	private Image PersonalCupIcon;

	[SerializeField]
	private Color[] CupColors;

	private RallyController _rallyController;

	public void SetTargetTimes(RallyController rallyController)
	{
		if (rallyController.RallyProgressController != null && rallyController.RallyProgressController.RallyData != null)
		{
			_rallyController = rallyController;
			RallyController rallyController2 = _rallyController;
			rallyController2.OnLocalProgressUpdated = (Action)Delegate.Combine(rallyController2.OnLocalProgressUpdated, new Action(UpdatePersonalBest));
			RallyTimes[] rallyTimes = rallyController.RallySequenceData.RallyTimes;
			_ = rallyTimes.Length;
			List<float> list = new List<float>();
			for (int i = 0; i < rallyTimes.Length; i++)
			{
				float num = ((_rallyController.RallyData != null && _rallyController.RallyData.NgTimeFactor > 0f) ? _rallyController.RallyData.NgTimeFactor : 1f);
				float item = rallyTimes[i].TargetTime * num;
				list.Add(item);
			}
			GoldTimeText.text = Utils.FormatTime(list[0]);
			SilverTimeText.text = Utils.FormatTime(list[1]);
			CopperTimeText.text = Utils.FormatTime(list[2]);
			UpdatePersonalBest();
		}
	}

	private void UpdatePersonalBest()
	{
		int locationIndex = RallyController.LocationIndex;
		int trajectoryIndex = RallyController.TrajectoryIndex;
		RallyProgress progress = _rallyController.RallyProgressController.GetProgress(locationIndex, trajectoryIndex);
		if (progress != null && progress.PersonalBest > 0f)
		{
			RecordText.text = Utils.FormatTime(progress.PersonalBest);
			int rallyTier = progress.RallyTier;
			if (rallyTier >= 0 && rallyTier < CupColors.Length)
			{
				SetCupColor(CupColors[rallyTier]);
			}
		}
	}

	private void SetCupColor(Color color)
	{
		PersonalCupIcon.color = color;
	}

	private void OnDestroy()
	{
		if (_rallyController != null)
		{
			RallyController rallyController = _rallyController;
			rallyController.OnLocalProgressUpdated = (Action)Delegate.Remove(rallyController.OnLocalProgressUpdated, new Action(UpdatePersonalBest));
		}
	}
}
