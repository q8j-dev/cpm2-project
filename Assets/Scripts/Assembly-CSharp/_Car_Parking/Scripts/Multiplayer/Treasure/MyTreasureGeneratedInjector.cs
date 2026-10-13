using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Treasure
{
	[Preserve]
	internal class MyTreasureGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MyTreasure cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MyTreasure obj = (MyTreasure)instance;
			MapControl mapControl = (MapControl)resolver.ResolveOrParameter(typeof(MapControl), "mapControl", parameters);
			GlobalCollectionsProvider globalCollectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "globalCollectionsProvider", parameters);
			MapIconsModel mapIconsModel = (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters);
			obj.Construct(mapControl, globalCollectionsProvider, mapIconsModel);
		}
	}
}
