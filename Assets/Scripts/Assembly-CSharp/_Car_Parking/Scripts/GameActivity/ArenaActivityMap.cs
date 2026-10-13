using System.Collections.Generic;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.GameActivity
{
	public static class ArenaActivityMap
	{
		private static readonly Dictionary<ArenaModeType, GameActivityType> ActivityByMode = new Dictionary<ArenaModeType, GameActivityType> { 
		{
			ArenaModeType.ObstacleRace,
			GameActivityType.ObstacleRace
		} };

		private static readonly Dictionary<GameActivityType, ArenaModeType> ModeByActivity = BuildReverse();

		public static bool TryGetActivity(ArenaModeType modeType, out GameActivityType activityType)
		{
			return ActivityByMode.TryGetValue(modeType, out activityType);
		}

		public static bool TryGetMode(GameActivityType activityType, out ArenaModeType modeType)
		{
			return ModeByActivity.TryGetValue(activityType, out modeType);
		}

		private static Dictionary<GameActivityType, ArenaModeType> BuildReverse()
		{
			Dictionary<GameActivityType, ArenaModeType> dictionary = new Dictionary<GameActivityType, ArenaModeType>(ActivityByMode.Count);
			foreach (KeyValuePair<ArenaModeType, GameActivityType> item in ActivityByMode)
			{
				dictionary[item.Value] = item.Key;
			}
			return dictionary;
		}
	}
}
