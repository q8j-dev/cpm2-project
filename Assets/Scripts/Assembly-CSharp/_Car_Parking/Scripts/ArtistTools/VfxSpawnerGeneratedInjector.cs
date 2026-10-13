using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.ArtistTools
{
	[Preserve]
	internal class VfxSpawnerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VfxSpawner cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VfxSpawner obj = (VfxSpawner)instance;
			VfxPoolProvider vfxPoolProvider = (VfxPoolProvider)resolver.ResolveOrParameter(typeof(VfxPoolProvider), "vfxPoolProvider", parameters);
			obj.Construct(vfxPoolProvider);
		}
	}
}
