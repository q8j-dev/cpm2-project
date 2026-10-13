using UnityEngine;

namespace Traffic
{
	public struct PointWithIndex
	{
		public Vector3 point;

		public int index;

		public PointWithIndex(Vector3 point, int index)
		{
			this.point = point;
			this.index = index;
		}
	}
}
