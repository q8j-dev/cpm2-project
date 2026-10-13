using System;
using UnityEngine;

[Serializable]
public class CustomVibrationData
{
	public double durationInSeconds;

	[Range(0f, 100f)]
	public float amplitude;

	[Range(0f, 100f)]
	[Header("Only for IOS")]
	public float sharpness;
}
