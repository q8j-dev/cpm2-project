using UnityEngine;

public class AutoFencePreset
{
	public string name = "UnnamedFencepreset";

	public int postType;

	public int railType;

	public int subType;

	public float fenceHeight = 1f;

	public float postHeightOffset;

	public Vector3 postSize = Vector3.one;

	public Vector3 postRotation = Vector3.zero;

	public int numRails = 1;

	public float railVerticalOffset = 0.2f;

	public float railGaps = 0.6f;

	public Vector3 railPositionOffset = Vector3.zero;

	public Vector3 railSize = Vector3.one;

	public Vector3 railRotation = Vector3.zero;

	public bool showSubs;

	public int subsFixedOrProportionalSpacing;

	public float subSpacing;

	public Vector3 subPositionOffset = Vector3.zero;

	public Vector3 subSize = Vector3.one;

	public Vector3 subRotation = Vector3.zero;

	public bool useWave;

	public bool useJoiners;

	public float frequency;

	public float amplitude;

	public float wavePosition;

	public bool interpolate;

	public float interPostDistance;

	public bool smooth;

	public float tension;

	public int roundingDistance;

	public bool forceSubsToGroundContour;

	public float randomness;

	public float removeIfLessThanAngle;

	public float stripTooClose;

	public AutoFencePreset(string inName, int inPostType, int inRailType, int inSubType, float inFenceHeight, float inPostHeightOffset, Vector3 inPostSize, Vector3 inPostRotation, int inNumRails, float inRailGaps, Vector3 inRailPositionOffset, Vector3 inRailSize, Vector3 inRailRotation, bool inShowSubs, int inSubsFixedOrProportionalSpacing, float inSubSpacing, Vector3 inSubPositionOffset, Vector3 inSubSize, Vector3 inSubRotation, bool inUseWave, float inFrequency, float inAmplitude, float inWavePosition, bool inUseJoiners, bool inInterpolate, float inInterPostDistance, bool inSmooth, float inTension, int inRoundingDistance, bool inForceSubsToGroundContour, float inRandomness, float inRemoveIfLessThanAngle, float inStripTooClose)
	{
		name = inName;
		postType = inPostType;
		railType = inRailType;
		subType = inSubType;
		fenceHeight = inFenceHeight;
		postHeightOffset = inPostHeightOffset;
		postSize = inPostSize;
		postRotation = inPostRotation;
		numRails = inNumRails;
		railGaps = inRailGaps;
		railPositionOffset = inRailPositionOffset;
		railSize = inRailSize;
		railRotation = inRailRotation;
		showSubs = inShowSubs;
		subsFixedOrProportionalSpacing = inSubsFixedOrProportionalSpacing;
		subSpacing = inSubSpacing;
		subSize = inSubSize;
		subPositionOffset = inSubPositionOffset;
		subRotation = inSubRotation;
		useWave = inUseWave;
		frequency = inFrequency;
		amplitude = inAmplitude;
		wavePosition = inWavePosition;
		useJoiners = inUseJoiners;
		interpolate = inInterpolate;
		interPostDistance = inInterPostDistance;
		smooth = inSmooth;
		tension = inTension;
		roundingDistance = inRoundingDistance;
		forceSubsToGroundContour = inForceSubsToGroundContour;
		randomness = inRandomness;
		removeIfLessThanAngle = inRemoveIfLessThanAngle;
		stripTooClose = inStripTooClose;
	}
}
