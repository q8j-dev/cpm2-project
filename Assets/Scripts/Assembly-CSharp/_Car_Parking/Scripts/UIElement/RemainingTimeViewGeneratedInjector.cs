using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class RemainingTimeViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RemainingTimeView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RemainingTimeView obj = (RemainingTimeView)instance;
			ServerTimeService serverTimeService = (ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters);
			obj.Construct(serverTimeService);
		}
	}
}
