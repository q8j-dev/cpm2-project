using CPM.DailyTasks.Daily;
using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.CPMRating.PopupNotifications
{
	public class PopupDailyTaskData : PopupNotificationData
	{
		private readonly DailyTask _task;

		private readonly Texture _texture;

		public PopupDailyTaskData(DailyTask task, Texture texture)
		{
			_task = task;
			_texture = texture;
		}

		public override void InitPrefab(PopupNotificationWindow popupNotificationWindow)
		{
			if ((bool)popupNotificationWindow)
			{
				Texture bckgTexture = Resources.Load("icons/Rating/Ach_BG_" + 2, typeof(Texture)) as Texture;
				popupNotificationWindow.InitDailyTaskNotification(new LocalizedString(_task.name), new LocalizedString(_task.description), new LocalizedString("PopupDailyTaskData.Dailytaskcompleted!"), _texture, bckgTexture);
			}
		}

		public override string ToString()
		{
			return new LocalizedString(_task.name);
		}
	}
}
