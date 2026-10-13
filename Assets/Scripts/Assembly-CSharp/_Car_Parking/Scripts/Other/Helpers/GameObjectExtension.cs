using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class GameObjectExtension
	{
		public static bool TryGetComponentInChildren<T>(this MonoBehaviour obj, out T component, bool includeInactive = false)
		{
			component = obj.GetComponentInChildren<T>(includeInactive);
			return component != null;
		}

		public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component, bool includeInactive = false) where T : class
		{
			if (obj == null)
			{
				component = null;
				return false;
			}
			component = obj.GetComponentInChildren<T>(includeInactive);
			return component != null;
		}
	}
}
