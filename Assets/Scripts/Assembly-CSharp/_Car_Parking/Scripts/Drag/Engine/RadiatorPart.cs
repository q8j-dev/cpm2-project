using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Radiator", menuName = "CarPartsTuning/EngineParts/Cooling/Radiator", order = 30)]
	public class RadiatorPart : PartItem
	{
		[SerializeField]
		private float CoolingCapacity = -1f;

		public float GetCoolingCapacity
		{
			get
			{
				if (!(CoolingCapacity >= 0f))
				{
					return EngineThermal.CoolingCapacityFromRating(base.GetReliability);
				}
				return CoolingCapacity;
			}
		}

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;

		public RadiatorPart()
		{
			TypeId = PartType.Intercooler;
			Mass = 5f;
		}
	}
}
