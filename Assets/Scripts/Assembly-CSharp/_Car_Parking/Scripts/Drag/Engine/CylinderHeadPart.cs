using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "CylinderHead", menuName = "CarPartsTuning/EngineParts/CylinderHead", order = 5)]
	public class CylinderHeadPart : ReciprocatingEnginePart
	{
		public enum CylinderHeadTypeEnum
		{
			SOHC = 0,
			DOHC = 1,
			OHV = 2
		}

		[Tooltip("Intake valve diameter in mm")]
		public float intake_valve_diam;

		[Tooltip("Intake valves per cylinder")]
		public float intake_valves_per_cylinder;

		[Tooltip("Exhaust valve diameter in mm")]
		public float exhaust_valve_diam;

		[Tooltip("Exhaust valves per cylinder")]
		public float exhaust_valves_per_cylinder;

		[Tooltip("Combustion chamber volume in cc")]
		public float Vmin;

		public CylinderHeadTypeEnum CylinderHeadType;

		[Header("If OHV head")]
		public float intake_rocker_ratio = 1f;

		public float exhaust_rocker_ratio = 1f;

		public override string GetDescription
		{
			get
			{
				object[] array = new object[4]
				{
					this.GetEngineName(),
					GetMaxReliability,
					$"{maxRPM} {Utils.RPM}",
					this.GetPartVendorName()
				};
				return string.Format(Utils.CorrectFormatString(new LocalizedString(Description), array.Length), array);
			}
		}
	}
}
