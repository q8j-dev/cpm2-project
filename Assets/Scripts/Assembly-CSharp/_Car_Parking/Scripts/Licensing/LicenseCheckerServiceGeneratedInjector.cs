using System.Collections.Generic;
using CPM.Connection;
using VContainer;

namespace _Car_Parking.Scripts.Licensing
{
	[Preserve]
	internal class LicenseCheckerServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LicenseCheckerService licenseCheckerService = new LicenseCheckerService((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters));
			Inject(licenseCheckerService, resolver, parameters);
			return licenseCheckerService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
