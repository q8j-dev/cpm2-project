using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.AssetBundles
{
	[Preserve]
	internal class AssetBundleManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AssetBundleManager assetBundleManager = new AssetBundleManager();
			Inject(assetBundleManager, resolver, parameters);
			return assetBundleManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
