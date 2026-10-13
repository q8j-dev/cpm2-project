using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Rotor", menuName = "CarPartsTuning/EngineParts/RotorParts/Rotor", order = 22)]
	public class RotorPart : ReciprocatingEnginePart
	{
		[SerializeField]
		[Range(0.6f, 1.5f)]
		[Tooltip("Compression ratio factor")]
		private float CompressionRatioFactor = 1f;

		[SerializeField]
		[Tooltip("Port shape efficiency factor")]
		[Range(0.7f, 0.95f)]
		internal float PortEfficiency = 0.85f;

		[Tooltip("Thermal expansion coefficient")]
		[Range(1f, 1.2f)]
		[SerializeField]
		internal float ThermalExpansion = 1.05f;

		public override string GetDescription
		{
			get
			{
				object[] array = new object[3]
				{
					this.GetEngineName(),
					GetMaxReliability,
					this.GetPartVendorName()
				};
				return string.Format(Utils.CorrectFormatString(new LocalizedString(Description), array.Length), array);
			}
		}

		internal float GetCompressionRatioFactor => CompressionRatioFactor;
	}
}
