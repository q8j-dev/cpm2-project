using System;

namespace CPM.Ghost.Core
{
	public static class GhostIds
	{
		public static string New(string raceKey, int carId, long timeMs)
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
