using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Trade
{
	[Preserve]
	internal class P2PAntiSpamServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			P2PAntiSpamService p2PAntiSpamService = new P2PAntiSpamService();
			Inject(p2PAntiSpamService, resolver, parameters);
			return p2PAntiSpamService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
