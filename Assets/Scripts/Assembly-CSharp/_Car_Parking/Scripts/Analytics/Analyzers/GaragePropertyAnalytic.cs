using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Database;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class GaragePropertyAnalytic : AnalyticProviderBase
	{
		public GaragePropertyAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void Log(GaragePropertyActionType action, ResponseCode responseCode, int garageIndex, bool isConsumable, int rewardSoft, int rewardHard, bool legacySellBlock)
		{
			GaragePropertyActionResult result = ((responseCode != ResponseCode.Success) ? GaragePropertyActionResult.FAIL : GaragePropertyActionResult.SUCCESS);
			GarageSourceType garageSourceType = ((!isConsumable) ? GarageSourceType.NON_CONSUMABLE : GarageSourceType.CONSUMABLE);
			if (!legacySellBlock)
			{
				garageSourceType = GarageSourceType.UNKNOWN;
			}
			base.AnalyticEventFactory.CreateEvent<GaragePropertyActionEvent, GaragePropertyActionContext>(new GaragePropertyActionContext
			{
				Action = action,
				Result = result,
				GarageId = garageIndex.ToString(),
				GarageSourceType = garageSourceType,
				RewardSoft = rewardSoft,
				RewardHard = rewardHard,
				FailReason = MapResponseCode(responseCode),
				LegacySellBlock = legacySellBlock
			}).Send();
		}

		public static GaragePropertyActionFailReason MapResponseCode(ResponseCode code)
		{
			return code switch
			{
				ResponseCode.GarageForSaleNotFound => GaragePropertyActionFailReason.DENIED_INVALID_GARAGE, 
				ResponseCode.TransferImpossible => GaragePropertyActionFailReason.DENIED_HAS_CARS, 
				ResponseCode.Unknown => GaragePropertyActionFailReason.DENIED_UNKNOWN, 
				ResponseCode.Success => GaragePropertyActionFailReason.NONE, 
				_ => GaragePropertyActionFailReason.DENIED_UNKNOWN, 
			};
		}
	}
}
