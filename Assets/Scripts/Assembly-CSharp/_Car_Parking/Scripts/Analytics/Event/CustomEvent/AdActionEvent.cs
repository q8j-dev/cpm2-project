using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class AdActionEvent : DefaultEvent<AdActionContext>
	{
		protected override string Name => "ad_action";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"status",
						base.Context.Status
					},
					{
						"ad_id",
						base.Context.AdId
					},
					{
						"ad_placement",
						base.Context.AdPlacement
					},
					{
						"ad_format",
						base.Context.AdFormat
					}
				};
				if (!string.IsNullOrEmpty(base.Context.AdServerId))
				{
					dictionary.Add("ad_server_id", base.Context.AdServerId);
				}
				if (!string.IsNullOrEmpty(base.Context.Result))
				{
					dictionary.Add("result", base.Context.Result);
				}
				if (!string.IsNullOrEmpty(base.Context.ErrorCode))
				{
					dictionary.Add("error_code", base.Context.ErrorCode);
				}
				if (!string.IsNullOrEmpty(base.Context.ErrorMessage))
				{
					dictionary.Add("error_message", base.Context.ErrorMessage);
				}
				if (!string.IsNullOrEmpty(base.Context.AdNetwork))
				{
					dictionary.Add("ad_network", base.Context.AdNetwork);
				}
				if (!string.IsNullOrEmpty(base.Context.AdCreativeId))
				{
					dictionary.Add("ad_creative_id", base.Context.AdCreativeId);
				}
				if (base.Context.AdRevenueUsd.HasValue)
				{
					dictionary.Add("ad_revenue_usd", base.Context.AdRevenueUsd.Value);
				}
				if (base.Context.AdCpm.HasValue)
				{
					dictionary.Add("ad_cpm", base.Context.AdCpm.Value);
				}
				if (!string.IsNullOrEmpty(base.Context.AdRevenuePrecision))
				{
					dictionary.Add("ad_revenue_precision", base.Context.AdRevenuePrecision);
				}
				if (!string.IsNullOrEmpty(base.Context.RewardId))
				{
					dictionary.Add("reward_id", base.Context.RewardId);
				}
				if (!string.IsNullOrEmpty(base.Context.RewardType))
				{
					dictionary.Add("reward_type", base.Context.RewardType);
				}
				if (base.Context.Amount.HasValue)
				{
					dictionary.Add("amount", base.Context.Amount.Value);
				}
				if (base.Context.RewardSoft.HasValue)
				{
					dictionary.Add("reward_soft", base.Context.RewardSoft.Value);
				}
				if (base.Context.RewardHard.HasValue)
				{
					dictionary.Add("reward_hard", base.Context.RewardHard.Value);
				}
				if (base.Context.RewardReceived.HasValue)
				{
					dictionary.Add("reward_received", base.Context.RewardReceived.Value);
				}
				if (base.Context.Price.HasValue)
				{
					dictionary.Add("price", base.Context.Price.Value);
				}
				if (!string.IsNullOrEmpty(base.Context.PriceCurrency))
				{
					dictionary.Add("price_currency", base.Context.PriceCurrency);
				}
				if (!string.IsNullOrEmpty(base.Context.GameMode))
				{
					dictionary.Add("game_mode", base.Context.GameMode);
				}
				if (!string.IsNullOrEmpty(base.Context.WindowId))
				{
					dictionary.Add("window_id", base.Context.WindowId);
				}
				string value = BuildProperties();
				if (!string.IsNullOrEmpty(value))
				{
					dictionary.Add("properties", value);
				}
				return dictionary;
			}
		}

		private string BuildProperties()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (base.Context.BalanceVersion.HasValue)
			{
				dictionary["balance_version"] = base.Context.BalanceVersion.Value;
			}
			if (!string.IsNullOrEmpty(base.Context.SourceUnitId))
			{
				dictionary["source_unit_id"] = base.Context.SourceUnitId;
			}
			if (!string.IsNullOrEmpty(base.Context.RaceMode))
			{
				dictionary["race_mode"] = base.Context.RaceMode;
			}
			if (dictionary.Count <= 0)
			{
				return null;
			}
			return JsonConvert.SerializeObject(dictionary);
		}

		public AdActionEvent(CPMAnalytics analytics, AdActionContext context)
			: base(analytics, context)
		{
		}
	}
}
