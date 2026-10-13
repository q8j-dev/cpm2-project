using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DiscordPresence
{
	[Preserve]
	internal class DiscordPresenceServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DiscordPresenceService discordPresenceService = new DiscordPresenceService((DiscordPresenceConfigSO)resolver.ResolveOrParameter(typeof(DiscordPresenceConfigSO), "config", parameters));
			Inject(discordPresenceService, resolver, parameters);
			return discordPresenceService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
