using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameActivity
{
	public interface IGameActivityCatalog
	{
		int GetCount(GameActivityType type);

		IReadOnlyList<GameActivityEntry> GetActivities(GameActivityType type);
	}
}
