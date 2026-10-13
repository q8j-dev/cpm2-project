using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class DragRacingMultiplayerTreeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:DragRacingMultiplayerTree cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DragRacingMultiplayerTree obj = (DragRacingMultiplayerTree)instance;
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			obj.Construct(carInputManager);
		}
	}
}
