using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "EngineId", menuName = "CarPartsTuning/EngineParts/EngineId", order = 1)]
	public class EngineId : PartItem
	{
		public int EngineSound = -1;

		public int StarterSound;

		public EngineId()
		{
			TypeId = PartType.EngineId;
			Mass = 0.1f;
		}
	}
}
