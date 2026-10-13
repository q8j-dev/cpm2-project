using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Preserve]
	internal class BusinessRootGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:BusinessRoot cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BusinessRoot obj = (BusinessRoot)instance;
			IBusinessService businessService = (IBusinessService)resolver.ResolveOrParameter(typeof(IBusinessService), "businessService", parameters);
			FreeDriveControl freeDriveControl = (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters);
			obj.Construct(businessService, freeDriveControl);
		}
	}
}
