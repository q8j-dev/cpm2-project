using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class VisualTuningViewElementGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VisualTuningViewElement cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualTuningViewElement obj = (VisualTuningViewElement)instance;
			IVisualTuningViewModel visualTuningViewModel = (IVisualTuningViewModel)resolver.ResolveOrParameter(typeof(IVisualTuningViewModel), "visualTuningViewModel", parameters);
			IVisualTuningViewModelComplexOne visualTuningViewModelComplexOne = (IVisualTuningViewModelComplexOne)resolver.ResolveOrParameter(typeof(IVisualTuningViewModelComplexOne), "visualTuningViewModelComplexOne", parameters);
			IVisualTuningViewModelComplexParams visualTuningViewModelComplexParams = (IVisualTuningViewModelComplexParams)resolver.ResolveOrParameter(typeof(IVisualTuningViewModelComplexParams), "visualTuningViewModelComplexParams", parameters);
			obj.Construct(visualTuningViewModel, visualTuningViewModelComplexOne, visualTuningViewModelComplexParams);
		}
	}
}
