using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	[Preserve]
	internal class FsoStrobeLightsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FsoStrobeLights cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FsoStrobeLights obj = (FsoStrobeLights)instance;
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(slotService, carInfoService, menuLevelParent, freeDriveProvider);
		}
	}
}
