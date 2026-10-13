using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaModeRegistry
	{
		private readonly Dictionary<ArenaModeType, IArenaModeController> _controllers = new Dictionary<ArenaModeType, IArenaModeController>();

		private readonly Dictionary<ArenaId, ArenaConfig> _configs = new Dictionary<ArenaId, ArenaConfig>();

		public ArenaModeRegistry(IEnumerable<IArenaModeController> controllers, IEnumerable<ArenaConfig> configs)
		{
			foreach (IArenaModeController controller in controllers)
			{
				if (controller != null)
				{
					_controllers[controller.ModeType] = controller;
				}
			}
			foreach (ArenaConfig config in configs)
			{
				if ((bool)config && config.ArenaId != ArenaId.Free)
				{
					if (_configs.ContainsKey(config.ArenaId))
					{
						Debug.LogError($"[Arena] duplicate ArenaId '{config.ArenaId}' — config ignored");
					}
					else
					{
						_configs[config.ArenaId] = config;
					}
				}
			}
		}

		public bool TryGet(ArenaId arenaId, out IArenaModeController controller, out ArenaConfig config)
		{
			controller = null;
			if (_configs.TryGetValue(arenaId, out config))
			{
				return _controllers.TryGetValue(config.ModeType, out controller);
			}
			return false;
		}

		public bool TryGetConfig(ArenaId arenaId, out ArenaConfig config, bool onlyEnabled = true)
		{
			if (_configs.TryGetValue(arenaId, out config) && (bool)config && (!onlyEnabled || config.Enabled))
			{
				return true;
			}
			config = null;
			return false;
		}

		public IReadOnlyCollection<ArenaConfig> GetAllArenaConfigs(bool onlyEnabled = true)
		{
			if (!onlyEnabled)
			{
				return _configs.Values;
			}
			List<ArenaConfig> list = new List<ArenaConfig>();
			foreach (ArenaConfig value in _configs.Values)
			{
				if ((bool)value && value.Enabled)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public int GetAllArenaConfigsCount(bool onlyEnabled = true)
		{
			return GetAllArenaConfigs(onlyEnabled).Count;
		}

		public IReadOnlyList<ArenaConfig> GetAllArenaConfigs(ArenaModeType type, bool onlyEnabled = true)
		{
			return _configs.Values.Where((ArenaConfig c) => c.ModeType == type && (!onlyEnabled || c.Enabled)).ToList();
		}

		public int GetAllArenaConfigsCount(ArenaModeType type, bool onlyEnabled = true)
		{
			return GetAllArenaConfigs(type, onlyEnabled).Count;
		}

		public bool TryGet(ArenaModeType arenaModeType, out IArenaModeController controller)
		{
			return _controllers.TryGetValue(arenaModeType, out controller);
		}

		public void ClearAllModes()
		{
			foreach (IArenaModeController value in _controllers.Values)
			{
				value.Clear();
			}
		}
	}
}
