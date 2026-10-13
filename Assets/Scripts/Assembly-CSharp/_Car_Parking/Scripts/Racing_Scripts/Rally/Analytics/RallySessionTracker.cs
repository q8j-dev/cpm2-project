using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Analytics
{
	public class RallySessionTracker
	{
		private readonly Dictionary<(int loc, int traj), int> _attemptNumbers = new Dictionary<(int, int), int>();

		private readonly Dictionary<(int loc, int traj), RallyMedalType> _lastClaimedMedals = new Dictionary<(int, int), RallyMedalType>();

		private int _restartCount;

		public int NextAttempt(int locationIndex, int trajectoryIndex)
		{
			(int, int) key = (locationIndex, trajectoryIndex);
			_attemptNumbers.TryGetValue(key, out var value);
			int num = value + 1;
			_attemptNumbers[key] = num;
			_restartCount = 0;
			return num;
		}

		public int GetAttempt(int locationIndex, int trajectoryIndex)
		{
			_attemptNumbers.TryGetValue((locationIndex, trajectoryIndex), out var value);
			return value;
		}

		public int IncrementRestart()
		{
			return ++_restartCount;
		}

		public int GetRestartCount()
		{
			return _restartCount;
		}

		public RallyMedalType GetLastClaimedMedal(int locationIndex, int trajectoryIndex)
		{
			if (!_lastClaimedMedals.TryGetValue((locationIndex, trajectoryIndex), out var value))
			{
				return RallyMedalType.NONE;
			}
			return value;
		}

		public void MarkClaimed(int locationIndex, int trajectoryIndex, RallyMedalType medal)
		{
			_lastClaimedMedals[(locationIndex, trajectoryIndex)] = medal;
		}
	}
}
