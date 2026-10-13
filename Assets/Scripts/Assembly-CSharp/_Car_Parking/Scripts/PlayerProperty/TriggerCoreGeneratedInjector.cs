using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProperty
{
	[Preserve]
	internal class TriggerCoreGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:TriggerCore cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
