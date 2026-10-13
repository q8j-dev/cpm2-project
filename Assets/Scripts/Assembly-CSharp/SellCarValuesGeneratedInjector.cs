using System;
using System.Collections.Generic;
using CPM.Scripts.VinylDrawer;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

[Preserve]
internal class SellCarValuesGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:SellCarValues cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		SellCarValues obj = (SellCarValues)instance;
		DrawQueue drawQueue = (DrawQueue)resolver.ResolveOrParameter(typeof(DrawQueue), "drawQueue", parameters);
		VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
		PlayerDatas.VinylsDataUtility vinylsDataUtility = (PlayerDatas.VinylsDataUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.VinylsDataUtility), "vinylsDataUtility", parameters);
		ICarTuningFactory carTuningFactory = (ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters);
		DefaultMaterialsConfig defaultMaterialsConfig = (DefaultMaterialsConfig)resolver.ResolveOrParameter(typeof(DefaultMaterialsConfig), "defaultMaterialsConfig", parameters);
		CarPriceHelperV2 carPriceHelperV = (CarPriceHelperV2)resolver.ResolveOrParameter(typeof(CarPriceHelperV2), "carPriceHelperV2", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		obj.Construct(drawQueue, vinylDrawer, vinylsDataUtility, carTuningFactory, defaultMaterialsConfig, carPriceHelperV, prefabFactory);
	}
}
