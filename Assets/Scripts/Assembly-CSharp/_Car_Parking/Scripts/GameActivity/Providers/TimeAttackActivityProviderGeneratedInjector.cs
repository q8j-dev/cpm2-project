using System.Collections.Generic;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;

namespace _Car_Parking.Scripts.GameActivity.Providers
{
	[Preserve]
	internal class TimeAttackActivityProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TimeAttackActivityProvider timeAttackActivityProvider = new TimeAttackActivityProvider((TimeAttackDataSO)resolver.ResolveOrParameter(typeof(TimeAttackDataSO), "data", parameters));
			Inject(timeAttackActivityProvider, resolver, parameters);
			return timeAttackActivityProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
