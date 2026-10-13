using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl
{
	public class AdjustableDictionary
	{
		private readonly Dictionary<byte, TimerObject> _timers = new Dictionary<byte, TimerObject>();

		public void Clear()
		{
			foreach (TimerObject value in _timers.Values)
			{
				value.Dispose();
			}
			_timers.Clear();
		}

		public bool Contains(byte key)
		{
			return _timers.ContainsKey(key);
		}

		public void Add(byte key, float time)
		{
			if (_timers.ContainsKey(key))
			{
				Remove(key);
			}
			TimerObject value = new TimerObject(key, time, this);
			_timers.Add(key, value);
		}

		public void Remove(byte key)
		{
			if (_timers.TryGetValue(key, out var value))
			{
				value.Dispose();
				_timers.Remove(key);
			}
		}

		~AdjustableDictionary()
		{
			Clear();
		}
	}
}
