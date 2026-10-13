using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Preserve]
	internal class BindingsPrefsStoreGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BindingsPrefsStore bindingsPrefsStore = new BindingsPrefsStore();
			Inject(bindingsPrefsStore, resolver, parameters);
			return bindingsPrefsStore;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
