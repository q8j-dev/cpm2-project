using System;
using Gley.TrafficSystem.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using _Car_Parking.Scripts.Steam.Traffic;

internal class __JobReflectionRegistrationOutput__1221673671587648887
{
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<NearbyCarJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<SteamReadSuspensionJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<SteamReadWheelPoseJob>();
			IJobParallelForExtensions.EarlyJobInit<DriveJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<UpdateTriggerJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<UpdateWheelJob>();
			IJobParallelForExtensions.EarlyJobInit<WheelJob>();
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
