using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class AchievementStatusEvent : DefaultEvent<AchievementStatusContext>
	{
		protected override string Name => "achievement_status";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"status",
						base.Context.Status.ToString()
					},
					{
						"achievement_service",
						base.Context.Service.ToString()
					}
				};
				if (!string.IsNullOrEmpty(base.Context.AchievementId))
				{
					dictionary.Add("achievement_id", base.Context.AchievementId);
				}
				if (!string.IsNullOrEmpty(base.Context.PlatformPlayerId))
				{
					dictionary.Add("platform_player_id", base.Context.PlatformPlayerId);
				}
				if (base.Context.FailReason.HasValue)
				{
					dictionary.Add("fail_reason", base.Context.FailReason.Value.ToString());
				}
				if (base.Context.Result.HasValue)
				{
					dictionary.Add("result", base.Context.Result.Value.ToString());
				}
				if (base.Context.ProgressCurrent.HasValue)
				{
					dictionary.Add("progress_current", base.Context.ProgressCurrent.Value);
				}
				if (base.Context.ProgressTarget.HasValue)
				{
					dictionary.Add("progress_target", base.Context.ProgressTarget.Value);
				}
				if (!string.IsNullOrEmpty(base.Context.Source))
				{
					dictionary.Add("source", base.Context.Source);
				}
				if (base.Context.IsRetry.HasValue)
				{
					dictionary.Add("is_retry", base.Context.IsRetry.Value);
				}
				if (!string.IsNullOrEmpty(base.Context.ErrorMessage))
				{
					dictionary.Add("error_message", base.Context.ErrorMessage);
				}
				if (!string.IsNullOrEmpty(base.Context.Properties))
				{
					dictionary.Add("properties", base.Context.Properties);
				}
				return dictionary;
			}
		}

		public AchievementStatusEvent(CPMAnalytics analytics, AchievementStatusContext context)
			: base(analytics, context)
		{
		}
	}
}
