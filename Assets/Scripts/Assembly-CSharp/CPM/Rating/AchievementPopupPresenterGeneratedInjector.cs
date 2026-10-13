using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace CPM.Rating
{
	[Preserve]
	internal class AchievementPopupPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AchievementPopupPresenter achievementPopupPresenter = new AchievementPopupPresenter((ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (AchievementPopupChannel)resolver.ResolveOrParameter(typeof(AchievementPopupChannel), "channel", parameters));
			Inject(achievementPopupPresenter, resolver, parameters);
			return achievementPopupPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
