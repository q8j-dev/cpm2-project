using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class CameraSetupMoveGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CameraSetupMove cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CameraSetupMove obj = (CameraSetupMove)instance;
		OptionalDependency<RacingCameraController> racingCameraControllerDep = (OptionalDependency<RacingCameraController>)resolver.ResolveOrParameter(typeof(OptionalDependency<RacingCameraController>), "racingCameraControllerDep", parameters);
		OptionalDependency<MenuLevelParent> menuLevelParentDep = (OptionalDependency<MenuLevelParent>)resolver.ResolveOrParameter(typeof(OptionalDependency<MenuLevelParent>), "menuLevelParentDep", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		obj.Construct(racingCameraControllerDep, menuLevelParentDep, eventRegistry, inputMapSetEvent, coreGameManager);
	}
}
