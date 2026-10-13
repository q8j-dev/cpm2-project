using System;
using System.Collections.Generic;
using Racing.CameraManagement;
using VContainer;

namespace CPM.Scripts.Garage.Elevator
{
	[Preserve]
	internal class ElevatorDoorTriggerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ElevatorDoorTrigger cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ElevatorDoorTrigger obj = (ElevatorDoorTrigger)instance;
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
			obj.Construct(cameraManager, garageController);
		}
	}
}
