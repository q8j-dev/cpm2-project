using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Vehicle.Damage;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Physics;

public class VehicleDamage : MonoBehaviour
{
	public bool isON;

	public MeshCollider meshCollider;

	public MeshFilter[] meshFilters;

	public MeshFilter[] m_meshFilters;

	public float deformNoise = 0.005f;

	public float deformRadius = 0.5f;

	public bool DoNotDamage;

	public float bounceBackSpeed = 2f;

	public float maxDeform = 0.5f;

	public float multiplier = 0.1f;

	public float YforceDamp = 1f;

	[HideInInspector]
	public bool repair;

	public GameObject[] HideWhenDamage;

	private const float BounceBackSleepCap = 0.002f;

	private const float MinImpulse = 800f;

	private const float ReferenceImpulseScale = 6000f;

	private const float ReferenceMultiplier = 0.01f;

	private const int MaxContacts = 16;

	private const int MaxImpactsPerFrame = 2;

	private const int QueueCapacity = 16;

	private const int DefaultSolverIterations = 8;

	private const int KitsQuietFrames = 2;

	private readonly List<DeformableMesh> _panels = new List<DeformableMesh>();

	private readonly List<MeshFilter> _scanBuffer = new List<MeshFilter>();

	private readonly List<MeshFilter> _scanScratch = new List<MeshFilter>();

	private readonly ImpactQueue _queue = new ImpactQueue(16);

	private readonly ContactPoint[] _contacts = new ContactPoint[16];

	private ContactAccumulator _contactAccumulator;

	private NativeArray<float3> _contactPoints;

	private NativeArray<float3> _contactNormals;

	private VehicleData _vehicleData;

	private BodyKits _bodyKits;

	private Transform _myTransform;

	private int _wheelLayer;

	private int _carLayer;

	private int _solverIterations = 8;

	private const int LowQualitySolverIterations = 4;

	private bool _scanned;

	private bool _ready;

	private bool _building;

	private bool _buildAttempted;

	private CancellationTokenSource _buildCts;

	private IDisposable _bodyKitsSubscription;

	private int _scannedSignature;

	private bool _scanRecorded;

	private int _seenSignature;

	private bool _seenRecorded;

	private int _seenChangedFrame;

	public void Awake()
	{
		if (HideWhenDamage.Length == 0)
		{
			HideWhenDamage = new GameObject[2];
		}
		_contactPoints = new NativeArray<float3>(16, Allocator.Persistent);
		_contactNormals = new NativeArray<float3>(16, Allocator.Persistent);
	}

	public void Init()
	{
		base.enabled = true;
		_myTransform = base.transform;
		_vehicleData = GetComponent<VehicleData>();
		_bodyKits = GetComponent<BodyKits>();
		ReleasePanels();
		_bodyKitsSubscription?.Dispose();
		_bodyKitsSubscription = null;
		if (!(_bodyKits == null))
		{
			_bodyKitsSubscription = _bodyKits.AllBodyKitsWasLoaded.Subscribe((bool _) =>
			{
				CheckAllBodyAndBodyKitsLoaded();
			});
			CheckAllBodyAndBodyKitsLoaded();
		}
	}

	private void ReleasePanels()
	{
		_buildCts?.Cancel();
		_buildCts?.Dispose();
		_buildCts = null;
		for (int i = 0; i < _panels.Count; i++)
		{
			_panels[i].Dispose();
		}
		_panels.Clear();
		_queue.Clear();
		_contactAccumulator.Reset();
		_scanned = false;
		_ready = false;
		_building = false;
		_buildAttempted = false;
		_scanRecorded = false;
		meshFilters = Array.Empty<MeshFilter>();
		m_meshFilters = Array.Empty<MeshFilter>();
	}

	private void CheckAllBodyAndBodyKitsLoaded()
	{
		if (_bodyKits != null && _bodyKits.AllBodyKitsWasLoaded.Value)
		{
			InitStart();
		}
	}

	private void InitStart()
	{
		if (!_scanned && !DoNotDamage)
		{
			CreateMeshFilters();
			if (meshFilters.Length != 0)
			{
				_wheelLayer = LayerViaName.WheelLayer;
				_carLayer = base.gameObject.layer;
				_solverIterations = ((CPMGraphicsSettingsManager.Instance != null && CPMGraphicsSettingsManager.Instance.Quality >= 3) ? 8 : 4);
				_scanned = true;
			}
		}
	}

	private void EnsurePanelsBuilt()
	{
		if (_building || _bodyKits == null)
		{
			return;
		}
		bool flag = _scanned && meshFilters.Length == 0;
		if (_buildAttempted || !_scanned || meshFilters.Length == 0)
		{
			if ((!flag && !CompositionChangedAndQuiet()) || AnyPanelDamaged())
			{
				return;
			}
			if (_buildAttempted)
			{
				ReleasePanels();
			}
			_scanned = false;
			CheckAllBodyAndBodyKitsLoaded();
			if (!_scanned)
			{
				return;
			}
		}
		_building = true;
		_buildCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
		BuildPanelsAsync(_buildCts.Token).Forget();
	}

	private static int MixKit(int hash, GameObject kit)
	{
		return (hash * 397) ^ (kit ? kit.GetInstanceID() : 0);
	}

	private int CompositionSignature()
	{
		GameObject gameObject = (_vehicleData ? _vehicleData.MainModel : null);
		Transform transform = (_bodyKits ? _bodyKits.parent : null);
		int num = (gameObject ? gameObject.GetInstanceID() : 0);
		num = (num * 397) ^ (transform ? transform.childCount : 0);
		if (_bodyKits == null)
		{
			return num;
		}
		num = MixKit(num, _bodyKits.InstalledSpoiler);
		num = MixKit(num, _bodyKits.InstalledFrontBumper);
		num = MixKit(num, _bodyKits.InstalledRearBumper);
		num = MixKit(num, _bodyKits.installedSkirts);
		num = MixKit(num, _bodyKits.installedHood);
		num = MixKit(num, _bodyKits.installedAirIntakeHood);
		num = MixKit(num, _bodyKits.installedAirIntakeRoof);
		return MixKit(num, _bodyKits.installedFender);
	}

	private bool CompositionChangedAndQuiet()
	{
		if (_bodyKits == null || !_bodyKits.AllBodyKitsWasLoaded.Value)
		{
			return false;
		}
		int num = CompositionSignature();
		if (!_seenRecorded || num != _seenSignature)
		{
			_seenSignature = num;
			_seenRecorded = true;
			_seenChangedFrame = Time.frameCount;
			return false;
		}
		if (_scanRecorded && num == _scannedSignature)
		{
			return false;
		}
		return Time.frameCount - _seenChangedFrame >= 2;
	}

	private bool AnyPanelDamaged()
	{
		for (int i = 0; i < _panels.Count; i++)
		{
			if (_panels[i].Damaged)
			{
				return true;
			}
		}
		return false;
	}

	public void CreateMeshFilters()
	{
		if (meshFilters.Length != 0 || DoNotDamage)
		{
			return;
		}
		_scannedSignature = CompositionSignature();
		_scanRecorded = true;
		if (!_vehicleData || !_vehicleData.MainModel)
		{
			return;
		}
		BodyKitsRootPolicy.Collect(_vehicleData.MainModel.transform, _bodyKits ? _bodyKits.parent : null, _scanBuffer, _scanScratch);
		int num = 0;
		for (int i = 0; i < _scanBuffer.Count; i++)
		{
			if (IsDeformable(_scanBuffer[i]))
			{
				num++;
			}
		}
		m_meshFilters = _scanBuffer.ToArray();
		meshFilters = new MeshFilter[num];
		num = 0;
		for (int j = 0; j < _scanBuffer.Count; j++)
		{
			if (IsDeformable(_scanBuffer[j]))
			{
				meshFilters[num++] = _scanBuffer[j];
			}
		}
		_scanBuffer.Clear();
	}

	private static bool IsDeformable(MeshFilter filter)
	{
		if (filter.GetComponent<Collider>() == null && filter.GetComponentInParent<IgnoreCarDamageTag>(includeInactive: true) == null)
		{
			return filter.GetComponentInParent<CarInteriorReferences>(includeInactive: true) == null;
		}
		return false;
	}

	private async UniTaskVoid BuildPanelsAsync(CancellationToken token)
	{
		DamageSettings settings = BuildSettings();
		MeshFilter[] filters = meshFilters;
		for (int m = 0; m < filters.Length; m++)
		{
			if (token.IsCancellationRequested)
			{
				return;
			}
			MeshFilter meshFilter = filters[m];
			if ((bool)meshFilter && (bool)meshFilter.sharedMesh && (meshFilter.gameObject.layer != _wheelLayer || _carLayer == _wheelLayer))
			{
				try
				{
					_panels.Add(new DeformableMesh(meshFilter.mesh, meshFilter.transform, in settings));
				}
				catch (Exception exception)
				{
					Debug.LogException(exception, meshFilter);
				}
				await UniTask.Yield(PlayerLoopTiming.Update, token);
			}
		}
		if (!token.IsCancellationRequested)
		{
			_building = false;
			_buildAttempted = true;
			_ready = _panels.Count > 0;
		}
	}

	private DamageSettings BuildSettings()
	{
		DamageSettings result = DamageSettings.Default;
		result.MinImpulse = 800f;
		result.ImpulseScale = 60f / Mathf.Max(0.0001f, multiplier);
		result.MaxDepth = Mathf.Max(0.001f, maxDeform);
		result.RadiusBase = Mathf.Max(0.001f, deformRadius);
		result.YForceDamp = Mathf.Clamp01(YforceDamp);
		result.NoiseAmplitude = Mathf.Max(0f, deformNoise);
		result.SolverIterations = _solverIterations;
		return result;
	}

	private void OnCollisionEnter(Collision collision)
	{
		FeedContact(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		FeedContact(collision);
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision != null && (bool)collision.collider && collision.collider.GetInstanceID() == _contactAccumulator.ColliderId)
		{
			_contactAccumulator.Reset();
		}
	}

	private void FeedContact(Collision collision)
	{
		if (!isON || DoNotDamage || !_ready || collision == null || collision.gameObject.layer == 17)
		{
			return;
		}
		int num = Mathf.Min(collision.GetContacts(_contacts), 16);
		if (num <= 0)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			Vector3 point = _contacts[i].point;
			Vector3 normal = _contacts[i].normal;
			zero += normal;
			_contactPoints[i] = new float3(point.x, point.y, point.z);
			_contactNormals[i] = new float3(normal.x, normal.y, normal.z);
		}
		if (zero.sqrMagnitude < 1E-08f)
		{
			return;
		}
		zero.Normalize();
		Vector3 impulse = collision.impulse;
		int colliderId = (collision.collider ? collision.collider.GetInstanceID() : 0);
		if (_contactAccumulator.Add(colliderId, new float3(impulse.x, impulse.y, impulse.z), new float3(zero.x, zero.y, zero.z), Time.fixedDeltaTime, 800f, out var impulse2))
		{
			Vector3 relativeVelocity = collision.relativeVelocity;
			Vector3 up = _myTransform.up;
			if (ImpactMath.TryBuild(_contactPoints, _contactNormals, num, new float3(zero.x, zero.y, zero.z) * (0f - impulse2), new float3(relativeVelocity.x, relativeVelocity.y, relativeVelocity.z), new float3(up.x, up.y, up.z), BuildSettings(), out var e))
			{
				_queue.Enqueue(in e);
			}
		}
	}

	public void ApplyExternalImpact(Vector3 worldPoint, Vector3 inward, float impulse, Vector3 relativeVelocity)
	{
		if (isON && !DoNotDamage && _ready && !(impulse <= 0f) && !(inward.sqrMagnitude < 1E-08f))
		{
			inward.Normalize();
			_contactPoints[0] = new float3(worldPoint.x, worldPoint.y, worldPoint.z);
			_contactNormals[0] = new float3(inward.x, inward.y, inward.z);
			Vector3 vector = -inward * impulse;
			Vector3 up = _myTransform.up;
			if (ImpactMath.TryBuild(_contactPoints, _contactNormals, 1, new float3(vector.x, vector.y, vector.z), new float3(relativeVelocity.x, relativeVelocity.y, relativeVelocity.z), new float3(up.x, up.y, up.z), BuildSettings(), out var e))
			{
				_queue.Enqueue(in e);
			}
		}
	}

	private void Update()
	{
		if (repair)
		{
			StepRepair();
		}
		else if (isON && !DoNotDamage)
		{
			EnsurePanelsBuilt();
			if (_ready)
			{
				DispatchImpacts();
			}
		}
	}

	private void DispatchImpacts()
	{
		if (_queue.Count == 0)
		{
			return;
		}
		DamageSettings settings = BuildSettings();
		for (int i = 0; i < 2; i++)
		{
			if (!_queue.TryDequeue(out var impact))
			{
				break;
			}
			bool flag = false;
			for (int j = 0; j < _panels.Count; j++)
			{
				flag |= _panels[j].ScheduleImpact(in impact, in settings);
			}
			if (flag)
			{
				PlayerDatas.DisableEnableAll(isEnable: false, HideWhenDamage);
			}
		}
	}

	private void StepRepair()
	{
		if (bounceBackSpeed <= 0f)
		{
			FinishRepair();
			return;
		}
		_queue.Clear();
		float step = Time.deltaTime * bounceBackSpeed;
		bool flag = false;
		for (int i = 0; i < _panels.Count; i++)
		{
			flag |= _panels[i].ScheduleRepair(step, 0.002f);
		}
		if (!flag)
		{
			FinishRepair();
		}
	}

	private void FinishRepair()
	{
		repair = false;
		if ((bool)_vehicleData && _vehicleData.WindowComposition != null)
		{
			_vehicleData.WindowComposition.SetWindowShatterHoleActive(isBreak: false);
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < _panels.Count; i++)
		{
			_panels[i].CompleteAndApply();
		}
	}

	private void OnDestroy()
	{
		_buildCts?.Cancel();
		_buildCts?.Dispose();
		_buildCts = null;
		_bodyKitsSubscription?.Dispose();
		_bodyKitsSubscription = null;
		for (int i = 0; i < _panels.Count; i++)
		{
			_panels[i].Dispose();
		}
		_panels.Clear();
		if (_contactPoints.IsCreated)
		{
			_contactPoints.Dispose();
		}
		if (_contactNormals.IsCreated)
		{
			_contactNormals.Dispose();
		}
	}
}
