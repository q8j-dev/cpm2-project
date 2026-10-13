using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	[Preserve]
	internal class RamHelperGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RamHelper ramHelper = new RamHelper();
			Inject(ramHelper, resolver, parameters);
			return ramHelper;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
