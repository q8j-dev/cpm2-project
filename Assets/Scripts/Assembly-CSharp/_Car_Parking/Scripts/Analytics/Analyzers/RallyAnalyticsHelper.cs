using System.Collections.Generic;
using CPMEngine;
using Newtonsoft.Json;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Rally.Data;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public static class RallyAnalyticsHelper
	{
		public static string BuildRewardJson(RallyMedalType prevMedal, RallyMedalType medal, RallyTimes[] times, bool awardSpecific = false, RallySequenceData.RewardData reward = null)
		{
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			if (times != null && times.Length >= 3)
			{
				for (int i = (int)(prevMedal + 1); i <= (int)medal; i++)
				{
					int num = 3 - i;
					if (num >= 0 && num < times.Length)
					{
						RallyTimes rallyTimes = times[num];
						Dictionary<string, object> obj = new Dictionary<string, object>
						{
							{
								"item_type",
								rallyTimes.IsCoin ? "HARD" : "SOFT"
							},
							{
								"value",
								rallyTimes.Price.ToString()
							}
						};
						RallyMedalType rallyMedalType = (RallyMedalType)i;
						obj.Add("medal", rallyMedalType.ToString());
						list.Add(obj);
					}
				}
			}
			if (awardSpecific && reward != null && reward.HaveReward)
			{
				RewardItemData[] rewardItems = reward.RewardItems;
				foreach (RewardItemData rewardItemData in rewardItems)
				{
					if (rewardItemData != null)
					{
						list.Add(rewardItemData.ToAnalyticsDictionary());
					}
				}
			}
			return JsonConvert.SerializeObject(list);
		}

		public static string TyreTypeToCanonical(TyreType tyre)
		{
			return tyre switch
			{
				TyreType.Touring => "touring", 
				TyreType.SemiSlick => "semslick", 
				TyreType.Slick => "slick", 
				TyreType.Offroad => "offroad", 
				TyreType.Winter => "winter", 
				TyreType.Buggy => "buggy", 
				_ => "touring", 
			};
		}

		public static RallyStageType MapStageType(bool isNight)
		{
			if (!isNight)
			{
				return RallyStageType.full_stage;
			}
			return RallyStageType.night_event;
		}

		public static RallyMedalType MapMedalTier(int tier)
		{
			return tier switch
			{
				1 => RallyMedalType.BRONZE, 
				2 => RallyMedalType.SILVER, 
				3 => RallyMedalType.GOLD, 
				_ => RallyMedalType.NONE, 
			};
		}

		public static RallyMedalType MapLapTimeToMedal(float lapTime, RallyTimes[] times)
		{
			if (times == null || times.Length < 3)
			{
				return RallyMedalType.NONE;
			}
			if (lapTime <= times[0].TargetTime)
			{
				return RallyMedalType.GOLD;
			}
			if (lapTime <= times[1].TargetTime)
			{
				return RallyMedalType.SILVER;
			}
			if (lapTime <= times[2].TargetTime)
			{
				return RallyMedalType.BRONZE;
			}
			return RallyMedalType.NONE;
		}

		public static RallyFailReason MapDisqualify(RallyDisqualifyType type)
		{
			switch (type)
			{
			case RallyDisqualifyType.StopCar:
				return RallyFailReason.LEAVE_DIRECTION;
			case RallyDisqualifyType.WrongWay:
			case RallyDisqualifyType.TouchMarshall:
			case RallyDisqualifyType.NotStarted:
				return RallyFailReason.DISQUALIFIED;
			default:
				return RallyFailReason.DISQUALIFIED;
			}
		}

		public static string BuildCampaignId(string locationName, int locationIndex)
		{
			return $"{Prefix(locationName)}campaign_{locationIndex}";
		}

		public static string BuildSeriesId(string locationName, int trajectoryIndex)
		{
			return $"{Prefix(locationName)}series_{trajectoryIndex}";
		}

		public static string BuildEventId(string locationName, int trajectoryIndex)
		{
			return $"{Prefix(locationName)}event_{trajectoryIndex}";
		}

		private static string Prefix(string locationName)
		{
			if (!string.IsNullOrEmpty(locationName))
			{
				return locationName + "_";
			}
			return "";
		}
	}
}
