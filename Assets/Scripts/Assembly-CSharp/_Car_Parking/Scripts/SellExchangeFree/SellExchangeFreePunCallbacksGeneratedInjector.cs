using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	[Preserve]
	internal class SellExchangeFreePunCallbacksGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SellExchangeFreePunCallbacks cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SellExchangeFreePunCallbacks obj = (SellExchangeFreePunCallbacks)instance;
			ISellExchangeFreeService sellExchangeFreeService = (ISellExchangeFreeService)resolver.ResolveOrParameter(typeof(ISellExchangeFreeService), "sellExchangeFreeService", parameters);
			MultiPlayer multiPlayer = (MultiPlayer)resolver.ResolveOrParameter(typeof(MultiPlayer), "multiPlayer", parameters);
			obj.Construct(sellExchangeFreeService, multiPlayer);
		}
	}
}
