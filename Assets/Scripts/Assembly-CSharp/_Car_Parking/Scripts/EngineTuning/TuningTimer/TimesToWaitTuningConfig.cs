using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[CreateAssetMenu(fileName = "TimesToWaitConfig", menuName = "Scriptables/TimesToWaitConfig", order = 12)]
	[Preserve]
	public class TimesToWaitTuningConfig : ScriptableObject
	{
		public const float DefaultTime = 5f;

		public const float DefaultEngineTime = 600f;

		public List<WaitTimer> Times;

		[Preserve]
		public TimesToWaitTuningConfig()
		{
		}
	}
}
