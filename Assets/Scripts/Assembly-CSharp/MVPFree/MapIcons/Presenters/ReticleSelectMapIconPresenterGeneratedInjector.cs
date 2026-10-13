using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class ReticleSelectMapIconPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ReticleSelectMapIconPresenter reticleSelectMapIconPresenter = new ReticleSelectMapIconPresenter((MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters), (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (GPSNavigationCrossView)resolver.ResolveOrParameter(typeof(GPSNavigationCrossView), "gpsNavigationCrossView", parameters), (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters));
			Inject(reticleSelectMapIconPresenter, resolver, parameters);
			return reticleSelectMapIconPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
