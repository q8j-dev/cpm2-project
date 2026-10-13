using System.Collections.Generic;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaForcedOutfitProvider : IForcedOutfitProvider
	{
		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly ArenaModeRegistry _registry;

		private readonly Dictionary<(int Gender, int TypeId), int> _rolled = new Dictionary<(int, int), int>();

		public ArenaForcedOutfitProvider(FreeDriveProvider freeDriveProvider, ArenaModeRegistry registry)
		{
			_freeDriveProvider = freeDriveProvider;
			_registry = registry;
		}

		public void Clear()
		{
			_rolled.Clear();
		}

		public bool TryGetForcedKit(int gender, int typeId, out int kitId)
		{
			kitId = -1;
			ArenaId roomArenaId = _freeDriveProvider.RoomArenaId;
			if (roomArenaId == ArenaId.Free)
			{
				return false;
			}
			(int, int) key = (gender, typeId);
			if (!_rolled.TryGetValue(key, out var value))
			{
				value = ((_registry.TryGetConfig(roomArenaId, out var config, onlyEnabled: false) && config.TryGetForcedKit((KitType)typeId, gender, out var kitId2)) ? kitId2 : (-1));
				_rolled[key] = value;
			}
			if (value < 0)
			{
				return false;
			}
			kitId = value;
			return true;
		}
	}
}
