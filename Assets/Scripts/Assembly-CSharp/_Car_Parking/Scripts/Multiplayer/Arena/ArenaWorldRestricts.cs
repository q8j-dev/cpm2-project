using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaWorldRestricts
	{
		private const string BoundariesName = "All ristricts - WorldBoundaries";

		private static GameObject _suppressed;

		public static void Suppress()
		{
			if (!_suppressed)
			{
				GameObject gameObject = GameObject.Find("All ristricts - WorldBoundaries");
				if ((bool)gameObject)
				{
					gameObject.SetActive(value: false);
					_suppressed = gameObject;
				}
			}
		}

		public static void Restore()
		{
			if ((bool)_suppressed)
			{
				_suppressed.SetActive(value: true);
			}
			_suppressed = null;
		}
	}
}
