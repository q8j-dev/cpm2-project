using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RallyRouteDraft", menuName = "Configs/Rally/Rally Route Draft")]
public class RallyRouteDraftSO : ScriptableObject
{
	[Serializable]
	public class DraftCurvePoint
	{
		public Vector3 Position;

		public Vector3 InHandle;

		public Vector3 OutHandle;
	}

	[Serializable]
	public class DraftMark
	{
		public int Mark;

		public float Confidence;

		public bool Accepted;
	}

	[Serializable]
	public class DraftEvent
	{
		public int PointIndex;

		public float DistanceToNext;

		public float SignedAngle;

		public int TurnType;

		public int Link;

		public float Confidence;

		public float ApexDistance;

		public Vector3 ApexPosition;

		public float TriggerDistance;

		public Vector3 TriggerPosition;

		public bool IgnoreRaycastOnStart;

		public bool ManualLocked;

		public int ManualSource;

		public bool TriggerSizeUseValue;

		public Vector3 TriggerSizeValue;

		public Vector3 TriggerCenter;

		public bool TriggerWidthManual;

		public List<DraftMark> Suggestions = new List<DraftMark>();
	}

	[Serializable]
	public class DraftAnalysisSettings
	{
		public bool InvertTurnDirection;

		public bool SpeedAwareDifficultyEnabled;

		public float SpeedAwareFlatTo6Approach;

		public float SpeedAware6To5Approach;

		public float SpeedAware5To4Approach;

		public float SpeedAware4To3Approach;

		public float SpeedAwareConfidencePenalty;

		public float KinkMax;

		public float FlatMax;

		public float Turn6Max;

		public float Turn5Max;

		public float Turn4Max;

		public float Turn3Max;

		public float Turn2Max;

		public float Turn1Max;

		public float SquareMin;

		public float SquareMax;

		public float HairpinTightMinDeg;

		public float ShortDistanceMax;

		public bool LocalEditSafetyGuardEnabled;

		public float LocalEditSafetyRadiusMeters;

		public float LocalEditSafetyEdgeBufferMeters;

		public bool LocalEditSafetyVerboseLog;

		public float LongDistanceMin;

		public float VeryLongDistanceMin;

		public float ChicaneDistanceMax;

		public float SampleStepMeters;

		public float HeadingWindowHalfMeters;

		public int AngleSmoothRadius;

		public int AngleSmoothPasses;

		public float KeyEventMinPeakDeg;

		public float MinTurnClusterMeters;

		public float MinTriggerSeparationMeters;

		public float MinEventGapBase;

		public float LeadFast;

		public float LeadMedium;

		public float LeadSlow;

		public float LeadHairpin;

		public float HighConfidenceThreshold;

		public float DefaultRoadWidth;

		public float MinRoadWidth;

		public float MaxRoadWidth;

		public bool UseAsymmetricWidth;

		public float WidthChangeDeltaForMarks;

		public float GeometrySampleStep;

		public float GeometryRayStartHeight;

		public float GeometryRayLength;

		public float CrestDipSlopeDeltaMin;

		public float RoughnessCautionThreshold;

		public float GeometryMarkWindowMeters;

		public float JumpSlopeDeltaMin;

		public float JumpHeightDropMin;

		public float CautionLookaheadMeters;

		public float CautionScoreThreshold;
	}

	[Serializable]
	public class DraftWidthKey
	{
		public float Distance;

		public bool UseAsymmetric;

		public float Width = 8f;

		public float LeftWidth = 4f;

		public float RightWidth = 4f;
	}

	[Serializable]
	public class DraftChicaneZone
	{
		public float StartDistance;

		public float EndDistance;

		public int Mode;

		public int EntryTurnType;

		public float MinAngleDeg;

		public float MaxDistance;
	}

	[Serializable]
	public class DraftSurfaceSample
	{
		public float Distance;

		public float Height;

		public float SlopeDeg;

		public float SlopeDeltaDeg;

		public float Roughness;

		public bool IsCrest;

		public bool IsDip;
	}

	[Serializable]
	public class DraftSuppressedEvent
	{
		public float ApexDistance;

		public float Radius;
	}

	[Serializable]
	public class DraftSuppressedDifficultySuggestion
	{
		public float TriggerDistance;

		public float Radius;

		public int CurrentTurnType;

		public int SuggestedTurnType;

		public int ReasonKind;
	}

	[Serializable]
	public class DraftTransformSlot
	{
		public bool Defined;

		public Vector3 Position;

		public Quaternion Rotation = Quaternion.identity;
	}

	[Serializable]
	public class DraftEntry
	{
		public int CurrentIndex;

		public ScriptableObject TrajectoryAsset;

		public GameObject TrajectoryPrefab;

		public int PlaceToSpawn;

		public string PlaceToSpawnName;

		public bool IsNight;

		public bool IsSpecial;

		public List<DraftCurvePoint> CurvePoints = new List<DraftCurvePoint>();

		public List<DraftEvent> Events = new List<DraftEvent>();

		public DraftAnalysisSettings AnalysisSettings = new DraftAnalysisSettings();

		public List<DraftWidthKey> WidthKeys = new List<DraftWidthKey>();

		public List<DraftChicaneZone> ChicaneZones = new List<DraftChicaneZone>();

		public List<DraftSurfaceSample> SurfaceSamples = new List<DraftSurfaceSample>();

		public List<DraftSuppressedEvent> SuppressedEvents = new List<DraftSuppressedEvent>();

		public List<DraftSuppressedDifficultySuggestion> SuppressedDifficultySuggestions = new List<DraftSuppressedDifficultySuggestion>();

		public List<Vector3> RoadCenterline = new List<Vector3>();

		public float RoadHalfWidth = 20f;

		public DraftTransformSlot GaragePoint = new DraftTransformSlot();

		public DraftTransformSlot CarStartPoint = new DraftTransformSlot();

		public DraftTransformSlot StagePoint = new DraftTransformSlot();

		public Vector3 DefaultTriggerColliderSize;

		public Vector3 DefaultTriggerColliderCenter;
	}

	[HideInInspector]
	public ScriptableObject TrajectoryAsset;

	[HideInInspector]
	public GameObject TrajectoryPrefab;

	[HideInInspector]
	public int CurrentIndex;

	[HideInInspector]
	public int PlaceToSpawn;

	[HideInInspector]
	public bool IsNight;

	[HideInInspector]
	public bool IsSpecial;

	[HideInInspector]
	public List<DraftCurvePoint> CurvePoints = new List<DraftCurvePoint>();

	[HideInInspector]
	public List<DraftEvent> Events = new List<DraftEvent>();

	[HideInInspector]
	public DraftAnalysisSettings AnalysisSettings = new DraftAnalysisSettings();

	[HideInInspector]
	public List<DraftWidthKey> WidthKeys = new List<DraftWidthKey>();

	[HideInInspector]
	public List<DraftChicaneZone> ChicaneZones = new List<DraftChicaneZone>();

	[HideInInspector]
	public List<DraftSurfaceSample> SurfaceSamples = new List<DraftSurfaceSample>();

	[HideInInspector]
	public List<DraftSuppressedEvent> SuppressedEvents = new List<DraftSuppressedEvent>();

	[HideInInspector]
	public List<DraftSuppressedDifficultySuggestion> SuppressedDifficultySuggestions = new List<DraftSuppressedDifficultySuggestion>();

	[HideInInspector]
	public List<Vector3> RoadCenterline = new List<Vector3>();

	[HideInInspector]
	public float RoadHalfWidth = 20f;

	[HideInInspector]
	public DraftTransformSlot GaragePoint = new DraftTransformSlot();

	[HideInInspector]
	public DraftTransformSlot CarStartPoint = new DraftTransformSlot();

	[HideInInspector]
	public DraftTransformSlot StagePoint = new DraftTransformSlot();

	public List<DraftEntry> Entries = new List<DraftEntry>();
}
