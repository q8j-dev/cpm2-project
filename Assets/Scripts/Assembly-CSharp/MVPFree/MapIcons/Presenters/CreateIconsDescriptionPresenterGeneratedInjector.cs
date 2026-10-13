using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class CreateIconsDescriptionPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CreateIconsDescriptionPresenter createIconsDescriptionPresenter = new CreateIconsDescriptionPresenter((MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters), (MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters));
			Inject(createIconsDescriptionPresenter, resolver, parameters);
			return createIconsDescriptionPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
