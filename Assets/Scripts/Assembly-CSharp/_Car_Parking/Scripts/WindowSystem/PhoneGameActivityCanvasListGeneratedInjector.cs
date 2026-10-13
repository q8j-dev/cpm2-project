using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class PhoneGameActivityCanvasListGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PhoneGameActivityCanvasList cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhoneGameActivityCanvasList obj = (PhoneGameActivityCanvasList)instance;
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(prefabFactory);
		}
	}
}
