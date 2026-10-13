using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class ClickToMapIconPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ClickToMapIconPresenter clickToMapIconPresenter = new ClickToMapIconPresenter((MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters), (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (GPSNavigationCrossView)resolver.ResolveOrParameter(typeof(GPSNavigationCrossView), "gpsNavigationCrossView", parameters));
			Inject(clickToMapIconPresenter, resolver, parameters);
			return clickToMapIconPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
