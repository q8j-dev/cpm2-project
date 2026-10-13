using System.Collections.Generic;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public class CarTuningTimer
	{
		public List<PartData> PartsToSet = new List<PartData>();

		public EngineKitData EngineKitData;

		public int SlotId;

		public double TimeOfStart;

		public double TimeOfEnd;

		public string Signature;

		public double AdReductionSeconds;
	}
}
