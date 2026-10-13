using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.VinylsAi
{
	[Preserve]
	internal class PlayerVinylCensorCheckerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerVinylCensorChecker playerVinylCensorChecker = new PlayerVinylCensorChecker((VinylsCheckApi)resolver.ResolveOrParameter(typeof(VinylsCheckApi), "vinylsCheckApi", parameters));
			Inject(playerVinylCensorChecker, resolver, parameters);
			return playerVinylCensorChecker;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
