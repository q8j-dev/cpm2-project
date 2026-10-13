using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class VContainerGameStateMachineGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VContainerGameStateMachine vContainerGameStateMachine = new VContainerGameStateMachine((IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "resolver", parameters));
			Inject(vContainerGameStateMachine, resolver, parameters);
			return vContainerGameStateMachine;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
