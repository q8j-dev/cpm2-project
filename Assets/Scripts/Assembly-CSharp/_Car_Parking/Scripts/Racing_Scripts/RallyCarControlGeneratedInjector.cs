using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class RallyCarControlGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RallyCarControl cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyCarControl obj = (RallyCarControl)instance;
			PlayerDatas.CarLoadingUtility carLoadingUtility = (PlayerDatas.CarLoadingUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.CarLoadingUtility), "carLoadingUtility", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			RallyCarsDatasSO carsDatasSo = (RallyCarsDatasSO)resolver.ResolveOrParameter(typeof(RallyCarsDatasSO), "carsDatasSo", parameters);
			DrawQueue drawQueue = (DrawQueue)resolver.ResolveOrParameter(typeof(DrawQueue), "drawQueue", parameters);
			VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
			PlayerDatas.VinylsDataUtility vinylsDataUtility = (PlayerDatas.VinylsDataUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.VinylsDataUtility), "vinylsDataUtility", parameters);
			QuickCarSettings quickCarSettings = (QuickCarSettings)resolver.ResolveOrParameter(typeof(QuickCarSettings), "quickCarSettings", parameters);
			AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
			obj.Construct(carLoadingUtility, prefabFactory, carsDatasSo, drawQueue, vinylDrawer, vinylsDataUtility, quickCarSettings, allCarsInfo);
		}
	}
}
