using System;
using System.Collections.Generic;
using CPM.DailyTasks;
using CPM.DailyTasks.Daily;
using CPM.Rating;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;

public class TaskLabel : MonoBehaviour
{
	private DailyTask dailyTask;

	private DailyTasksManager dailyTasksManager;

	[SerializeField]
	private Slider Slider;

	[SerializeField]
	private TextMeshProUGUI Progress;

	[SerializeField]
	private TextMeshProUGUI CoinLabel;

	[SerializeField]
	private TextMeshProUGUI CashLabel;

	[SerializeField]
	private Image BackgroundDoneImage;

	[SerializeField]
	private RawImage TaskImage;

	[SerializeField]
	private TextMeshProUGUI TaskName;

	[SerializeField]
	private TextMeshProUGUI TaskDescription;

	[SerializeField]
	private GameObject CompletePanel;

	public const float RoundKmToMeters = 1000f;

	[Inject]
	public void Construct(DailyTasksManager dailyTasksManager)
	{
		this.dailyTasksManager = dailyTasksManager;
	}

	public async UniTaskVoid Init(DailyTask dailyTask)
	{
		this.dailyTask = dailyTask;
		if (!dailyTasksManager.events.ContainsKey((int)dailyTask.type))
		{
			dailyTasksManager.events[(int)dailyTask.type] = null;
		}
		Dictionary<int, DailyTasksManager.OnValueChangeDelegate> events = dailyTasksManager.events;
		int type = (int)dailyTask.type;
		events[type] = (DailyTasksManager.OnValueChangeDelegate)Delegate.Combine(events[type], new DailyTasksManager.OnValueChangeDelegate(UpdateValue));
		RawImage taskImage = TaskImage;
		taskImage.texture = await dailyTasksManager.GetTexture(dailyTask.type);
		TaskName.text = new LocalizedString(dailyTask.name);
		TaskDescription.text = new LocalizedString(dailyTask.description);
		CoinLabel.text = dailyTask.prize.coin.ToString();
		CashLabel.text = dailyTask.prize.cash.ToString();
		float num = FormatValue(dailyTask.type, dailyTask.goal);
		SetText(dailyTask.current, num);
		Slider.value = dailyTask.current / num;
		if (Slider.value >= 1f)
		{
			CompletePanel.SetActive(value: true);
			BackgroundDoneImage.gameObject.SetActive(value: true);
		}
		if (dailyTask.prize.cash == 0)
		{
			CashLabel.gameObject.transform.parent.gameObject.SetActive(value: false);
		}
		if (dailyTask.prize.coin == 0)
		{
			CoinLabel.gameObject.transform.parent.gameObject.SetActive(value: false);
		}
	}

	private void SetText(float current, float goal)
	{
		float num = Mathf.Clamp(current, 0f, goal);
		string arg = ((num % 1f == 0f) ? $"{num}" : $"{num:F1}");
		Progress.text = $"[{arg}/<color=grey>{goal}]";
	}

	private float FormatValue(TYPE type, float val)
	{
		if (type == TYPE.drift_max)
		{
			return val * 1000f;
		}
		return val;
	}

	private void UpdateValue(float val)
	{
		SetText(dailyTask.current, FormatValue(dailyTask.type, dailyTask.goal));
		Slider.value = val / FormatValue(dailyTask.type, dailyTask.goal);
		if (Slider.value >= 1f)
		{
			BackgroundDoneImage.gameObject.SetActive(value: true);
		}
	}

	private void OnDestroy()
	{
		if (dailyTasksManager == null || dailyTask == null)
		{
			return;
		}
		int type = (int)dailyTask.type;
		if (dailyTasksManager.events.TryGetValue(type, out DailyTasksManager.OnValueChangeDelegate value))
		{
			value = (DailyTasksManager.OnValueChangeDelegate)Delegate.Remove(value, new DailyTasksManager.OnValueChangeDelegate(UpdateValue));
			if (value == null)
			{
				dailyTasksManager.events.Remove(type);
			}
			else
			{
				dailyTasksManager.events[type] = value;
			}
		}
	}
}
