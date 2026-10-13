using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class EnableDisableCarIconPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EnableDisableCarIconPresenter enableDisableCarIconPresenter = new EnableDisableCarIconPresenter((MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters));
			Inject(enableDisableCarIconPresenter, resolver, parameters);
			return enableDisableCarIconPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
