using System;
using System.Collections.Generic;
using CPM.DailyTasks.Daily;
using CPM.Rating;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace CPM.DailyTasks
{
	public class MiniTaskLabel : MonoBehaviour
	{
		private DailyTask dailyTask;

		private DailyTasksManager _dailyTasksManager;

		[SerializeField]
		private GameObject checkmark;

		[SerializeField]
		private TextMeshProUGUI TitleText;

		[SerializeField]
		private GameObject ProgressIconPanel;

		[SerializeField]
		private Image ProgressImage;

		public void Init(DailyTasksManager dailyTasksManager, DailyTask dailyTask)
		{
			this.dailyTask = dailyTask;
			_dailyTasksManager = dailyTasksManager;
			if (!dailyTasksManager.events.ContainsKey((int)dailyTask.type))
			{
				dailyTasksManager.events[(int)dailyTask.type] = null;
			}
			Dictionary<int, DailyTasksManager.OnValueChangeDelegate> events = dailyTasksManager.events;
			int type = (int)dailyTask.type;
			events[type] = (DailyTasksManager.OnValueChangeDelegate)Delegate.Combine(events[type], new DailyTasksManager.OnValueChangeDelegate(UpdateValue));
			float num = ((dailyTask != null && dailyTask.type == TYPE.drift_max) ? (dailyTask.goal * 1000f) : dailyTask.goal);
			float num2 = Mathf.Clamp(this.dailyTask.current, 0f, num);
			string arg = ((num2 % 1f == 0f) ? $"{num2}" : $"{num2:F1}");
			TitleText.text = $"{new LocalizedString(dailyTask.name)} [{arg}/<color=grey>{num}]";
			float num3 = dailyTask.current / num;
			if ((bool)ProgressImage)
			{
				ProgressImage.fillAmount = num3;
			}
			if (num3 >= 1f)
			{
				checkmark.SetActive(value: true);
				ProgressIconPanel.SetActive(value: false);
			}
			if (num3 == 0f)
			{
				ProgressImage.fillAmount = 0.05f;
			}
		}

		private void UpdateValue(float val)
		{
			if ((bool)ProgressImage)
			{
				ProgressImage.fillAmount = val / dailyTask.goal;
			}
		}

		private void OnDestroy()
		{
			if (_dailyTasksManager == null || dailyTask == null)
			{
				return;
			}
			int type = (int)dailyTask.type;
			if (_dailyTasksManager.events.TryGetValue(type, out DailyTasksManager.OnValueChangeDelegate value))
			{
				value = (DailyTasksManager.OnValueChangeDelegate)Delegate.Remove(value, new DailyTasksManager.OnValueChangeDelegate(UpdateValue));
				if (value == null)
				{
					_dailyTasksManager.events.Remove(type);
				}
				else
				{
					_dailyTasksManager.events[type] = value;
				}
			}
			_dailyTasksManager = null;
		}
	}
}
