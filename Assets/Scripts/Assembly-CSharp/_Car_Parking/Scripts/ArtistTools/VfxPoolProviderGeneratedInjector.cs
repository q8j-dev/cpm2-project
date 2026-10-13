using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.ArtistTools
{
	[Preserve]
	internal class VfxPoolProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VfxPoolProvider vfxPoolProvider = new VfxPoolProvider();
			Inject(vfxPoolProvider, resolver, parameters);
			return vfxPoolProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
