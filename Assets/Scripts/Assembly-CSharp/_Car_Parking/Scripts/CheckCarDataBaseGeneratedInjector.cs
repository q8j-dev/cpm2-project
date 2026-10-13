using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class CheckCarDataBaseGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CheckCarDataBase checkCarDataBase = new CheckCarDataBase();
			Inject(checkCarDataBase, resolver, parameters);
			return checkCarDataBase;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
