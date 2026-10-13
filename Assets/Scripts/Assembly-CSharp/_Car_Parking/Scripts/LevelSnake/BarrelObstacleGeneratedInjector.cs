using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.LevelSnake
{
	[Preserve]
	internal class BarrelObstacleGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:BarrelObstacle cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BarrelObstacle obj = (BarrelObstacle)instance;
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			obj.Construct(menuLevelParent);
		}
	}
}
