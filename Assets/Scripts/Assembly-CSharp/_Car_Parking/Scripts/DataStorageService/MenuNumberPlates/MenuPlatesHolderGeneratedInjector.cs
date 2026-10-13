using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DataStorageService.MenuNumberPlates
{
	[Preserve]
	internal class MenuPlatesHolderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MenuPlatesHolder menuPlatesHolder = new MenuPlatesHolder();
			Inject(menuPlatesHolder, resolver, parameters);
			return menuPlatesHolder;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
