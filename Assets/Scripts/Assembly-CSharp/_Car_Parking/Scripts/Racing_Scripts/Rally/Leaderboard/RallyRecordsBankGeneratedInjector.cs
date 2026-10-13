using System.Collections.Generic;
using CPM.RacingRecords;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	[Preserve]
	internal class RallyRecordsBankGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyRecordsBank rallyRecordsBank = new RallyRecordsBank((RacingRecordsCache)resolver.ResolveOrParameter(typeof(RacingRecordsCache), "cache", parameters));
			Inject(rallyRecordsBank, resolver, parameters);
			return rallyRecordsBank;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
