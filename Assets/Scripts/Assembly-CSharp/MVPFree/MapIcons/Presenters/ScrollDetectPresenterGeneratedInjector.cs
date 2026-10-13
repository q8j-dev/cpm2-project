using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class ScrollDetectPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ScrollDetectPresenter scrollDetectPresenter = new ScrollDetectPresenter((MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconModel", parameters));
			Inject(scrollDetectPresenter, resolver, parameters);
			return scrollDetectPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
