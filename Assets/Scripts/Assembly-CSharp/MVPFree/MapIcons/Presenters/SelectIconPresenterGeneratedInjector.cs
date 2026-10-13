using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class SelectIconPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SelectIconPresenter selectIconPresenter = new SelectIconPresenter((MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters));
			Inject(selectIconPresenter, resolver, parameters);
			return selectIconPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
