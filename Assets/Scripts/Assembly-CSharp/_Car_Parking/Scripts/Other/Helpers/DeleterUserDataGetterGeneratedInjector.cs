using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	internal class DeleterUserDataGetterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DeleterUserDataGetter deleterUserDataGetter = new DeleterUserDataGetter();
			Inject(deleterUserDataGetter, resolver, parameters);
			return deleterUserDataGetter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
