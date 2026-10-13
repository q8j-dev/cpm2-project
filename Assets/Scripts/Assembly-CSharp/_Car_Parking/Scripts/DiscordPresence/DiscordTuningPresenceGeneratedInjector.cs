using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DiscordPresence
{
	[Preserve]
	internal class DiscordTuningPresenceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DiscordTuningPresence discordTuningPresence = new DiscordTuningPresence((IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "resolver", parameters), (IDiscordPresenceService)resolver.ResolveOrParameter(typeof(IDiscordPresenceService), "presence", parameters), (DiscordActivityKind)resolver.ResolveOrParameter(typeof(DiscordActivityKind), "idleKind", parameters));
			Inject(discordTuningPresence, resolver, parameters);
			return discordTuningPresence;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
