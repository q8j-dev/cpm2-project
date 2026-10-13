using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class MatchmakingLobbyEvent : DefaultEvent<MatchmakingLobbyContext>
	{
		protected override string Name => "matchmaking_lobby";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"status",
						base.Context.Status.GetEnumMemberValue()
					},
					{
						"game_mode",
						base.Context.GameMode
					}
				};
				AddIfPresent(dictionary, "lobby_id", base.Context.LobbyId);
				AddIfPresent(dictionary, "game_event_id", base.Context.GameEventId);
				AddIfPresent(dictionary, "event_variant", base.Context.EventVariant);
				AddIfPresent(dictionary, "region", base.Context.Region);
				AddIfPresent(dictionary, "race_id", base.Context.RaceId);
				AddIfPresent(dictionary, "arena_id", base.Context.ArenaId);
				AddIfPresent(dictionary, "error_code", base.Context.ErrorCode);
				AddIfPresent(dictionary, "error_message", base.Context.ErrorMessage);
				if (base.Context.EntryPath.HasValue)
				{
					dictionary.Add("entry_path", base.Context.EntryPath.Value.GetEnumMemberValue());
				}
				if (base.Context.Visibility.HasValue)
				{
					dictionary.Add("visibility", base.Context.Visibility.Value.GetEnumMemberValue());
				}
				if (base.Context.FailReason.HasValue)
				{
					dictionary.Add("fail_reason", base.Context.FailReason.Value.GetEnumMemberValue());
				}
				if (base.Context.PlayersCurrent.HasValue)
				{
					dictionary.Add("players_current", base.Context.PlayersCurrent.Value);
				}
				if (base.Context.PlayersMin.HasValue)
				{
					dictionary.Add("players_min", base.Context.PlayersMin.Value);
				}
				if (base.Context.PlayersMax.HasValue)
				{
					dictionary.Add("players_max", base.Context.PlayersMax.Value);
				}
				if (base.Context.TimeLeft.HasValue)
				{
					dictionary.Add("time_left", base.Context.TimeLeft.Value);
				}
				if (base.Context.TimeLimit.HasValue)
				{
					dictionary.Add("time_limit", base.Context.TimeLimit.Value);
				}
				if (base.Context.RegionPing.HasValue)
				{
					dictionary.Add("region_ping", base.Context.RegionPing.Value);
				}
				return dictionary;
			}
		}

		private static void AddIfPresent(Dictionary<string, object> parameters, string key, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				parameters.Add(key, value);
			}
		}

		public MatchmakingLobbyEvent(CPMAnalytics analytics, MatchmakingLobbyContext context)
			: base(analytics, context)
		{
		}
	}
}
