using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	internal class TuningTimerUiGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:TuningTimerUi cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TuningTimerUi obj = (TuningTimerUi)instance;
			ITimerTuningService timerTuningService = (ITimerTuningService)resolver.ResolveOrParameter(typeof(ITimerTuningService), "timerTuningService", parameters);
			IEngineTuningTimerService engineTuningTimerService = (IEngineTuningTimerService)resolver.ResolveOrParameter(typeof(IEngineTuningTimerService), "engineTuningTimerService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(timerTuningService, engineTuningTimerService, managerUI);
		}
	}
}
