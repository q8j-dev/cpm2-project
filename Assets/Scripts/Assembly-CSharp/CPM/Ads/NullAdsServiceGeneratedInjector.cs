using System.Collections.Generic;
using VContainer;

namespace CPM.Ads
{
	[Preserve]
	internal class NullAdsServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			NullAdsService nullAdsService = new NullAdsService();
			Inject(nullAdsService, resolver, parameters);
			return nullAdsService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
