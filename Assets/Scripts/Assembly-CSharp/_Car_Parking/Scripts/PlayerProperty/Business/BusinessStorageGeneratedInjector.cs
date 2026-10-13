using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Preserve]
	internal class BusinessStorageGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BusinessStorage businessStorage = new BusinessStorage();
			Inject(businessStorage, resolver, parameters);
			return businessStorage;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
