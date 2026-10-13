using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Notification
{
	public interface INotificationService
	{
		bool HasNotification { get; }

		event Action<int, int> NoticeByTypeChanged;

		void Initialize();

		void SetNotice(int type, int index, int gender);

		void MarkAsRead(int type, int index, int gender);

		List<EquipmentNotice> GetNotices(int gender);

		bool HasNotice(int type, int index, int gender);

		bool HasNotice(PersonWindowCategoryType categoryType, int gender);

		bool HasNotice(int type, int gender);

		bool HasNotice(int gender);

		bool AnyNotice(int gender, Func<EquipmentNotice, bool> predicate);
	}
}
