using System;
using UnityEngine;

[Serializable]
public class PositionsAndRotation
{
	public Vector3 Position;

	public Quaternion Rotation;

	public static PositionsAndRotation Of(Transform point)
	{
		if (!point)
		{
			return null;
		}
		return new PositionsAndRotation
		{
			Position = point.position,
			Rotation = point.rotation
		};
	}
}
