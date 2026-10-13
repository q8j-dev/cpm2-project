using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class ClickSnowMenIconPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ClickSnowMenIconPresenter clickSnowMenIconPresenter = new ClickSnowMenIconPresenter((MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters));
			Inject(clickSnowMenIconPresenter, resolver, parameters);
			return clickSnowMenIconPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
