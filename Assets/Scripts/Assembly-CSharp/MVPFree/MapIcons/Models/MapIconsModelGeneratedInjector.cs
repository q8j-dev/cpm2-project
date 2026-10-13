using System.Collections.Generic;
using VContainer;

namespace MVPFree.MapIcons.Models
{
	[Preserve]
	internal class MapIconsModelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MapIconsModel mapIconsModel = new MapIconsModel();
			Inject(mapIconsModel, resolver, parameters);
			return mapIconsModel;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
