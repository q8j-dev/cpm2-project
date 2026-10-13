using System;
using System.Collections.Generic;
using MVPFree.MapIcons;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class MapCameraGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MapCamera cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MapCamera obj = (MapCamera)instance;
		MapControl mapControl = (MapControl)resolver.ResolveOrParameter(typeof(MapControl), "mapControl", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		GlobalCollectionsProvider collectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "collectionsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		MapIconsModel mapIconsModel = (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		SteeringWheelValueEvent steeringWheelValueEvent = (SteeringWheelValueEvent)resolver.ResolveOrParameter(typeof(SteeringWheelValueEvent), "steeringWheelValueEvent", parameters);
		UiNavigationValueEvent uiNavigationValueEvent = (UiNavigationValueEvent)resolver.ResolveOrParameter(typeof(UiNavigationValueEvent), "uiNavigationValueEvent", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		OptionalDependency<DesktopControlsSettingsSO> settingsDep = (OptionalDependency<DesktopControlsSettingsSO>)resolver.ResolveOrParameter(typeof(OptionalDependency<DesktopControlsSettingsSO>), "settingsDep", parameters);
		OptionalDependency<MapIconsStarter> mapIconsStarter = (OptionalDependency<MapIconsStarter>)resolver.ResolveOrParameter(typeof(OptionalDependency<MapIconsStarter>), "mapIconsStarter", parameters);
		OptionalDependency<RacingCameraController> racingCameraController = (OptionalDependency<RacingCameraController>)resolver.ResolveOrParameter(typeof(OptionalDependency<RacingCameraController>), "racingCameraController", parameters);
		obj.Construct(mapControl, menuLevelParent, collectionsProvider, freeDriveProvider, mapIconsModel, eventRegistry, steeringWheelValueEvent, uiNavigationValueEvent, carInputManager, inputMapSetEvent, settingsDep, mapIconsStarter, racingCameraController);
	}
}
