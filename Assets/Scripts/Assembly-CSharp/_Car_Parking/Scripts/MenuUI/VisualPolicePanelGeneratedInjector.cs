using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Police;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class VisualPolicePanelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VisualPolicePanel cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualPolicePanel obj = (VisualPolicePanel)instance;
			IPoliceSystem policeSystem = (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(policeSystem, prefabFactory);
		}
	}
}
