using System;
using CPM.Vehicle.Damage;
using Unity.Jobs;
using UnityEngine;

internal class __JobReflectionRegistrationOutput__18197555335958370282
{
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<SeedJob>();
			IJobExtensions.EarlyJobInit<RelaxJob>();
			IJobParallelForExtensions.EarlyJobInit<ClampAndNoiseJob>();
			IJobParallelForExtensions.EarlyJobInit<ScatterJob>();
			IJobExtensions.EarlyJobInit<NormalsJob>();
			IJobExtensions.EarlyJobInit<RepairJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		CreateJobReflectionData();
	}
}
