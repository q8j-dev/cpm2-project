using System.Collections.Generic;
using CPM.PlayerData;
using CPM.RacingRecords;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	[Preserve]
	internal class RallyRecordsSyncGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyRecordsSync rallyRecordsSync = new RallyRecordsSync((IRacingRecordsBackend)resolver.ResolveOrParameter(typeof(IRacingRecordsBackend), "backend", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerData", parameters));
			Inject(rallyRecordsSync, resolver, parameters);
			return rallyRecordsSync;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
