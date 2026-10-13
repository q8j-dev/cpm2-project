using System.Collections.Generic;
using VContainer;

namespace CPM.Rating
{
	[Preserve]
	internal class AchievementPopupChannelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AchievementPopupChannel achievementPopupChannel = new AchievementPopupChannel();
			Inject(achievementPopupChannel, resolver, parameters);
			return achievementPopupChannel;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
