using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Free_drive
{
	[Preserve]
	internal class FreeDrivePrefabSpawnerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FreeDrivePrefabSpawner cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeDrivePrefabSpawner obj = (FreeDrivePrefabSpawner)instance;
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			IGameFactManager gameFactManager = (IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "gameFactManager", parameters);
			obj.Construct(prefabFactory, gameFactManager);
		}
	}
}
