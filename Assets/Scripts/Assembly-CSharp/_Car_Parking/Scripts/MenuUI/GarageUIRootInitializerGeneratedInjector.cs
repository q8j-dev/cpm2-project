using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class GarageUIRootInitializerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GarageUIRootInitializer cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageUIRootInitializer obj = (GarageUIRootInitializer)instance;
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			obj.Construct(menuLevelParent);
		}
	}
}
