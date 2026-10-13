using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Audio
{
	[Preserve]
	internal class TribuneSoundPlayerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:TribuneSoundPlayer cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TribuneSoundPlayer obj = (TribuneSoundPlayer)instance;
			RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
			obj.Construct(racingProvider);
		}
	}
}
