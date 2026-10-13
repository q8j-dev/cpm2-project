using System;
using UnityEngine;

[Serializable]
public class SkidmarkSurfaceColor
{
	public Surface Surface;

	public Color Color = Color.black;

	[Range(0f, 1f)]
	public float DigOpacity;

	[Range(0f, 1f)]
	public float SlipFloor;

	[Range(0f, 1f)]
	public float SlipCeiling = 1f;
}
