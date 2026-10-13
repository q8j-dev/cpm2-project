using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.CDN
{
	[Preserve]
	internal class CDNDownloaderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CDNDownloader cDNDownloader = new CDNDownloader();
			Inject(cDNDownloader, resolver, parameters);
			return cDNDownloader;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
