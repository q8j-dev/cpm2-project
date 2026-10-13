using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Serializable]
	public class EngineTuningTimer : IComparable<EngineTuningTimer>
	{
		public List<PartData> PartsToSet = new List<PartData>();

		public int EngineId;

		public double TimeOfStart;

		public double TimeOfEnd;

		public double AdReductionSeconds;

		public string Signature;

		public int CompareTo(EngineTuningTimer other)
		{
			return TimeOfEnd.CompareTo(other.TimeOfEnd);
		}
	}
}
