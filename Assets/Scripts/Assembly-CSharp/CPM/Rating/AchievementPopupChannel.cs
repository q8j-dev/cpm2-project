using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.CPMRating.PopupNotifications;

namespace CPM.Rating
{
	public class AchievementPopupChannel
	{
		private readonly Queue<PopupNotificationData> _queue = new Queue<PopupNotificationData>();

		public int Count => _queue.Count;

		public event Action OnEnqueued;

		public void Enqueue(PopupNotificationData data)
		{
			_queue.Enqueue(data);
			OnEnqueued?.Invoke();
		}

		public bool TryPeek(out PopupNotificationData data)
		{
			if (_queue.Count > 0)
			{
				data = _queue.Peek();
				return true;
			}
			data = null;
			return false;
		}

		public void Dequeue()
		{
			if (_queue.Count > 0)
			{
				_queue.Dequeue();
			}
		}
	}
}
