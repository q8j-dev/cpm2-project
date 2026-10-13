using System.Collections.Generic;
using MVPFree.MapIcons;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.ScrObjs.Scripts;
using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.GameActivity.Providers
{
	public sealed class ArenaActivityProvider : IGameActivityProvider
	{
		private readonly ArenaModeRegistry _registry;

		private readonly OptionalDependency<MapIconPositionsDataSo> _positions;

		private static readonly Dictionary<GameActivityType, IconType> ArenaIcons = new Dictionary<GameActivityType, IconType> { [GameActivityType.ObstacleRace] = IconType.ArenaObstacleRace };

		public ArenaActivityProvider(ArenaModeRegistry registry, OptionalDependency<MapIconPositionsDataSo> positions)
		{
			_registry = registry;
			_positions = positions;
		}

		public bool Handles(GameActivityType type)
		{
			return ArenaIcons.ContainsKey(type);
		}

		public IReadOnlyList<GameActivityEntry> GetActivities(GameActivityType type)
		{
			List<GameActivityEntry> list = new List<GameActivityEntry>();
			if (!ArenaIcons.TryGetValue(type, out var value))
			{
				return list;
			}
			if (!ArenaActivityMap.TryGetMode(type, out var modeType))
			{
				return list;
			}
			foreach (ArenaConfig allArenaConfig in _registry.GetAllArenaConfigs(modeType))
			{
				list.Add(new GameActivityEntry(type, allArenaConfig.ArenaId, allArenaConfig.ArenaId.ToString(), string.Empty, value, FindPosition((int)allArenaConfig.ArenaId)));
			}
			return list;
		}

		private Vector3? FindPosition(int arenaId)
		{
			if (_positions == null || !_positions.Value || _positions.Value.IconPositions == null)
			{
				return null;
			}
			MapIconPositionsDataSo.IconPositionData[] iconPositions = _positions.Value.IconPositions;
			foreach (MapIconPositionsDataSo.IconPositionData iconPositionData in iconPositions)
			{
				if (iconPositionData.Id == arenaId && ArenaMapIcons.IsArenaIcon(iconPositionData.IconType))
				{
					return iconPositionData.WorldPosition;
				}
			}
			return null;
		}
	}
}
