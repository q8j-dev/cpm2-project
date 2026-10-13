using UnityEngine;

[CreateAssetMenu(fileName = "Wet Params Profile", menuName = "Rendering/Wet Params Profile")]
public sealed class WetParamsProfile : ScriptableObject
{
	[Range(0f, 1f)]
	[Header("Global Wet Response")]
	public float GeneralWetnessScale = 0.6f;

	[Range(0.5f, 4f)]
	[Header("Puddles (mask-driven)")]
	public float PuddleExp = 1.6f;

	[Range(0f, 1f)]
	public float PuddleFill = 0.06f;

	[Range(0.001f, 0.5f)]
	public float PuddleEdgeSoftness = 0.08f;

	[Range(0f, 1f)]
	public float PuddleDarkExtra = 0.15f;

	[Range(0f, 1f)]
	public float PuddleNormalFlatten = 0.9f;

	[Header("Puddle build-up curve")]
	[Range(0f, 1f)]
	public float PuddleWetStart = 0.25f;

	[Range(0.25f, 4f)]
	public float PuddleWetPower = 1.4f;

	[Header("Normals")]
	[Range(1f, 3f)]
	public float NormalBoostMax = 1.5f;

	[Range(0.5f, 2f)]
	public float NormalBoostEarlyK = 0.8f;

	[Range(0f, 2f)]
	[Header("Smoothness response")]
	public float SmoothFilmMul = 1f;

	[Range(0f, 2f)]
	public float SmoothPuddleMul = 1f;

	[Header("Clear Coat")]
	[Range(0f, 1f)]
	public float CoatMaskScale = 1f;

	[Range(0f, 1f)]
	public float CoatSmoothness = 0.9f;

	[Range(0f, 2f)]
	public float CoatFilmMul = 0.35f;

	[Range(0f, 2f)]
	public float CoatPuddleMul = 1f;

	[Header("Puddle Slope Trimming")]
	[Range(0f, 45f)]
	public float PuddleSlopeStartDeg = 8f;

	[Range(0f, 80f)]
	public float PuddleSlopeEndDeg = 22f;

	[Range(0.5f, 8f)]
	public float PuddleSlopePow = 2f;

	[Tooltip("Default width (m) puddles fade over at a dry-zone face, per box-local axis. The fade straddles the face. Keep Y small: the box floor often sits right under the road.")]
	[Header("Dry Zones (tunnels) — edge fade")]
	public Vector3 DryZoneFeather = new Vector3(8f, 2f, 8f);

	[Tooltip("Curve on the fade. 1 = raw ramp — puddles hold full strength until the mask drops under ~0.2, so the whole transition collapses into the last fifth of the feather and reads as a line. Higher spreads it over most of the feather.")]
	[Range(1f, 8f)]
	public float DryZoneEdgeGamma = 4f;

	[Range(0f, 5f)]
	[Tooltip("How far (m) the boundary wanders, so puddles stop along a wavy line, not a box edge.")]
	public float DryZoneEdgeWobble = 1.2f;

	[Range(0f, 2f)]
	[Tooltip("Wobble frequency, rad/m (wavelength = 2π/f). 0 = perfectly straight edges.")]
	public float DryZoneWobbleFreq = 0.5f;

	[HideInInspector]
	public int revision;
}
