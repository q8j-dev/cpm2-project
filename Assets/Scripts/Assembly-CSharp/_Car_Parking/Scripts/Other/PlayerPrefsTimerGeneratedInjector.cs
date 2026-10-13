using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;

namespace _Car_Parking.Scripts.Other
{
	[Preserve]
	internal class PlayerPrefsTimerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerPrefsTimer playerPrefsTimer = new PlayerPrefsTimer((IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters));
			Inject(playerPrefsTimer, resolver, parameters);
			return playerPrefsTimer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
