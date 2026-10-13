using System.Collections.Generic;
using System.Reflection;

namespace _Car_Parking.Scripts.GooglePlayServices
{
	public static class GPGSIdNames
	{
		private const string AchievementPrefix = "achievement_";

		private static readonly Dictionary<string, string> CanonicalKeysByPlatformId = new Dictionary<string, string>
		{
			{ "CgkIuKqW0PcZEAIQCw", "ACH_IGNITION_ON" },
			{ "CgkIuKqW0PcZEAIQDA", "ACH_FIRST_WRENCH_TURN" },
			{ "CgkIuKqW0PcZEAIQDQ", "ACH_FIRST_CUSTOM_LOOK" },
			{ "CgkIuKqW0PcZEAIQDg", "ACH_DYNO_DAY" },
			{ "CgkIuKqW0PcZEAIQDw", "ACH_VINYL_SIGNATURE" },
			{ "CgkIuKqW0PcZEAIQEA", "ACH_MARKET_SCOUT" },
			{ "CgkIuKqW0PcZEAIQEQ", "ACH_PROFILE_PEEK" },
			{ "CgkIuKqW0PcZEAIQEg", "ACH_PERFECTLY_PARKED" },
			{ "CgkIuKqW0PcZEAIQEw", "ACH_CHECKERED_HELLO" },
			{ "CgkIuKqW0PcZEAIQFA", "ACH_QUATER_MILE_HERO" },
			{ "CgkIuKqW0PcZEAIQFQ", "ACH_OPEN_ROADS" },
			{ "CgkIuKqW0PcZEAIQFg", "ACH_SMILE_FOR_THE_RADAR" },
			{ "CgkIuKqW0PcZEAIQFw", "ACH_NO_PAVEMENT_NO_PROBLEM" },
			{ "CgkIuKqW0PcZEAIQGA", "ACH_FIVE_STAR_FARE" },
			{ "CgkIuKqW0PcZEAIQGQ", "ACH_BOOSTED_CAREFULLY" },
			{ "CgkIuKqW0PcZEAIQGg", "ACH_REDLINE_SCHOLAR" },
			{ "CgkIuKqW0PcZEAIQGw", "ACH_PERFECT_FITMENT" },
			{ "CgkIuKqW0PcZEAIQHA", "ACH_WIDE_STANCE" },
			{ "CgkIuKqW0PcZEAIQHQ", "ACH_PAINT_CHEMISTRY" },
			{ "CgkIuKqW0PcZEAIQHg", "ACH_TIMEATTACK_RACE" },
			{ "CgkIuKqW0PcZEAIQHw", "ACH_SECOND_SET_OF_KEYS" },
			{ "CgkIuKqW0PcZEAIQIA", "ACH_STREET_DEAL" },
			{ "CgkIuKqW0PcZEAIQIQ", "ACH_EVEN_TRADE" },
			{ "CgkIuKqW0PcZEAIQIg", "ACH_FIRST_CHALLENGE" },
			{ "CgkIuKqW0PcZEAIQIw", "ACH_RIDE_ALONG" },
			{ "CgkIuKqW0PcZEAIQJA", "ACH_TREE_WHISPERER" },
			{ "CgkIuKqW0PcZEAIQJQ", "ACH_CLEAN_LINE" },
			{ "CgkIuKqW0PcZEAIQJg", "ACH_TRUST_THE_CODRIVER" },
			{ "CgkIuKqW0PcZEAIQJw", "ACH_NO_NOTES_MISSED" },
			{ "CgkIuKqW0PcZEAIQKA", "ACH_TWENTY_FOUR_MINUTES" },
			{ "CgkIuKqW0PcZEAIQKQ", "ACH_POSTCARD_HUNTER" },
			{ "CgkIuKqW0PcZEAIQKg", "ACH_NIGHT_SHIFT" },
			{ "CgkIuKqW0PcZEAIQKw", "ACH_LIGHTS_AND_PAPERS" },
			{ "CgkIuKqW0PcZEAIQLA", "ACH_GRAPH_PAPER" },
			{ "CgkIuKqW0PcZEAIQLQ", "ACH_TUNED_RESALE" },
			{ "CgkIuKqW0PcZEAIQLg", "ACH_RUN_IT_BACK" },
			{ "CgkIuKqW0PcZEAIQLw", "ACH_BREAKAWAY" },
			{ "CgkIuKqW0PcZEAIQMA", "ACH_CAUGHT_IN_SIXTY" },
			{ "CgkIuKqW0PcZEAIQMQ", "ACH_BRACKET_BOSS" },
			{ "CgkIuKqW0PcZEAIQMg", "ACH_POLE_TO_PODIUM" },
			{ "CgkIuKqW0PcZEAIQMw", "ACH_SPLASH_AND_DASH" },
			{ "CgkIuKqW0PcZEAIQNA", "ACH_SIDEWAYS_SCIENCE" },
			{ "CgkIuKqW0PcZEAIQNQ", "ACH_GARAGE_VARIETY" },
			{ "CgkIuKqW0PcZEAIQNg", "ACH_CLEAN_RIVALRY" },
			{ "CgkIuKqW0PcZEAIQNw", "ACH_PHOTO_FINISH" },
			{ "CgkIuKqW0PcZEAIQOA", "ACH_BLINK_AND_YOU_MISSED_IT" },
			{ "CgkIuKqW0PcZEAIQOQ", "ACH_RUNNING_ON_FUMES" },
			{ "CgkIuKqW0PcZEAIQOg", "ACH_DEALER_LUCK" },
			{ "CgkIuKqW0PcZEAIQOw", "ACH_CLEAN_SLATE" }
		};

		private static Dictionary<string, string> _names;

		public static string Of(string achievementId)
		{
			if (_names == null)
			{
				_names = Build();
			}
			if (!_names.TryGetValue(achievementId, out var value))
			{
				return achievementId;
			}
			return value;
		}

		public static string CanonicalKeyOf(string achievementId)
		{
			if (string.IsNullOrEmpty(achievementId))
			{
				return achievementId;
			}
			if (!CanonicalKeysByPlatformId.TryGetValue(achievementId, out var value))
			{
				return achievementId;
			}
			return value;
		}

		public static IEnumerable<string> AllIds()
		{
			if (_names == null)
			{
				_names = Build();
			}
			return _names.Keys;
		}

		private static Dictionary<string, string> Build()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			FieldInfo[] fields = typeof(GPGSIds).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsLiteral && !(fieldInfo.FieldType != typeof(string)) && fieldInfo.GetRawConstantValue() is string text && !string.IsNullOrEmpty(text))
				{
					dictionary[text] = (fieldInfo.Name.StartsWith("achievement_") ? fieldInfo.Name.Substring("achievement_".Length) : fieldInfo.Name);
				}
			}
			return dictionary;
		}
	}
}
