using System.Collections.Generic;
using MVPFree.MapIcons.Enum;

namespace MVPFree.MapIcons
{
	public static class ArenaMapIcons
	{
		private static readonly HashSet<IconType> Types = new HashSet<IconType> { IconType.ArenaObstacleRace };

		public static bool IsArenaIcon(IconType type)
		{
			return Types.Contains(type);
		}
	}
}
