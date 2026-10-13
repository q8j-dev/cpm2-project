using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DiscordPresence
{
	[Preserve]
	internal class DiscordScenePresenceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DiscordScenePresence discordScenePresence = new DiscordScenePresence((IDiscordPresenceService)resolver.ResolveOrParameter(typeof(IDiscordPresenceService), "presence", parameters), (DiscordActivityKind)resolver.ResolveOrParameter(typeof(DiscordActivityKind), "kind", parameters), (bool)resolver.ResolveOrParameter(typeof(bool), "reportEnvironment", parameters));
			Inject(discordScenePresence, resolver, parameters);
			return discordScenePresence;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
