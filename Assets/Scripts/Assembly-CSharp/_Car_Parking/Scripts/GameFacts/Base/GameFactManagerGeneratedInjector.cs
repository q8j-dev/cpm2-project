using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameFacts.Base
{
	[Preserve]
	internal class GameFactManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameFactManager gameFactManager = new GameFactManager();
			Inject(gameFactManager, resolver, parameters);
			return gameFactManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
