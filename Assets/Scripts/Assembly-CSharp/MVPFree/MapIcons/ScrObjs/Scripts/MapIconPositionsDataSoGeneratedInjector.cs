using System.Collections.Generic;
using VContainer;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[Preserve]
	internal class MapIconPositionsDataSoGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MapIconPositionsDataSo mapIconPositionsDataSo = new MapIconPositionsDataSo();
			Inject(mapIconPositionsDataSo, resolver, parameters);
			return mapIconPositionsDataSo;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
