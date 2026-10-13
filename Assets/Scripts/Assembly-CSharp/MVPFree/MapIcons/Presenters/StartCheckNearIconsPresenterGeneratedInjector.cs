using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class StartCheckNearIconsPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			StartCheckNearIconsPresenter startCheckNearIconsPresenter = new StartCheckNearIconsPresenter((MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters));
			Inject(startCheckNearIconsPresenter, resolver, parameters);
			return startCheckNearIconsPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
