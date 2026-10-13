using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameFacts.Base
{
	public class GameFactManager : IGameFactManager
	{
		private readonly Dictionary<GameFact, HashSet<object>> _sources = new Dictionary<GameFact, HashSet<object>>();

		public event Action<GameFact> OnChanged;

		public void Set(GameFact gameFact, object source, bool active)
		{
			if (!_sources.TryGetValue(gameFact, out var value))
			{
				value = (_sources[gameFact] = new HashSet<object>());
			}
			bool num = IsTrue(gameFact);
			if (active)
			{
				value.Add(source);
			}
			else
			{
				value.Remove(source);
			}
			if (num != IsTrue(gameFact))
			{
				OnChanged?.Invoke(gameFact);
			}
		}

		public bool IsTrue(GameFact gameFact)
		{
			if (_sources.TryGetValue(gameFact, out var value))
			{
				return value.Count > 0;
			}
			return false;
		}

		public void Release(object source)
		{
			List<GameFact> list = null;
			foreach (KeyValuePair<GameFact, HashSet<object>> source2 in _sources)
			{
				bool flag = source2.Value.Count > 0;
				if ((source2.Value.Remove(source) & flag) && source2.Value.Count == 0)
				{
					(list ?? (list = new List<GameFact>())).Add(source2.Key);
				}
			}
			if (list == null)
			{
				return;
			}
			foreach (GameFact item in list)
			{
				OnChanged?.Invoke(item);
			}
		}
	}
}
