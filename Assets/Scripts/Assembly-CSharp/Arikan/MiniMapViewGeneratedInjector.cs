using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace Arikan
{
	[Preserve]
	internal class MiniMapViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MiniMapView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MiniMapView obj = (MiniMapView)instance;
			OptionalDependency<MiniMapBounds> miniMapBounds = (OptionalDependency<MiniMapBounds>)resolver.ResolveOrParameter(typeof(OptionalDependency<MiniMapBounds>), "miniMapBounds", parameters);
			obj.Construct(miniMapBounds);
		}
	}
}
