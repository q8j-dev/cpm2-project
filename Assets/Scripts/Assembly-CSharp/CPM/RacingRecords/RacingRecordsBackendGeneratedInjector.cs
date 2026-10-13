using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace CPM.RacingRecords
{
	[Preserve]
	internal class RacingRecordsBackendGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RacingRecordsBackend racingRecordsBackend = new RacingRecordsBackend((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(racingRecordsBackend, resolver, parameters);
			return racingRecordsBackend;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
