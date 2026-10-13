using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class UpdateIconsDescriptionPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UpdateIconsDescriptionPresenter updateIconsDescriptionPresenter = new UpdateIconsDescriptionPresenter((ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters));
			Inject(updateIconsDescriptionPresenter, resolver, parameters);
			return updateIconsDescriptionPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
