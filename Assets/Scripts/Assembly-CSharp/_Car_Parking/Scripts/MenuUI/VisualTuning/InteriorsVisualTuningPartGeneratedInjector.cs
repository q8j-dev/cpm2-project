using System.Collections.Generic;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class InteriorsVisualTuningPartGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InteriorsVisualTuningPart interiorsVisualTuningPart = new InteriorsVisualTuningPart();
			Inject(interiorsVisualTuningPart, resolver, parameters);
			return interiorsVisualTuningPart;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InteriorsVisualTuningPart obj = (InteriorsVisualTuningPart)instance;
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			AssetBundleManager assetBundleManager = (AssetBundleManager)resolver.ResolveOrParameter(typeof(AssetBundleManager), "assetBundleManager", parameters);
			obj.Construct(prefabFactory, cameraManager, assetBundleManager);
		}
	}
}
