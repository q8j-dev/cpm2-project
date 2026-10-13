using UnityEngine;

namespace CPM.RouteStandings
{
	public sealed class RoutePath : MonoBehaviour
	{
		public Transform[] Waypoints;

		private void OnValidate()
		{
			_ = (bool)base.transform.parent;
			if (Waypoints == null)
			{
				return;
			}
			_ = Waypoints.Length;
			_ = 2;
			for (int i = 0; i < Waypoints.Length; i++)
			{
				Transform transform = Waypoints[i];
				if ((bool)transform)
				{
					for (int j = 0; j < i; j++)
					{
						_ = Waypoints[j] == transform;
					}
				}
			}
		}
	}
}
