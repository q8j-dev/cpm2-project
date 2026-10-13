using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using Cinemachine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;

namespace Racing.CameraManagement
{
	[Preserve]
	internal class CameraManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CameraManager cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CameraManager obj = (CameraManager)instance;
			ScreenFader screenFader = (ScreenFader)resolver.ResolveOrParameter(typeof(ScreenFader), "screenFader", parameters);
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
			IOfflineModeSystem offlineModeSystem = (IOfflineModeSystem)resolver.ResolveOrParameter(typeof(IOfflineModeSystem), "offlineModeSystem", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			GaragePositionsConfig garagePositionsConfig = (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			OptionalDependency<CinemachineBrain> cinemachineBrain = (OptionalDependency<CinemachineBrain>)resolver.ResolveOrParameter(typeof(OptionalDependency<CinemachineBrain>), "cinemachineBrain", parameters);
			OptionalDependency<MenuControl> menuControl = (OptionalDependency<MenuControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<MenuControl>), "menuControl", parameters);
			obj.Construct(screenFader, slotService, garageSelectorService, offlineModeSystem, managerUI, garagePositionsConfig, prefabFactory, cinemachineBrain, menuControl);
		}
	}
}
