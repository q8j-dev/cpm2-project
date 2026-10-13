using System.Collections.Generic;
using CPM.Database;
using VContainer;

namespace _Car_Parking.Scripts.Steam.Graphics
{
	[Preserve]
	internal class SteamCarPreviewPreloadServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamCarPreviewPreloadService steamCarPreviewPreloadService = new SteamCarPreviewPreloadService((AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters));
			Inject(steamCarPreviewPreloadService, resolver, parameters);
			return steamCarPreviewPreloadService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
