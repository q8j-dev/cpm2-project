using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameActivity
{
	public interface IGameActivityProvider
	{
		bool Handles(GameActivityType type);

		IReadOnlyList<GameActivityEntry> GetActivities(GameActivityType type);
	}
}
