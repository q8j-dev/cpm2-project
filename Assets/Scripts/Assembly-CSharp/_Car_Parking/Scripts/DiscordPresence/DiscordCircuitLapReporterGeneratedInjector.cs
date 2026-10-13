using System.Collections.Generic;
using RacingScripts.RaceControllers;
using VContainer;

namespace _Car_Parking.Scripts.DiscordPresence
{
	[Preserve]
	internal class DiscordCircuitLapReporterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DiscordCircuitLapReporter discordCircuitLapReporter = new DiscordCircuitLapReporter((IDiscordPresenceService)resolver.ResolveOrParameter(typeof(IDiscordPresenceService), "presence", parameters), (RaceModeController)resolver.ResolveOrParameter(typeof(RaceModeController), "raceModeController", parameters));
			Inject(discordCircuitLapReporter, resolver, parameters);
			return discordCircuitLapReporter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
