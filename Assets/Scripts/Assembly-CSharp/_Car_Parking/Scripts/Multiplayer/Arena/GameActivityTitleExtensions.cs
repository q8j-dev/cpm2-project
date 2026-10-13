using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class GameActivityTitleExtensions
	{
		public static bool TryGetActivityTitle(this ArenaModeRegistry registry, GameActivityEntry entry, out LocalizedString title)
		{
			title = null;
			if (entry.ActivityId is ArenaId arenaId)
			{
				return registry.TryGetActivityTitle(arenaId, out title);
			}
			return false;
		}

		public static bool TryGetActivityTitle(this ArenaModeRegistry registry, ArenaId arenaId, out LocalizedString title)
		{
			title = null;
			if (registry.TryGetConfig(arenaId, out var config))
			{
				title = new LocalizedString(config.Title);
				return true;
			}
			return false;
		}
	}
}
