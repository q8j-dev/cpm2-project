using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class LockCarButtonControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:LockCarButtonController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LockCarButtonController obj = (LockCarButtonController)instance;
			OptionalDependency<MultiPlayer> multiplayer = (OptionalDependency<MultiPlayer>)resolver.ResolveOrParameter(typeof(OptionalDependency<MultiPlayer>), "multiplayer", parameters);
			obj.Construct(multiplayer);
		}
	}
}
