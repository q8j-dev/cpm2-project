using System.Collections.Generic;
using VContainer;

namespace CodeBase.Infrastructure.States
{
	[Preserve]
	internal class MainMenuStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MainMenuState mainMenuState = new MainMenuState();
			Inject(mainMenuState, resolver, parameters);
			return mainMenuState;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
