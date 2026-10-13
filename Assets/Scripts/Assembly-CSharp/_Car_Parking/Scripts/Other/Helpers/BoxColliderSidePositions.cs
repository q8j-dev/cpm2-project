using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class BoxColliderSidePositions
	{
		public enum Side
		{
			Left = 0,
			Right = 1,
			Top = 2,
			Bottom = 3,
			Front = 4,
			Back = 5
		}

		public static Vector3 GetSide(this BoxCollider boxCollider, Side side)
		{
			if ((bool)boxCollider)
			{
				Vector3 center = boxCollider.center;
				switch (side)
				{
				case Side.Right:
					center.x += boxCollider.size.x * 0.5f;
					break;
				case Side.Left:
					center.x -= boxCollider.size.x * 0.5f;
					break;
				case Side.Top:
					center.y += boxCollider.size.y * 0.5f;
					break;
				case Side.Bottom:
					center.y -= boxCollider.size.y * 0.5f;
					break;
				case Side.Front:
					center.z += boxCollider.size.z * 0.5f;
					break;
				case Side.Back:
					center.z -= boxCollider.size.z * 0.5f;
					break;
				}
				return boxCollider.transform.TransformPoint(center);
			}
			return Vector3.zero;
		}
	}
}
