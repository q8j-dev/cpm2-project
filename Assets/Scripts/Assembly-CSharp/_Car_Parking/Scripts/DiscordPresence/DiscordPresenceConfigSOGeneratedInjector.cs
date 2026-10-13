using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DiscordPresence
{
	[Preserve]
	internal class DiscordPresenceConfigSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DiscordPresenceConfigSO discordPresenceConfigSO = new DiscordPresenceConfigSO();
			Inject(discordPresenceConfigSO, resolver, parameters);
			return discordPresenceConfigSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
