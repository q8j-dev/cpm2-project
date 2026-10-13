using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	internal class InjectableSceneObjectSpawnerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:InjectableSceneObjectSpawner cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InjectableSceneObjectSpawner obj = (InjectableSceneObjectSpawner)instance;
			OptionalDependency<IPrefabFactory> prefabFactory = (OptionalDependency<IPrefabFactory>)resolver.ResolveOrParameter(typeof(OptionalDependency<IPrefabFactory>), "prefabFactory", parameters);
			IGameFactManager gameFactManager = (IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "gameFactManager", parameters);
			obj.Construct(prefabFactory, gameFactManager);
		}
	}
}
