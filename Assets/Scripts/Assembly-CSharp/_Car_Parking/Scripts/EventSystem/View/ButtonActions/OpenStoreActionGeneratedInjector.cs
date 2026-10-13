using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	[Preserve]
	internal class OpenStoreActionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OpenStoreAction openStoreAction = new OpenStoreAction((SteamStorePage)resolver.ResolveOrParameter(typeof(SteamStorePage), "steamStorePage", parameters));
			Inject(openStoreAction, resolver, parameters);
			return openStoreAction;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
