using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.EventSystem;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class MapIconShowHidePresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MapIconShowHidePresenter mapIconShowHidePresenter = new MapIconShowHidePresenter((MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters), (TimedActivityZoneController)resolver.ResolveOrParameter(typeof(TimedActivityZoneController), "timedActivityZoneController", parameters));
			Inject(mapIconShowHidePresenter, resolver, parameters);
			return mapIconShowHidePresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
