using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace CarAnimationScripts
{
	[Preserve]
	internal class CarAnimationsControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:CarAnimationsController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarAnimationsController obj = (CarAnimationsController)instance;
			OptionalDependency<MultiPlayer> multiPlayer = (OptionalDependency<MultiPlayer>)resolver.ResolveOrParameter(typeof(OptionalDependency<MultiPlayer>), "multiPlayer", parameters);
			obj.Construct(multiPlayer);
		}
	}
}
