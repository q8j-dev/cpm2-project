using System.Collections.Generic;
using VContainer;

namespace Services.PriceStyleService
{
	[Preserve]
	internal class PriceStyleConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PriceStyleConfig priceStyleConfig = new PriceStyleConfig();
			Inject(priceStyleConfig, resolver, parameters);
			return priceStyleConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
