using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class VisualTuningViewModelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualTuningViewModel visualTuningViewModel = new VisualTuningViewModel();
			Inject(visualTuningViewModel, resolver, parameters);
			return visualTuningViewModel;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualTuningViewModel obj = (VisualTuningViewModel)instance;
			VisualTuning visualTuning = (VisualTuning)resolver.ResolveOrParameter(typeof(VisualTuning), "visualTuning", parameters);
			obj.Construct(visualTuning);
		}
	}
}
