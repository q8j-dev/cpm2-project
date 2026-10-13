using System;
using System.Collections.Generic;
using VContainer;

namespace CPM.RacingRecords
{
	[Preserve]
	internal class RacingRecordsCacheGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RacingRecordsCache racingRecordsCache = new RacingRecordsCache((IRacingRecordsBackend)resolver.ResolveOrParameter(typeof(IRacingRecordsBackend), "backend", parameters), (string)resolver.ResolveOrParameter(typeof(string), "mode", parameters), (int)resolver.ResolveOrParameter(typeof(int), "count", parameters), (double)resolver.ResolveOrParameter(typeof(double), "ttlSeconds", parameters), (Func<double>)resolver.ResolveOrParameter(typeof(Func<double>), "now", parameters));
			Inject(racingRecordsCache, resolver, parameters);
			return racingRecordsCache;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
