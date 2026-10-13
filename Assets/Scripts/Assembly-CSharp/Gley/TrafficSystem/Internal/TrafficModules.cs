using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class TrafficModules : MonoBehaviour
	{
		[SerializeField]
		private bool _pathFinding;

		public bool PathFinding => _pathFinding;

		public void SetModules(bool enablePathFinding)
		{
			_pathFinding = enablePathFinding;
		}
	}
}
