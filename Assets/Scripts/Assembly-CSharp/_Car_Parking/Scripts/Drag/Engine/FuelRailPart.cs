using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "FuelRailPart", menuName = "CarPartsTuning/EngineParts/AirFuel/FuelRail", order = 22)]
	public class FuelRailPart : AirFuelDeliverySystem
	{
		[SerializeField]
		private FuelType FuelType;

		[SerializeField]
		private float maxFuelConsumption = 0.01f;

		[SerializeField]
		private float efficiency;

		public override string GetDescription
		{
			get
			{
				object[] array = new object[4]
				{
					this.GetEngineName(),
					GetMaxReliability,
					FuelName,
					this.GetPartVendorName()
				};
				return string.Format(Utils.CorrectFormatString(new LocalizedString(Description), array.Length), array);
			}
		}

		private string FuelName => FuelType switch
		{
			FuelType.None => string.Empty, 
			FuelType.Gas95 => new LocalizedString("FuelType.Gas95"), 
			FuelType.Gas98 => new LocalizedString("FuelType.Gas98"), 
			FuelType.Gas100 => new LocalizedString("FuelType.Gas100"), 
			FuelType.GasE85 => new LocalizedString("FuelType.GasE85"), 
			FuelType.Diesel => new LocalizedString("FuelType.Diesel"), 
			_ => string.Empty, 
		};

		public float GetEfficiency => Mathf.Clamp01(efficiency);

		public float GetMFC => maxFuelConsumption;

		public FuelType GetFT => FuelType;

		public FuelRailPart()
		{
			efficiency = 1f;
			TypeId = PartType.FuelRail;
			Mass = 1f;
		}
	}
}
