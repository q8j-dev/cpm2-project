using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DiscordPresence
{
	[Preserve]
	internal class DiscordFreeDrivePresenceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DiscordFreeDrivePresence discordFreeDrivePresence = new DiscordFreeDrivePresence((IDiscordPresenceService)resolver.ResolveOrParameter(typeof(IDiscordPresenceService), "presence", parameters));
			Inject(discordFreeDrivePresence, resolver, parameters);
			return discordFreeDrivePresence;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
