using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameActivity
{
	public sealed class GameActivityCatalog : IGameActivityCatalog
	{
		private static readonly GameActivityEntry[] Empty = new GameActivityEntry[0];

		private readonly List<IGameActivityProvider> _providers;

		public GameActivityCatalog(IEnumerable<IGameActivityProvider> providers)
		{
			_providers = new List<IGameActivityProvider>(providers);
		}

		public int GetCount(GameActivityType type)
		{
			return GetActivities(type).Count;
		}

		public IReadOnlyList<GameActivityEntry> GetActivities(GameActivityType type)
		{
			for (int i = 0; i < _providers.Count; i++)
			{
				if (_providers[i].Handles(type))
				{
					return _providers[i].GetActivities(type);
				}
			}
			return Empty;
		}
	}
}
