using System;
using UnityEngine;

[Serializable]
public struct CameraViewConfig
{
	public Vector3 Offset;

	public float FOV;

	public Vector3 ComposerOffset;

	public float SlopeUp;

	public float SlopeDown;
}
