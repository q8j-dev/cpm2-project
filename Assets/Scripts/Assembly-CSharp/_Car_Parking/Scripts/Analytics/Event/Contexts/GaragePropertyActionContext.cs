using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class GaragePropertyActionContext : IEventContext
	{
		public GaragePropertyActionType Action { get; set; }

		public GaragePropertyActionResult Result { get; set; }

		public string GarageId { get; set; }

		public GarageSourceType GarageSourceType { get; set; }

		public int RewardSoft { get; set; }

		public int RewardHard { get; set; }

		public GaragePropertyActionFailReason FailReason { get; set; }

		public bool LegacySellBlock { get; set; }
	}
}
