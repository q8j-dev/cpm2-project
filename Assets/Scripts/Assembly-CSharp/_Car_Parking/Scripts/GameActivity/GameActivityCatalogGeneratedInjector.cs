using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameActivity
{
	[Preserve]
	internal class GameActivityCatalogGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GameActivityCatalog gameActivityCatalog = new GameActivityCatalog((IEnumerable<IGameActivityProvider>)resolver.ResolveOrParameter(typeof(IEnumerable<IGameActivityProvider>), "providers", parameters));
			Inject(gameActivityCatalog, resolver, parameters);
			return gameActivityCatalog;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
