using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Flywheel", menuName = "CarPartsTuning/EngineParts/BlockParts/Flywheel", order = 24)]
	public class FlywheelPart : ReciprocatingEnginePart
	{
		[SerializeField]
		private float diameter = 195f;

		public override string GetMaxReliability => $"{maxRPM} {Utils.RPM}";

		public override string GetDescription
		{
			get
			{
				object[] array = new object[4]
				{
					this.GetEngineName(),
					GetMaxReliability,
					maxRPM,
					this.GetPartVendorName()
				};
				return string.Format(Utils.CorrectFormatString(new LocalizedString(Description), array.Length), array);
			}
		}

		public override float GetInertia()
		{
			float num = Utils.Mm2Meter(diameter) * 0.5f;
			return 0.5f * Mass * num * num;
		}
	}
}
