using System;
using System.Threading;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using RootMotion.FinalIK;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.WorldInteraction
{
	[RequireComponent(typeof(MainPerson))]
	public class WorldInteractionController : MonoBehaviour
	{
		private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

		private static readonly int XHash = Animator.StringToHash("X");

		private static readonly int ZHash = Animator.StringToHash("Z");

		[SerializeField]
		private WorldInteractionConfig _config;

		[SerializeField]
		private WorldInteractionConfig _femaleConfig;

		[SerializeField]
		private WorldInteractionSettings _settings;

		private MainPerson _person;

		private Rigidbody _rb;

		private OptionalDependency<ManagerUI> _managerUi;

		private WorldSurfaceFinder _finder;

		private WorldInteractionIkApplier _applier;

		private WorldPoseTargets _targets;

		private WorldInteractionConfig _resolvedConfig;

		private WorldPoseData _activePose;

		private CancellationTokenSource _cts;

		private Transform _surface;

		private bool _surfaceTracked;

		private Vector3 _surfaceAnchorLocal;

		private Vector3 _surfaceAnchorInvariant;

		private bool _remoteColliderOff;

		private PhotonView _view;

		private MultiPersonControl _mpc;

		private bool _remote;

		private CancellationTokenSource _remoteCts;

		private int _remoteId;

		private Vector3 _remoteAnchorWorld;

		private Vector3 _remoteRootWorld;

		private float _remoteYaw;

		private bool _netActive;

		private int _netId;

		private Vector3 _netAnchorInv;

		private Vector3 _netRootInv;

		private float _netYaw;

		private PersonAnimationsController _anim;

		private bool _busy;

		private bool _preview;

		private bool _hasEntryRoot;

		private Vector3 _entryRootInvariant;

		private bool _driveRunning;

		private bool _animFrozen;

		private float _savedAnimSpeed = 1f;

		private readonly RaycastHit[] _groundBuf = new RaycastHit[8];

		public bool IsPosed { get; private set; }

		public bool IsPoseIkActive => _applier?.IsActive ?? false;

		public int CurrentId { get; private set; }

		public WorldInteractionConfig ActivePoseConfig => ResolvePoseConfig();

		private bool IsLocal => base.gameObject == V.DriverPerson;

		public bool TryGetCameraFocus(out Vector3 focus)
		{
			if (IsPosed)
			{
				Transform transform = (((bool)_person && (bool)_person.ik) ? _person.ik.references.head : null);
				if ((bool)transform)
				{
					focus = transform.position;
					return true;
				}
				if (_targets != null && (bool)_targets.Body)
				{
					focus = _targets.Body.position;
					return true;
				}
			}
			focus = default;
			return false;
		}

		private void Awake()
		{
			_person = GetComponent<MainPerson>();
			_rb = GetComponent<Rigidbody>();
			if (_settings == null)
			{
				_settings = Resources.Load<WorldInteractionSettings>("WorldInteractionSettings");
			}
			if (_config == null)
			{
				_config = Resources.Load<WorldInteractionConfig>("WorldInteractionConfig");
			}
			if (_femaleConfig == null)
			{
				_femaleConfig = Resources.Load<WorldInteractionConfig>("WorldInteractionConfigFemale");
			}
			if (_settings == null)
			{
				_settings = ScriptableObject.CreateInstance<WorldInteractionSettings>();
			}
			_finder = new WorldSurfaceFinder(_settings);
			_view = GetComponent<PhotonView>();
			_mpc = GetComponentInParent<MultiPersonControl>();
			if (_mpc == null)
			{
				_mpc = GetComponentInChildren<MultiPersonControl>(includeInactive: true);
			}
			if (_mpc != null)
			{
				_mpc.WorldInteraction = this;
			}
		}

		private WorldInteractionConfig ResolvePoseConfig()
		{
			if ((bool)_resolvedConfig)
			{
				return _resolvedConfig;
			}
			bool flag = (int)_person.Gender == 0;
			_resolvedConfig = ((flag && (bool)_femaleConfig) ? _femaleConfig : _config);
			if (!_resolvedConfig)
			{
				_resolvedConfig = ScriptableObject.CreateInstance<WorldInteractionConfig>();
			}
			return _resolvedConfig;
		}

		[Inject]
		public void Construct(OptionalDependency<ManagerUI> managerUi)
		{
			_managerUi = managerUi;
		}

		private void ShowUnavailableHereMessage()
		{
			if (_managerUi != null && _managerUi.HasValue)
			{
				_managerUi.Value.ShowNotificationWindow(new LocalizedString("Poses.UnavailableHere"), 3f).Forget();
			}
		}

		private bool TryCreateApplier()
		{
			if (_applier != null)
			{
				return true;
			}
			if (_person.ik == null)
			{
				return false;
			}
			_applier = new WorldInteractionIkApplier(_person.ik);
			return true;
		}

		public void Toggle(int id)
		{
			if (IsPosed)
			{
				bool num = CurrentId == id;
				ExitInteraction();
				if (num)
				{
					return;
				}
			}
			RequestInteraction(id);
		}

		public void RequestInteraction(int id)
		{
			if (!_busy && !IsPosed && IsLocal && FlagsStorage.OutOfCar && TryCreateApplier())
			{
				_cts?.Dispose();
				_cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
				RunAsync(id, _cts.Token).Forget();
			}
		}

		private async UniTaskVoid RunAsync(int id, CancellationToken ct)
		{
			_ = 1;
			try
			{
				_busy = true;
				Vector3 entryRootPos = base.transform.position;
				FlagsStorage.Interacting = true;
				SetSuppress(on: true);
				StopActiveEmote();
				WorldInteractionConfig worldInteractionConfig = ResolvePoseConfig();
				WorldInteractionConfig.TypeSettings st = worldInteractionConfig.GetSettings(id);
				WorldSurfaceHit hit = (st.IsInPlace ? WorldSurfaceFinder.BuildInPlaceHit(base.transform, st.DefaultPose.Clone()) : _finder.TryFind(base.transform, id, worldInteractionConfig));
				if (!hit.Valid)
				{
					ShowUnavailableHereMessage();
					FlagsStorage.Interacting = false;
					SetSuppress(on: false);
					_busy = false;
					return;
				}
				if (!st.IsVertical && !IsGrounded())
				{
					FlagsStorage.Interacting = false;
					SetSuppress(on: false);
					_busy = false;
					return;
				}
				if (!st.IsInPlace)
				{
					if (!(await ApproachAsync(hit.ApproachPoint, ct)))
					{
						FlagsStorage.Interacting = false;
						SetSuppress(on: false);
						_busy = false;
						return;
					}
					await AlignAsync(hit.Facing, ct);
					if (ct.IsCancellationRequested)
					{
						FlagsStorage.Interacting = false;
						SetSuppress(on: false);
						_busy = false;
						return;
					}
				}
				SetWalkAnim(moving: false, 0f, 0f);
				if ((bool)_rb)
				{
					_rb.velocity = Vector3.zero;
					if (!st.IsInPlace && !st.IsVertical)
					{
						_rb.isKinematic = true;
					}
				}
				base.transform.position = hit.ApproachPoint;
				_activePose = hit.Pose;
				EnsureTargets();
				WorldPoseBuilder.Apply(_targets, _activePose, hit.SurfacePoint, hit.Facing);
				if (_activePose.UseFeet && !st.IsInPlace)
				{
					WorldPoseBuilder.ClampFeetToSurface(_targets, _settings, base.transform);
				}
				_applier.Apply(_activePose, _targets);
				TrackSurface(hit.Collider, hit.SurfacePoint);
				if (st.DisableAnimation)
				{
					FreezeAnimation(freeze: true);
				}
				_entryRootInvariant = WorldPoseSyncCodec.ToInvariant(entryRootPos, V.GlobalOffset);
				_hasEntryRoot = true;
				IsPosed = true;
				CurrentId = id;
				EnsureDriveLoop();
				SendStart(hit.SurfacePoint, hit.Facing);
			}
			catch (Exception)
			{
				SetSuppress(on: false);
				if ((bool)_rb)
				{
					_rb.velocity = Vector3.zero;
					_rb.isKinematic = false;
				}
			}
			finally
			{
				_busy = false;
				if (!IsPosed || !base.gameObject)
				{
					FlagsStorage.Interacting = false;
					InputStorage.PoseExitRequested = false;
				}
			}
		}

		private async UniTask<bool> ApproachAsync(Vector3 target, CancellationToken ct)
		{
			if ((bool)_rb)
			{
				_rb.isKinematic = false;
			}
			float t = 0f;
			while (t < _settings.ApproachTimeout)
			{
				if (ct.IsCancellationRequested)
				{
					return false;
				}
				if (IsJoystickActive() || ConsumeExitRequest())
				{
					return false;
				}
				Vector3 vector = new Vector3(target.x, base.transform.position.y, target.z);
				if (Vector3.Distance(base.transform.position, vector) <= _settings.ArriveDistance)
				{
					break;
				}
				Vector3 normalized = (vector - base.transform.position).normalized;
				if ((bool)_rb)
				{
					_rb.MovePosition(base.transform.position + normalized * (_settings.ApproachSpeed * Time.deltaTime));
				}
				Vector3 vector2 = Quaternion.Inverse(base.transform.rotation) * normalized;
				SetWalkAnim(moving: true, vector2.x, vector2.z);
				t += Time.deltaTime;
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
			SetWalkAnim(moving: false, 0f, 0f);
			return true;
		}

		private async UniTask AlignAsync(Quaternion facing, CancellationToken ct)
		{
			while (Quaternion.Angle(base.transform.rotation, facing) > _settings.ArriveAngle)
			{
				if (ct.IsCancellationRequested)
				{
					return;
				}
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, facing, _settings.RotateSpeed * Time.deltaTime);
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
			base.transform.rotation = facing;
		}

		private void EnsureTargets()
		{
			if (_targets != null && !_targets.Anchor)
			{
				_targets = null;
			}
			if (_targets == null)
			{
				_targets = WorldPoseBuilder.CreatePool();
			}
			if ((bool)V.GlobalPoint && _targets.Anchor.parent != V.GlobalPoint)
			{
				_targets.Anchor.SetParent(V.GlobalPoint, worldPositionStays: true);
			}
		}

		private void EnsureSolverInitiated()
		{
			FullBodyBipedIK ik = _person.ik;
			if ((bool)ik && !ik.solver.initiated)
			{
				ik.enabled = true;
				ik.solver.Initiate(ik.transform);
			}
		}

		private void ReleaseLegSuppressors()
		{
			if ((bool)_mpc)
			{
				RemoteLegIkSuppressor[] components = _mpc.GetComponents<RemoteLegIkSuppressor>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].ReleaseForPose();
				}
			}
		}

		private void EnsureDriveLoop()
		{
			if (!_driveRunning)
			{
				_driveRunning = true;
				DriveAsync(this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		private void OnDestroy()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
			_remoteCts?.Cancel();
			_remoteCts?.Dispose();
			_remoteCts = null;
			_targets?.Destroy();
			_targets = null;
			if (IsPosed && !_remote)
			{
				FlagsStorage.Interacting = false;
				SetSuppress(on: false);
				if (_animFrozen)
				{
					FreezeAnimation(freeze: false);
				}
			}
		}

		private async UniTaskVoid DriveAsync(CancellationToken ct)
		{
			try
			{
				while (!ct.IsCancellationRequested)
				{
					WorldInteractionIkApplier applier = _applier;
					if (applier == null || !applier.IsActive)
					{
						break;
					}
					_applier.Tick(_settings.IkWeightLerpSpeed);
					if (IsPosed && !_remote && !_preview)
					{
						if (HasSurfaceMoved())
						{
							ExitInteraction(handoffToCar: false, immediate: true);
						}
						else if (IsJoystickActive() || ConsumeExitRequest())
						{
							ExitInteraction();
						}
					}
					await UniTask.Yield(PlayerLoopTiming.Update, ct);
				}
			}
			finally
			{
				_driveRunning = false;
			}
		}

		public void StopForCarEnter()
		{
			if (IsLocal && !_remote)
			{
				ExitInteraction(handoffToCar: true);
			}
		}

		public void PreviewPose(int id)
		{
			if (TryCreateApplier())
			{
				WorldInteractionConfig.TypeSettings settings = ResolvePoseConfig().GetSettings(id);
				WorldPoseData worldPoseData = ((settings.DefaultPose != null) ? settings.DefaultPose.Clone() : new WorldPoseData
				{
					Id = id
				});
				worldPoseData.Id = id;
				WorldSurfaceHit worldSurfaceHit = WorldSurfaceFinder.BuildInPlaceHit(base.transform, worldPoseData);
				_preview = true;
				_activePose = worldSurfaceHit.Pose;
				EnsureTargets();
				Vector3 surfacePoint = worldSurfaceHit.SurfacePoint;
				surfacePoint.y += settings.PreviewGroundLift;
				WorldPoseBuilder.Apply(_targets, _activePose, surfacePoint, worldSurfaceHit.Facing);
				if (_activePose.UseFeet && !settings.IsInPlace)
				{
					WorldPoseBuilder.ClampFeetToSurface(_targets, _settings, base.transform);
				}
				_applier.Apply(_activePose, _targets);
				IsPosed = true;
				CurrentId = id;
				EnsureDriveLoop();
			}
		}

		public void StopPreview()
		{
			if (IsPosed && !_preview)
			{
				if (_remote)
				{
					return;
				}
				bool hasEntryRoot = _hasEntryRoot;
				Vector3 position = WorldPoseSyncCodec.ToWorld(_entryRootInvariant, V.GlobalOffset);
				ExitInteraction();
				if (hasEntryRoot)
				{
					base.transform.position = position;
					if ((bool)_rb)
					{
						_rb.position = position;
						_rb.velocity = Vector3.zero;
					}
				}
			}
			else
			{
				_preview = false;
				_applier?.BeginRelease();
				IsPosed = false;
				CurrentId = 0;
			}
		}

		public void ExitInteraction(bool handoffToCar = false)
		{
			ExitInteraction(handoffToCar, immediate: false);
		}

		public void CancelForTeleport()
		{
			ExitInteraction(handoffToCar: false, immediate: true);
		}

		public void CancelForRagdoll()
		{
			if (IsPosed || _busy)
			{
				if (_remote)
				{
					ReleaseRemotePose();
					SetPoseLocked(on: false);
				}
				else
				{
					ExitInteraction(handoffToCar: false, immediate: true);
				}
			}
		}

		private void TrackSurface(Collider surface, Vector3 anchorWorld)
		{
			_surface = (surface ? surface.transform : null);
			_surfaceTracked = _surface != null;
			if (_surfaceTracked)
			{
				_surfaceAnchorLocal = _surface.InverseTransformPoint(anchorWorld);
				_surfaceAnchorInvariant = WorldPoseSyncCodec.ToInvariant(anchorWorld, V.GlobalOffset);
			}
		}

		private bool HasSurfaceMoved()
		{
			if (!_surfaceTracked)
			{
				return false;
			}
			if (!_surface)
			{
				return true;
			}
			float surfaceMoveExitDistance = _settings.SurfaceMoveExitDistance;
			if (surfaceMoveExitDistance <= 0f)
			{
				return false;
			}
			return (WorldPoseSyncCodec.ToInvariant(_surface.TransformPoint(_surfaceAnchorLocal), V.GlobalOffset) - _surfaceAnchorInvariant).sqrMagnitude > surfaceMoveExitDistance * surfaceMoveExitDistance;
		}

		private void SetRemoteColliderOff(bool off)
		{
			if (_remoteColliderOff != off)
			{
				if (!_anim)
				{
					_anim = GetComponentInChildren<PersonAnimationsController>(includeInactive: true);
				}
				if ((bool)_anim && _anim.AnimationService != null)
				{
					_anim.AnimationService.TurnOnOffCollider(base.gameObject, !off);
					_remoteColliderOff = off;
				}
			}
		}

		private void ExitInteraction(bool handoffToCar, bool immediate)
		{
			_cts?.Cancel();
			InputStorage.PoseExitRequested = false;
			if (IsPosed || _busy)
			{
				if (_animFrozen)
				{
					FreezeAnimation(freeze: false);
				}
				SetWalkAnim(moving: false, 0f, 0f);
				if (handoffToCar)
				{
					_applier?.HandOff();
				}
				else if (immediate)
				{
					_applier?.ReleaseImmediate();
				}
				else
				{
					_applier?.BeginRelease();
				}
				SetSuppress(on: false);
				if ((bool)_rb && !handoffToCar)
				{
					_rb.isKinematic = false;
				}
				_preview = false;
				_surface = null;
				_surfaceTracked = false;
				_hasEntryRoot = false;
				IsPosed = false;
				CurrentId = 0;
				FlagsStorage.Interacting = false;
				if (IsLocal)
				{
					SendStop();
				}
			}
		}

		public void ApplyRemotePose(int id, Vector3 anchorWorld, float yaw, Vector3 rootWorld)
		{
			_remoteId = id;
			_remoteAnchorWorld = anchorWorld;
			_remoteRootWorld = rootWorld;
			_remoteYaw = yaw;
			_remoteCts?.Cancel();
			_remoteCts?.Dispose();
			_remoteCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			RemoteApplyAsync(_remoteCts.Token).Forget();
		}

		private async UniTaskVoid RemoteApplyAsync(CancellationToken ct)
		{
			if (_mpc != null)
			{
				_mpc.SnapForPose(_remoteRootWorld, WorldPoseSyncCodec.Facing(_remoteYaw));
			}
			while (!TryCreateApplier())
			{
				if (ct.IsCancellationRequested)
				{
					return;
				}
				if (_mpc != null)
				{
					_mpc.SnapForPose(_remoteRootWorld, WorldPoseSyncCodec.Facing(_remoteYaw));
				}
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
			WorldInteractionConfig.TypeSettings settings = ResolvePoseConfig().GetSettings(_remoteId);
			WorldPoseData worldPoseData = ((settings.DefaultPose != null) ? settings.DefaultPose.Clone() : new WorldPoseData
			{
				Id = _remoteId
			});
			worldPoseData.Id = _remoteId;
			Quaternion facing = WorldPoseSyncCodec.Facing(_remoteYaw);
			_activePose = worldPoseData;
			EnsureTargets();
			WorldPoseBuilder.Apply(_targets, _activePose, _remoteAnchorWorld, facing);
			if (_activePose.UseFeet && !settings.IsInPlace)
			{
				WorldPoseBuilder.ClampFeetToSurface(_targets, _settings, base.transform);
			}
			EnsureSolverInitiated();
			ReleaseLegSuppressors();
			_applier.Apply(_activePose, _targets);
			SetSuppress(on: true);
			StopActiveEmote();
			if (settings.DisableAnimation)
			{
				FreezeAnimation(freeze: true);
			}
			_remote = true;
			IsPosed = true;
			CurrentId = _remoteId;
			SetRemoteColliderOff(off: true);
			EnsureDriveLoop();
		}

		public void ReleaseRemotePose()
		{
			_remoteCts?.Cancel();
			_remoteCts?.Dispose();
			_remoteCts = null;
			SetRemoteColliderOff(off: false);
			if (_remote || IsPosed)
			{
				if (_animFrozen)
				{
					FreezeAnimation(freeze: false);
				}
				SetSuppress(on: false);
				_applier?.BeginRelease();
				_remote = false;
				IsPosed = false;
				CurrentId = 0;
			}
		}

		public void ClearRemotePoseLocal()
		{
			if (!_view || !_view.IsMine)
			{
				ReleaseRemotePose();
				SetPoseLocked(on: false);
			}
		}

		private void SetPoseLocked(bool on)
		{
			if (_mpc != null)
			{
				_mpc.IsPoseLocked = on;
			}
		}

		private void SendStart(Vector3 anchorWorld, Quaternion facing)
		{
			_netActive = true;
			_netId = CurrentId;
			_netAnchorInv = WorldPoseSyncCodec.ToInvariant(anchorWorld, V.GlobalOffset);
			_netRootInv = WorldPoseSyncCodec.ToInvariant(base.transform.position, V.GlobalOffset);
			_netYaw = WorldPoseSyncCodec.Yaw(facing);
			if ((bool)_view && PhotonNetwork.IsConnected)
			{
				_view.RPC("NetApplyPose", RpcTarget.Others, _netId, _netAnchorInv, _netYaw, _netRootInv);
			}
		}

		private void SendStop()
		{
			if (_netActive && (bool)_view && PhotonNetwork.IsConnected)
			{
				_view.RPC("NetReleasePose", RpcTarget.Others);
			}
			_netActive = false;
		}

		public void ResendStateTo(Player p)
		{
			if (IsLocal && !(_view == null) && PhotonNetwork.IsConnected && _netActive)
			{
				_view.RPC("NetApplyPose", p, _netId, _netAnchorInv, _netYaw, _netRootInv);
			}
		}

		[PunRPC]
		private void NetApplyPose(int id, Vector3 anchorInv, float yaw, Vector3 rootInv)
		{
			if (!(_view != null) || !_view.IsMine)
			{
				SetPoseLocked(on: true);
				ApplyRemotePose(id, WorldPoseSyncCodec.ToWorld(anchorInv, V.GlobalOffset), yaw, WorldPoseSyncCodec.ToWorld(rootInv, V.GlobalOffset));
			}
		}

		[PunRPC]
		private void NetReleasePose()
		{
			if (!(_view != null) || !_view.IsMine)
			{
				ReleaseRemotePose();
				SetPoseLocked(on: false);
			}
		}

		private void SetSuppress(bool on)
		{
			if (!_anim)
			{
				_anim = GetComponentInChildren<PersonAnimationsController>(includeInactive: true);
			}
			SitGetOutData sitGetOutData = ((!_anim) ? null : _anim.SitGetOutCarSystem?.SitGetOutData);
			if (sitGetOutData != null)
			{
				sitGetOutData.IsIKPoseActive = on;
			}
		}

		private void StopActiveEmote()
		{
			if (!_anim)
			{
				_anim = GetComponentInChildren<PersonAnimationsController>(includeInactive: true);
			}
			if ((bool)_anim)
			{
				_anim.StopSpecialAnimation();
			}
			Animator animator = _person.Animator;
			if ((bool)animator)
			{
				animator.SetBool(IsMovingHash, value: false);
			}
		}

		private void SetWalkAnim(bool moving, float x, float z)
		{
			Animator animator = _person.Animator;
			if ((bool)animator)
			{
				animator.SetBool(IsMovingHash, moving);
				animator.SetFloat(XHash, x);
				animator.SetFloat(ZHash, z);
			}
		}

		private void FreezeAnimation(bool freeze)
		{
			Animator animator = _person.Animator;
			if ((bool)animator)
			{
				if (freeze)
				{
					_savedAnimSpeed = animator.speed;
					animator.speed = 0f;
					_animFrozen = true;
				}
				else
				{
					animator.speed = _savedAnimSpeed;
					_animFrozen = false;
				}
			}
		}

		private static bool IsJoystickActive()
		{
			if (!(Mathf.Abs(InputStorage.LeftJoyInputX) > 0.01f))
			{
				return Mathf.Abs(InputStorage.LeftJoyInputY) > 0.01f;
			}
			return true;
		}

		private static bool ConsumeExitRequest()
		{
			if (!InputStorage.PoseExitRequested)
			{
				return false;
			}
			InputStorage.PoseExitRequested = false;
			return true;
		}

		private bool IsGrounded()
		{
			int num = Physics.RaycastNonAlloc(base.transform.position + Vector3.up * _settings.GroundCheckUp, maxDistance: _settings.GroundCheckUp + _settings.GroundCheckDistance, direction: Vector3.down, results: _groundBuf, layerMask: _settings.SurfaceMask, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				if (!_groundBuf[i].collider.transform.IsChildOf(base.transform))
				{
					return true;
				}
			}
			return false;
		}
	}
}
