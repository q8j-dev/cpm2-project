using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Atmosphere", menuName = "AtmosphereSettings")]
public class AtmosphereSettings : ScriptableObject
{
	[SerializeField]
	public float SeaLevel;

	[SerializeField]
	public float PlanetRadius = 6360000f;

	[SerializeField]
	public float AtmosphereHeight = 60000f;

	[SerializeField]
	public float SunDiskAngle = 9f;

	[SerializeField]
	public float RayleighScatteringScale = 1f;

	[SerializeField]
	public float RayleighScatteringScalarHeight = 8000f;

	[SerializeField]
	public float MieScatteringScale = 1f;

	[SerializeField]
	public float MieAnisotropy = 0.8f;

	[SerializeField]
	public float MieScatteringScalarHeight = 1200f;

	[SerializeField]
	public float OzoneAbsorptionScale = 1f;

	[SerializeField]
	public float OzoneLevelCenterHeight = 25000f;

	[SerializeField]
	public float OzoneLevelWidth = 15000f;

	[SerializeField]
	public float AerialPerspectiveDistance = 32000f;
}
