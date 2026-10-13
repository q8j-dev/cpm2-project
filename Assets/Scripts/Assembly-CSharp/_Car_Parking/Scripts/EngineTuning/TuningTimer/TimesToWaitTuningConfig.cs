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

		private int _int1;

		private int _int2;

		private int _int3;

		private List<WaitTimer> _times;

		private List<WaitTimer> _times2;

		private float _ttimes1;

		private float _ttimes4;

		public List<WaitTimer> Times;

		private float _ttimes3;

		private List<WaitTimer> _times1;

		private float _ttimes2;

		private List<WaitTimer> _times3;

		[Preserve]
		public TimesToWaitTuningConfig()
		{
		}
	}
}
