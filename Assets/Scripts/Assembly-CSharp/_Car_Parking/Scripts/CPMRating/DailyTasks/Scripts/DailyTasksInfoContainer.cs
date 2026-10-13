#nullable enable annotations
using System;
using CPM.DailyTasks.Daily;
using CPM.Rating;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.CPMRating.DailyTasks.Scripts
{
	[Preserve]
	public class DailyTasksInfoContainer
	{
		public bool HasDriftMaxTask { get; private set; }

		public void Set(DailyTask[]? dailyTasksTasks)
		{
			try
			{
				foreach (DailyTask dailyTask in dailyTasksTasks)
				{
					if (dailyTask.type == TYPE.drift_max && dailyTask.status == 0)
					{
						HasDriftMaxTask = true;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[DailyTasksInfoContainer] Set daily tasks error: " + ex);
			}
		}

		public void SetDriftMaxCompleted()
		{
			HasDriftMaxTask = false;
		}
	}
}
