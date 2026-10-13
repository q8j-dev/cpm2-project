using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class ArenaIconClickPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaIconClickPresenter arenaIconClickPresenter = new ArenaIconClickPresenter((MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arenaFlow", parameters));
			Inject(arenaIconClickPresenter, resolver, parameters);
			return arenaIconClickPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
