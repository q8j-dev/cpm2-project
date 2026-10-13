using UnityEngine;

namespace Arikan
{
	public static class Extensions
	{
		public static Vector3 GetPosition(this Matrix4x4 m)
		{
			return new Vector3
			{
				x = m.m03,
				y = m.m13,
				z = m.m23
			};
		}

		public static Quaternion GetRotation(this Matrix4x4 m)
		{
			return Quaternion.LookRotation(new Vector3
			{
				x = m.m02,
				y = m.m12,
				z = m.m22
			}, new Vector3
			{
				x = m.m01,
				y = m.m11,
				z = m.m21
			});
		}

		public static Vector2 XZ(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}
	}
}
