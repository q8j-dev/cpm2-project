using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class InitMapIconsPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InitMapIconsPresenter initMapIconsPresenter = new InitMapIconsPresenter((MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (SceneControl)resolver.ResolveOrParameter(typeof(SceneControl), "sceneControl", parameters), (MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters), (MapIconsStarter)resolver.ResolveOrParameter(typeof(MapIconsStarter), "mapIconsStarter", parameters), (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (TimeAttackDataSO)resolver.ResolveOrParameter(typeof(TimeAttackDataSO), "timeAttackDataSo", parameters), (MapIconPositionsDataSo)resolver.ResolveOrParameter(typeof(MapIconPositionsDataSo), "mapIconPositionsDataSo", parameters), (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "arenaRegistry", parameters));
			Inject(initMapIconsPresenter, resolver, parameters);
			return initMapIconsPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
