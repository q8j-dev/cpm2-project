using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class PhoneGameActivityCanvasInfoGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PhoneGameActivityCanvasInfo cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhoneGameActivityCanvasInfo obj = (PhoneGameActivityCanvasInfo)instance;
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			ArenaModeRegistry arenaModeRegistry = (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "arenaModeRegistry", parameters);
			obj.Construct(prefabFactory, arenaModeRegistry);
		}
	}
}
