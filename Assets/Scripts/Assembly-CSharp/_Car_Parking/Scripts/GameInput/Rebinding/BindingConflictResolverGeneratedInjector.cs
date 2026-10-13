using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Preserve]
	internal class BindingConflictResolverGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BindingConflictResolver bindingConflictResolver = new BindingConflictResolver((GameInputManager)resolver.ResolveOrParameter(typeof(GameInputManager), "inputManager", parameters), (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters), (RebindableActionsConfig)resolver.ResolveOrParameter(typeof(RebindableActionsConfig), "config", parameters));
			Inject(bindingConflictResolver, resolver, parameters);
			return bindingConflictResolver;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
