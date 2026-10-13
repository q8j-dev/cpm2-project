using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Notification;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	[Preserve]
	internal class EmoteNoticeTrackerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EmoteNoticeTracker emoteNoticeTracker = new EmoteNoticeTracker((INotificationService)resolver.ResolveOrParameter(typeof(INotificationService), "notifications", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters));
			Inject(emoteNoticeTracker, resolver, parameters);
			return emoteNoticeTracker;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
