using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaPointsContainer
	{
		private readonly Dictionary<ArenaLocation, ArenaPointsConfig> _configs = new Dictionary<ArenaLocation, ArenaPointsConfig>();

		public ArenaPointsContainer(IEnumerable<ArenaPointsConfig> configs)
		{
			foreach (ArenaPointsConfig config in configs)
			{
				if ((bool)config)
				{
					if (_configs.ContainsKey(config.Type))
					{
						Debug.LogError($"[Arena] duplicate ArenaPointsConfig for '{config.Type}' — config ignored");
					}
					else
					{
						_configs[config.Type] = config;
					}
				}
			}
		}

		public bool TryGet(ArenaLocation type, out ArenaPointsConfig config)
		{
			return _configs.TryGetValue(type, out config);
		}
	}
}
