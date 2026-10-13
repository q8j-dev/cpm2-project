using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class ResetIconsDescriptionScrollViewPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ResetIconsDescriptionScrollViewPresenter resetIconsDescriptionScrollViewPresenter = new ResetIconsDescriptionScrollViewPresenter((MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters));
			Inject(resetIconsDescriptionScrollViewPresenter, resolver, parameters);
			return resetIconsDescriptionScrollViewPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
