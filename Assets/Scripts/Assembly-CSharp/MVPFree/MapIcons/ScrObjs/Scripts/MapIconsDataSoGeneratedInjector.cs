using System.Collections.Generic;
using VContainer;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[Preserve]
	internal class MapIconsDataSoGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MapIconsDataSo mapIconsDataSo = new MapIconsDataSo();
			Inject(mapIconsDataSo, resolver, parameters);
			return mapIconsDataSo;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
