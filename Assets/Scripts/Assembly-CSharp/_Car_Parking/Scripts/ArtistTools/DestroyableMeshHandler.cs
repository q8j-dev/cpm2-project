using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FunctionalExtensions;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.ArtistTools.Aspects;
using _Car_Parking.Scripts.ArtistTools.BakedData;
using _Car_Parking.Scripts.ArtistTools.Enums;
using _Car_Parking.Scripts.ArtistTools.Trackables;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class DestroyableMeshHandler : DestroyableMeshBase, IStartAspect, IAspect, IRenameAspect
	{
		private const float LerpStep = 0.25f;

		public const string DestroyableName = "DestroyableMeshHandler";

		private const float PhysicAnimDuration = 3f;

		[Header("Car activation settings")]
		[SerializeField]
		[Space(10f)]
		private ImpulseBySpeedTypeModel[] ActivationCarSpeedData = new ImpulseBySpeedTypeModel[3]
		{
			new ImpulseBySpeedTypeModel(BakedAnimSpeedType.Slow, 60f),
			new ImpulseBySpeedTypeModel(BakedAnimSpeedType.Medium, 100f),
			new ImpulseBySpeedTypeModel(BakedAnimSpeedType.Fast, 140f)
		};

		[SerializeField]
		private bool AutoDestroyAfterPlayingAnim = default;

		[SerializeField]
		private float AutoDestroyFramesCoefficient = 0.3f;

		[Header("Childs data")]
		[SerializeField]
		public float DestroyableObjectsMass = 7f;

		[SerializeField]
		[Header("Baking data")]
		public TextAsset PackedBakedAnim;

		[SerializeField]
		public float BakingTimeStep = 0.034f;

		[SerializeField]
		[Header("Random offsets for baking anim playing")]
		private bool UseRandomOffset = true;

		[SerializeField]
		private Vector3 OffsetMin = new Vector3(-0.5f, -0.5f, -0.5f);

		[SerializeField]
		private Vector3 OffsetMax = new Vector3(0.5f, 0.5f, 0.5f);

		private List<BakedDirectionPoint> _bakedAnimPoints = new List<BakedDirectionPoint>();

		private Vector3 _randomOffset;

		private List<BakedPointData> _findedBakedPoints;

		private IDisposable _physicAnimDisposable;

		private readonly (HitDirection, Vector3)[] DirectionData = new (HitDirection, Vector3)[8]
		{
			(HitDirection.Forward, Vector3.forward),
			(HitDirection.ForwardRight, (Vector3.forward + Vector3.right).normalized),
			(HitDirection.Right, Vector3.right),
			(HitDirection.BackRight, (Vector3.back + Vector3.right).normalized),
			(HitDirection.Back, Vector3.back),
			(HitDirection.BackLeft, (Vector3.back + Vector3.left).normalized),
			(HitDirection.Left, Vector3.left),
			(HitDirection.ForwardLeft, (Vector3.forward + Vector3.left).normalized)
		};

		[field: SerializeField]
		public bool CanDamageCar { get; private set; }

		public async UniTask StartImpl(TrackablesRegisterTool tool)
		{
			if ((bool)PackedBakedAnim)
			{
				if (!tool.CachedBakedPoints.TryGetValue(PackedBakedAnim, out var value))
				{
					value = await BakedAnimCompressor.UnpackAsync(PackedBakedAnim.bytes);
					tool.CachedBakedPoints[PackedBakedAnim] = value;
				}
				_bakedAnimPoints = value;
				PackedBakedAnim = null;
			}
		}

		public override void OnDestroyImpl(TrackableWithAspects _)
		{
			base.OnDestroyImpl(_);
			_physicAnimDisposable?.Dispose();
		}

		public void RenameImpl()
		{
			base.gameObject.name = "DestroyableMeshHandler";
		}

		private float GetActivationImpulseByCarSpeed(BakedAnimSpeedType speedType)
		{
			return ActivationCarSpeedData.FirstOrDefault((ImpulseBySpeedTypeModel x) => x.SpeedType == speedType).ImpulseValue;
		}

		protected override bool StartCollisionEffect(GameObject other)
		{
			_randomOffset = GenerateRandomOffset();
			if (_bakedAnimPoints.IsNullOrEmpty())
			{
				return false;
			}
			Vector3 normalized = (base.transform.position - other.transform.position).normalized;
			HitDirection findedDirection = GetClosestDirection(normalized, base.transform);
			_ = GetBakedAnimParameters(other).SpeedType;
			List<BakedDirectionPoint> bakedAnimPoints = _bakedAnimPoints;
			if (bakedAnimPoints != null && bakedAnimPoints.Count > 0)
			{
				_findedBakedPoints = _bakedAnimPoints.FirstOrDefault((BakedDirectionPoint x) => x.Direction == findedDirection)?.BakedPoints;
				if (!_findedBakedPoints.IsNullOrEmpty())
				{
					PlayBakedAnim();
				}
				else
				{
					ActivatePhysicsAnimToChilds(other).Forget();
				}
			}
			else
			{
				ActivatePhysicsAnimToChilds(other).Forget();
			}
			return true;
		}

		private BakedAnimParameters GetBakedAnimParameters(GameObject other)
		{
			Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
			Vector3 position = componentInParent.position;
			float num = componentInParent.velocity.magnitude * 3.6f;
			if (num <= GetActivationImpulseByCarSpeed(BakedAnimSpeedType.Slow))
			{
				return new BakedAnimParameters(position, num, BakedAnimSpeedType.Slow);
			}
			if (num <= GetActivationImpulseByCarSpeed(BakedAnimSpeedType.Medium))
			{
				return new BakedAnimParameters(position, num, BakedAnimSpeedType.Medium);
			}
			return new BakedAnimParameters(position, num, BakedAnimSpeedType.Fast);
		}

		private void PlayBakedAnim()
		{
			PlayBakedAnimAsync(base.destroyCancellationToken).Forget();
		}

		private void StopBakedAnim()
		{
			foreach (GameObject destroyableObject in DestroyableObjects)
			{
				if ((bool)destroyableObject && destroyableObject.TryGetComponent<Collider>(out var component))
				{
					UnityEngine.Object.Destroy(component);
				}
			}
		}

		private async UniTask PlayBakedAnimAsync(CancellationToken cancellationToken)
		{
			if (_bakedAnimPoints.IsNullOrEmpty() || DestroyableObjects.IsNullOrEmpty() || _findedBakedPoints.IsNullOrEmpty())
			{
				return;
			}
			int frameIndex = 0;
			float timer = 0f;
			int framesCount = _findedBakedPoints.Count / DestroyableObjects.Count;
			while (!cancellationToken.IsCancellationRequested)
			{
				float num = (AutoDestroyAfterPlayingAnim ? ((float)framesCount * AutoDestroyFramesCoefficient) : ((float)(framesCount - 1)));
				if ((float)frameIndex >= num)
				{
					StopBakedAnim();
					SetBakingAnimFinishedStatusTrue();
					break;
				}
				timer += Time.fixedDeltaTime;
				for (int i = 0; i < DestroyableObjects.Count; i++)
				{
					GameObject gameObject = DestroyableObjects[i];
					if ((bool)gameObject)
					{
						int index = (frameIndex + 1) * DestroyableObjects.Count + i;
						BakedPointData bakedPointData = _findedBakedPoints[index];
						Vector3 b = bakedPointData.Position + _randomOffset;
						gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, b, 0.25f);
						gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, bakedPointData.Rotation, 0.25f);
					}
				}
				if (timer >= BakingTimeStep)
				{
					timer -= BakingTimeStep;
					frameIndex++;
				}
				await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
			}
		}

		private void SetBakingAnimFinishedStatusTrue()
		{
			if (AutoDestroyAfterPlayingAnim)
			{
				CommonExtensions.ClearGOCollection(_invisibles);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private Vector3 GenerateRandomOffset()
		{
			if (!UseRandomOffset)
			{
				return Vector3.zero;
			}
			if (OffsetMin == Vector3.zero && OffsetMax == Vector3.zero)
			{
				return Vector3.zero;
			}
			float x = UnityEngine.Random.Range(OffsetMin.x, OffsetMax.x);
			float y = UnityEngine.Random.Range(OffsetMin.y, OffsetMax.y);
			float z = UnityEngine.Random.Range(OffsetMin.z, OffsetMax.z);
			return new Vector3(x, y, z);
		}

		private async UniTaskVoid ActivatePhysicsAnimToChilds(GameObject other)
		{
			if (DestroyableObjects.IsNullOrEmpty())
			{
				return;
			}
			BakedAnimParameters bakedAnimParameters = GetBakedAnimParameters(other);
			float impulse = bakedAnimParameters.Impulse;
			Vector3 expCenter = bakedAnimParameters.ExplosionCenter;
			foreach (GameObject current in DestroyableObjects)
			{
				if (!current)
				{
					continue;
				}
				if (!current.TryGetComponent<Rigidbody>(out var currentRb))
				{
					currentRb = current.AddComponent<Rigidbody>();
				}
				currentRb.isKinematic = false;
				currentRb.useGravity = true;
				currentRb.mass = DestroyableObjectsMass;
				await UniTask.Yield();
				if ((bool)current)
				{
					currentRb.interpolation = RigidbodyInterpolation.Interpolate;
					currentRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
					Vector3 force = (currentRb.position - expCenter).normalized * (impulse / DestroyableObjectsMass);
					currentRb.AddForce(force, ForceMode.Force);
					if (currentRb.TryGetComponent<Collider>(out var component))
					{
						component.isTrigger = false;
					}
					currentRb = null;
				}
			}
			_physicAnimDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe(FinishBakingAnim).AddTo(this);
		}

		private void FinishBakingAnim(long _)
		{
			foreach (GameObject item in DestroyableObjects.Where(F.Exists))
			{
				if (item.TryGetComponent<Rigidbody>(out var component))
				{
					UnityEngine.Object.Destroy(component);
				}
				if (item.TryGetComponent<Collider>(out var component2))
				{
					UnityEngine.Object.Destroy(component2);
				}
			}
		}

		private HitDirection GetClosestDirection(Vector3 hitDir, Transform target)
		{
			Vector3 normalized = target.InverseTransformDirection(hitDir).normalized;
			float num = float.NegativeInfinity;
			HitDirection result = HitDirection.Forward;
			(HitDirection, Vector3)[] directionData = DirectionData;
			for (int i = 0; i < directionData.Length; i++)
			{
				(HitDirection, Vector3) tuple = directionData[i];
				HitDirection item = tuple.Item1;
				Vector3 item2 = tuple.Item2;
				float num2 = Vector3.Dot(normalized, item2);
				if (num2 > num)
				{
					num = num2;
					result = item;
				}
			}
			return result;
		}
	}
}
