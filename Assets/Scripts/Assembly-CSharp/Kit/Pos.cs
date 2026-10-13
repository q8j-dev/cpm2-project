using UnityEngine;

namespace Kit
{
	public struct Pos
	{
		public int index;

		public Transform transform;

		public Pos(int index, Transform transform)
		{
			this.index = index;
			this.transform = transform;
		}
	}
}
