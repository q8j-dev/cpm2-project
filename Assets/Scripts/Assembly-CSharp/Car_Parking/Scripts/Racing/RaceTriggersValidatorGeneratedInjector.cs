using System;
using System.Collections.Generic;
using VContainer;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	internal class RaceTriggersValidatorGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RaceTriggersValidator cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RaceTriggersValidator obj = (RaceTriggersValidator)instance;
			FreeTimeAttackController timeAttackController = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "timeAttackController", parameters);
			TimeBannersManager timeBannersManager = (TimeBannersManager)resolver.ResolveOrParameter(typeof(TimeBannersManager), "timeBannersManager", parameters);
			obj.Construct(timeAttackController, timeBannersManager);
		}
	}
}
