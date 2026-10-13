using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace Gley.TrafficSystem
{
	[Preserve]
	internal class TrafficComponentGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:TrafficComponent cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TrafficComponent obj = (TrafficComponent)instance;
			OptionalDependency<MapIconsModel> mapIconsModel = (OptionalDependency<MapIconsModel>)resolver.ResolveOrParameter(typeof(OptionalDependency<MapIconsModel>), "mapIconsModel", parameters);
			OptionalDependency<GlobalCollectionsProvider> collectionsProvider = (OptionalDependency<GlobalCollectionsProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<GlobalCollectionsProvider>), "collectionsProvider", parameters);
			OptionalDependency<FreeDriveProvider> freeDriveProvider = (OptionalDependency<FreeDriveProvider>)resolver.ResolveOrParameter(typeof(OptionalDependency<FreeDriveProvider>), "freeDriveProvider", parameters);
			SceneControl sceneControl = (SceneControl)resolver.ResolveOrParameter(typeof(SceneControl), "sceneControl", parameters);
			SoundBankLoadService soundBankService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankService", parameters);
			obj.Construct(mapIconsModel, collectionsProvider, freeDriveProvider, sceneControl, soundBankService);
		}
	}
}
