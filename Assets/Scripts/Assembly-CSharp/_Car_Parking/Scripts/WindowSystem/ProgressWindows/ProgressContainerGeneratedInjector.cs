using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem.ProgressWindows
{
	[Preserve]
	internal class ProgressContainerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ProgressContainer progressContainer = new ProgressContainer();
			Inject(progressContainer, resolver, parameters);
			return progressContainer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
