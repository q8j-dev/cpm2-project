using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.GameFacts.Consumers
{
	[Preserve]
	internal class ConditionStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ConditionState cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ConditionState obj = (ConditionState)instance;
			IGameFactManager facts = (IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "facts", parameters);
			obj.Construct(facts);
		}
	}
}
