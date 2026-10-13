using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "TurboCharger", menuName = "CarPartsTuning/EngineParts/Chargers/TurboCharger", order = 21)]
	public class TurboChargerPart : ChargerPart
	{
		public override string GetDescription
		{
			get
			{
				object[] array = new object[4]
				{
					this.GetEngineName(),
					GetMaxReliability,
					ChargerTypeText,
					this.GetPartVendorName()
				};
				return string.Format(Utils.CorrectFormatString(new LocalizedString(Description), array.Length), array);
			}
		}

		private string ChargerTypeText => base.GetChargerType switch
		{
			TurboType.SingleTurboTwinScroll => new LocalizedString("TurboType.SingleTurbo"), 
			TurboType.SingleTurboMonoScroll => new LocalizedString("TurboType.SingleTurbo"), 
			TurboType.TwinTurboSingleScroll => new LocalizedString("TurboType.TwinTurbo"), 
			TurboType.TwinTurboTwinScroll => new LocalizedString("TurboType.TwinTurbo"), 
			TurboType.BiTurbo => new LocalizedString("TurboType.BiTurbo"), 
			TurboType.QuadTurboCharger => new LocalizedString("TurboType.QuadTurbo"), 
			_ => string.Empty, 
		};

		public TurboChargerPart()
		{
			TypeId = PartType.TurboCharger;
			Mass = 10f;
		}
	}
}
