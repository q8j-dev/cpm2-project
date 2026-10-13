using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	internal class UITouchHelperGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UITouchHelper uITouchHelper = new UITouchHelper();
			Inject(uITouchHelper, resolver, parameters);
			return uITouchHelper;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
