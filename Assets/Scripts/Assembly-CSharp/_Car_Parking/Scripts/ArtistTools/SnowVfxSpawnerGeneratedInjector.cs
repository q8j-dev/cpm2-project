using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.ArtistTools
{
	[Preserve]
	internal class SnowVfxSpawnerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SnowVfxSpawner cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SnowVfxSpawner obj = (SnowVfxSpawner)instance;
			VfxPoolProvider vfxPoolProvider = (VfxPoolProvider)resolver.ResolveOrParameter(typeof(VfxPoolProvider), "vfxPoolProvider", parameters);
			obj.Construct(vfxPoolProvider);
		}
	}
}
