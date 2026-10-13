using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PlayerProfile;

namespace _Car_Parking.Scripts.GrantService
{
	[Preserve]
	internal class GrantHandlerAnimationsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GrantHandlerAnimations grantHandlerAnimations = new GrantHandlerAnimations((IPlayerProfileService)resolver.ResolveOrParameter(typeof(IPlayerProfileService), "playerProfileService", parameters));
			Inject(grantHandlerAnimations, resolver, parameters);
			return grantHandlerAnimations;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
