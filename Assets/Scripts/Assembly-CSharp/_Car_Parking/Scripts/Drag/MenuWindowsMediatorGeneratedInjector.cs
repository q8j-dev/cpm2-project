using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class MenuWindowsMediatorGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MenuWindowsMediator menuWindowsMediator = new MenuWindowsMediator();
			Inject(menuWindowsMediator, resolver, parameters);
			return menuWindowsMediator;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			((MenuWindowsMediator)instance).InventoryPartsService = (IInventoryPartsService)resolver.ResolveOrParameter(typeof(IInventoryPartsService), "InventoryPartsService", parameters);
		}
	}
}
