using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PlayerProfile;

namespace _Car_Parking.Scripts.GrantService
{
	[Preserve]
	internal class GrantHandlerIKPosesGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GrantHandlerIKPoses grantHandlerIKPoses = new GrantHandlerIKPoses((IPlayerProfileService)resolver.ResolveOrParameter(typeof(IPlayerProfileService), "playerProfileService", parameters));
			Inject(grantHandlerIKPoses, resolver, parameters);
			return grantHandlerIKPoses;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
