using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.CPMRating.DailyTasks.Scripts;
using _Car_Parking.Scripts.Providers;

namespace CPMEngine
{
	[Preserve]
	internal class PowertrainGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:Powertrain cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			Powertrain obj = (Powertrain)instance;
			CPMVehicleCore cpmVehicleCore = (CPMVehicleCore)resolver.ResolveOrParameter(typeof(CPMVehicleCore), "cpmVehicleCore", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			DailyTasksInfoContainer dailyTasksInfoContainer = (DailyTasksInfoContainer)resolver.ResolveOrParameter(typeof(DailyTasksInfoContainer), "dailyTasksInfoContainer", parameters);
			SoundBankLoadService soundBankService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankService", parameters);
			obj.Construct(cpmVehicleCore, freeDriveProvider, dailyTasksInfoContainer, soundBankService);
		}
	}
}
