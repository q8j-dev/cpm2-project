using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class InsideCamMoveGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:InsideCamMove cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		InsideCamMove obj = (InsideCamMove)instance;
		OptionalDependency<RacingCameraController> racingCameraControllerDep = (OptionalDependency<RacingCameraController>)resolver.ResolveOrParameter(typeof(OptionalDependency<RacingCameraController>), "racingCameraControllerDep", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		obj.Construct(racingCameraControllerDep, coreGameManager);
	}
}
