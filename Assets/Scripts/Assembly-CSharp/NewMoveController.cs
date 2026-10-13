using System;
using System.Collections.Generic;
using System.Threading;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.WorldInteraction;

public class NewMoveController : MonoBehaviourPunCallbacks
{
	[Serializable]
	public struct InputStr
	{
		public float LookX;

		public float LookZ;

		public float RunX;

		public float RunZ;

		public int Emotion;

		public bool Jump;
	}

	private const float MaleJumpForce = 100f;

	private const float FemaleJumpForce = 150f;

	private const float MaxStepHeight = 1f;

	private const float LowStepThreshold = 0.6f;

	private const float LandingEpsilon = 0.02f;

	private const float MidJumpVelocityThreshold = 0.1f;

	private const string LastPersonCamIndexKey = "lastPersonCamIndex";

	public const int FirstPersonCameraPosition = 3;

	public const int ThirdPersonCameraPosition = 0;

	private static readonly int SpecialAnimationPlay = Animator.StringToHash("PlaySpecialAnimation");

	private static readonly int AnimationStop = Animator.StringToHash("StopAnimation");

	private TouchMovePerson _touchMovePerson;

	private Rigidbody _rigidbody;

	private MainPerson _mainPerson;

	private MultiPersonControl _multiPersonControl;

	private WorldInteractionController _worldInteractionController;

	public InputStr Input;

	public bool playingRandom;

	public float Speed = 6f;

	public PersonAnimationsController PersonAnimationsController;

	private Quaternion _lookRotation;

	public int CurrentCameraPosition = 1;

	public List<Vector3> CameraPositionArray;

	public List<int> CameraDistanceArray;

	public bool RotateCameraAroundPerson;

	private bool _isInCar;

	private GameObject _topRaycast;

	private GameObject _bottomRaycast;

	private RaycastHit _bottomHit;

	private CapsuleCollider _capsuleCollider;

	public Vector3 inputToAnimation;

	private IDisposable _disposableJumpAddForce;

	private bool _canMove;

	private bool _shouldUpdateMovement;

	private static readonly int AlterIdleStateHash = Animator.StringToHash("idle 2");

	private static readonly int AlterIdleStateHashFemale = Animator.StringToHash("iddle 2");

	private bool _alterWasMoving;

	private bool _emotionInputActive;

	private static NewMoveController _driverMoveCache;

	public bool IsFirstPersonCamera => CurrentCameraPosition == 3;

	public bool IsEmotePlaying => Input.Emotion != 0;

	public bool IsHoldingAnimation
	{
		get
		{
			if (Input.Emotion == 0 && (!PersonAnimationsController || PersonAnimationsController.CurrentSpecialAnimationId == 0))
			{
				if ((bool)_worldInteractionController)
				{
					return _worldInteractionController.IsPosed;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsDriverHoldingAnimation()
	{
		if (!V.DriverPerson)
		{
			return false;
		}
		if (!_driverMoveCache || _driverMoveCache.gameObject != V.DriverPerson)
		{
			_driverMoveCache = V.DriverPerson.GetComponent<NewMoveController>();
		}
		if ((bool)_driverMoveCache)
		{
			return _driverMoveCache.IsHoldingAnimation;
		}
		return false;
	}

	private void Awake()
	{
		TryGetComponent<PersonAnimationsController>(out PersonAnimationsController);
		TryGetComponent<TouchMovePerson>(out _touchMovePerson);
		TryGetComponent<Rigidbody>(out _rigidbody);
		TryGetComponent<MultiPersonControl>(out _multiPersonControl);
		TryGetComponent<MainPerson>(out _mainPerson);
		TryGetComponent<CapsuleCollider>(out _capsuleCollider);
		TryGetComponent<WorldInteractionController>(out _worldInteractionController);
		_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	private void OnDestroy()
	{
		_disposableJumpAddForce?.Dispose();
	}

	private void UpdateMovementState()
	{
		_shouldUpdateMovement = ((!PhotonNetwork.IsConnected) ? (FlagsStorage.OutOfCar && (!FlagsStorage.IsMainMenu || FlagsStorage.FreeWalk || V.CoreGameManager.HasFirstCar())) : (base.photonView.IsMine ? FlagsStorage.OutOfCar : (!_multiPersonControl.currentCar)));
		_canMove = FlagsStorage.OutOfCar && base.gameObject == V.DriverPerson && (!FlagsStorage.IsMainMenu || FlagsStorage.FreeWalk || V.CoreGameManager.HasFirstCar());
		if (FlagsStorage.UseNewLocomotion && base.gameObject == V.DriverPerson && (!PhotonNetwork.IsConnected || base.photonView.IsMine))
		{
			_canMove = false;
			_shouldUpdateMovement = false;
		}
	}

	private void Update()
	{
		if ((bool)PersonAnimationsController)
		{
			UpdateMovementState();
			UpdateCarState();
			if (_canMove)
			{
				UpdateRaycasts();
			}
		}
	}

	private void UpdateCarState()
	{
		bool flag = ((!PhotonNetwork.IsConnected) ? (!FlagsStorage.OutOfCar) : (base.photonView.IsMine ? (!FlagsStorage.OutOfCar) : ((bool)_multiPersonControl.currentCar)));
		if (_isInCar != flag)
		{
			_isInCar = flag;
			PersonAnimationsController.SetBoolIsMoving(value: false);
			if (!_isInCar && !_rigidbody.isKinematic)
			{
				_rigidbody.velocity = Vector3.zero;
			}
		}
	}

	private void UpdateRaycasts()
	{
		Vector3 position = base.transform.position;
		Vector3 direction = base.transform.TransformDirection(Vector3.forward);
		_topRaycast = (Physics.Raycast(new Vector3(position.x, position.y + 0.6f, position.z), direction, out var hitInfo, 0.5f, -1, QueryTriggerInteraction.Ignore) ? hitInfo.transform.gameObject : null);
		_bottomRaycast = (Physics.Raycast(new Vector3(position.x, position.y + 0.05f, position.z), direction, out _bottomHit, 0.5f, -1, QueryTriggerInteraction.Ignore) ? _bottomHit.transform.gameObject : null);
	}

	public void PlaySpecialAnimation(Animator animator, AnimationDataSO.AnimationDataItem animationDataItem)
	{
		if (!animator || animationDataItem == null)
		{
			return;
		}
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
		animator.runtimeAnimatorController = animatorOverrideController;
		animatorOverrideController["SpecialAnimations"] = animationDataItem.animationClip;
		animator.SetTrigger(SpecialAnimationPlay);
		Observable.Timer(TimeSpan.FromSeconds(animationDataItem.duration)).Subscribe((long _) =>
		{
			if ((bool)animator)
			{
				animator.SetTrigger(AnimationStop);
			}
		}).AddTo(this);
	}

	public async UniTaskVoid SetRandomAnimator(Animator animator, AnimationDataSO.AnimationDataItem[] animation, int idleRandom = 30)
	{
		if (!animator || animation == null || animation.Length == 0 || !this)
		{
			return;
		}
		playingRandom = true;
		int last = -1;
		CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
		try
		{
			while (playingRandom && (bool)this && (bool)base.gameObject)
			{
				PersonAnimationsController?.StopSpecialAnimation();
				int random;
				do
				{
					random = UnityEngine.Random.Range(0, animation.Length + idleRandom);
				}
				while (random == last);
				last = random;
				int randomTimeDuration = UnityEngine.Random.Range(1, 5);
				if (random >= animation.Length)
				{
					await UniTask.Delay(randomTimeDuration * 1000, ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
					continue;
				}
				for (int i = 0; i < randomTimeDuration; i++)
				{
					if ((bool)PersonAnimationsController)
					{
						PersonAnimationsController.PlaySpecialAnimation(animation[random].Y);
					}
					else
					{
						PlaySpecialAnimation(animator, animation[random]);
					}
					await UniTask.Delay((int)(animation[random].duration * 1000f), ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
					PersonAnimationsController?.StopSpecialAnimation();
					await UniTask.Delay(1, ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
				}
			}
		}
		finally
		{
			playingRandom = false;
			if ((bool)this && (bool)base.gameObject && (bool)PersonAnimationsController)
			{
				PersonAnimationsController.StopSpecialAnimation();
			}
		}
	}

	private void SetLocalVelocity()
	{
		if ((bool)PersonAnimationsController && (bool)_rigidbody && !playingRandom)
		{
			if ((PhotonNetwork.IsConnected && base.photonView.IsMine) || !PhotonNetwork.IsConnected)
			{
				RotCamAroundCharacterFunc();
			}
			bool num = (bool)_worldInteractionController && _worldInteractionController.IsPosed;
			Vector3 vector = Quaternion.Inverse(base.transform.rotation) * (inputToAnimation / Speed);
			bool flag = !num && (Mathf.Abs(vector.x) > 0.01f || Mathf.Abs(vector.z) > 0.01f);
			PersonAnimationsController.SetBoolIsMoving(flag);
			if (flag)
			{
				EmotionController.oldSelected = 0;
				PersonAnimationsController.PlayMoveAnimation(vector.x, vector.z);
			}
			if (Mathf.Abs(Input.RunX) > 0.05f || Mathf.Abs(Input.RunZ) > 0.05f)
			{
				Input.Emotion = 0;
				EmotionController.oldSelected = 0;
			}
			if (Input.Emotion == 0 && _emotionInputActive && PersonAnimationsController.CurrentSpecialAnimationId != 0)
			{
				PersonAnimationsController.StopSpecialAnimation();
			}
			_emotionInputActive = Input.Emotion != 0;
			PersonAnimationsController.PlaySpecialAnimation(Input.Emotion);
			if (Input.Emotion == 1 && base.gameObject == V.DriverPerson)
			{
				SetJumpForce();
			}
		}
	}

	private void FixedUpdate()
	{
		if ((MathF.Abs(Input.RunX) > 0.2f || MathF.Abs(Input.RunZ) > 0.2f) && (bool)_multiPersonControl.driverActionController.ragdollCpm && _multiPersonControl.driverActionController.ragdollCpm.isRagdoll && !_multiPersonControl.driverActionController.ragdollCpm.isStadningUp)
		{
			if (_multiPersonControl.driverActionController.ragdollCpm.CanStandUpFromInput)
			{
				_multiPersonControl.driverActionController.ragdollCpm.DisableRagdoll();
			}
			Input.RunX = 0f;
			Input.RunZ = 0f;
			PersonAnimationsController.PlaySpecialAnimation(Input.Emotion);
		}
		else if ((bool)PersonAnimationsController)
		{
			if (_shouldUpdateMovement && (!PhotonNetwork.IsConnected || base.photonView.IsMine))
			{
				SetLocalVelocity();
			}
			else if (FlagsStorage.UseNewLocomotion && base.gameObject == V.DriverPerson && (!PhotonNetwork.IsConnected || base.photonView.IsMine) && FlagsStorage.OutOfCar)
			{
				PersonAnimationsController.PlaySpecialAnimation(Input.Emotion);
			}
			if (_canMove)
			{
				HandleMovement();
			}
		}
	}

	private void HandleMovement()
	{
		if (FlagsStorage.Interacting)
		{
			return;
		}
		if (!RotateCameraAroundPerson)
		{
			Vector3 to = Vector3.ClampMagnitude(new Vector3(Input.LookX, 0f, Input.LookZ), 1f);
			if (to.magnitude > 0.03f)
			{
				_lookRotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, to, Vector3.up), Vector3.up);
			}
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, _lookRotation, Time.deltaTime * Speed);
		}
		Vector3 vector = Vector3.ClampMagnitude(new Vector3(Input.RunX, 0f, Input.RunZ), 1f);
		inputToAnimation = new Vector3(vector.x * Speed, 0f, vector.z * Speed);
		bool flag = vector.sqrMagnitude < 0.0025f;
		if (flag)
		{
			if ((bool)_rigidbody)
			{
				Vector3 velocity = _rigidbody.velocity;
				_rigidbody.velocity = new Vector3(0f, velocity.y, 0f);
			}
			if (_alterWasMoving && Input.Emotion == 0 && (bool)PersonAnimationsController && (bool)PersonAnimationsController.Animator)
			{
				Animator animator = PersonAnimationsController.Animator;
				int num = (animator.HasState(0, AlterIdleStateHash) ? AlterIdleStateHash : (animator.HasState(0, AlterIdleStateHashFemale) ? AlterIdleStateHashFemale : 0));
				if (num != 0)
				{
					animator.CrossFadeInFixedTime(num, 0.08f, 0);
				}
			}
		}
		_alterWasMoving = !flag;
		TryStepUp();
	}

	private void TryStepUp()
	{
		if (!_bottomHit.transform || new Vector2(Input.RunX, Input.RunZ).sqrMagnitude < 0.0025f || Vector3.Angle(_bottomHit.normal, -base.transform.forward) >= 30f || ((bool)_rigidbody && _rigidbody.velocity.y > 0.1f) || !TryFindLedgeTop(out var ledgeY))
		{
			return;
		}
		float num = ledgeY - base.transform.position.y;
		if (num <= 0f)
		{
			return;
		}
		if (num <= 0.6f)
		{
			Vector3 b = base.transform.position + Vector3.up * 2f;
			base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * 1.5f);
		}
		else if (!(num > 1f))
		{
			Vector3 vector = new Vector3(base.transform.position.x, ledgeY + 0.02f, base.transform.position.z);
			if (IsLandingClear(vector))
			{
				Vector3 velocity = _rigidbody.velocity;
				velocity.y = 0f;
				_rigidbody.velocity = velocity;
				base.transform.position = vector;
			}
		}
	}

	private bool TryFindLedgeTop(out float ledgeY)
	{
		ledgeY = 0f;
		if (!_bottomHit.transform)
		{
			return false;
		}
		if (!Physics.Raycast(_bottomHit.point + Vector3.up * 1f + base.transform.forward * 0.05f, Vector3.down, out var hitInfo, 1.1f, -1, QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		if (Vector3.Angle(hitInfo.normal, Vector3.up) > 45f)
		{
			return false;
		}
		ledgeY = hitInfo.point.y;
		return true;
	}

	private bool IsLandingClear(Vector3 landingPos)
	{
		if (!_capsuleCollider)
		{
			return true;
		}
		float radius = _capsuleCollider.radius;
		float num = Mathf.Max(_capsuleCollider.height, radius * 2f);
		Vector3 vector = landingPos + _capsuleCollider.center;
		float num2 = num * 0.5f - radius;
		Vector3 up = base.transform.up;
		Vector3 start = vector + up * num2;
		Vector3 end = vector - up * num2;
		return !Physics.CheckCapsule(start, end, radius, -1, QueryTriggerInteraction.Ignore);
	}

	public void ChangeCameraPosition()
	{
		CurrentCameraPosition++;
		if (ScenesHelper.IsMenuSceneActive())
		{
			int currentCameraPosition = CurrentCameraPosition;
			if (currentCameraPosition == 1 || currentCameraPosition == 2)
			{
				CurrentCameraPosition = 3;
			}
		}
		if (CurrentCameraPosition >= CameraPositionArray.Count)
		{
			CurrentCameraPosition = 0;
		}
		ApplyCameraPosition(save: true);
	}

	public void SetCameraPosition(int position, bool save)
	{
		if (position >= 0 && position < CameraPositionArray.Count)
		{
			CurrentCameraPosition = position;
			ApplyCameraPosition(save);
		}
	}

	private void ApplyCameraPosition(bool save)
	{
		if (CameraPositionArray.Count != 0 && CameraDistanceArray.Count != 0 && (bool)_touchMovePerson)
		{
			_touchMovePerson.CameraPivot = CameraPositionArray[CurrentCameraPosition];
			_touchMovePerson.CameraDistance = CameraDistanceArray[CurrentCameraPosition];
			_touchMovePerson.CameraDistanceGlobal = _touchMovePerson.CameraDistance;
			if (CurrentCameraPosition == 3)
			{
				_touchMovePerson.angleMax = 60f;
				_touchMovePerson.angleMaxUp = -30f;
				_touchMovePerson.CamPosLerpSpeed = 1000f;
			}
			else
			{
				_touchMovePerson.CamPosLerpSpeed = 7f;
				_touchMovePerson.angleMax = 45f;
				_touchMovePerson.angleMaxUp = -10f;
			}
			if (save)
			{
				PlayerPrefs.SetInt("lastPersonCamIndex", CurrentCameraPosition);
			}
		}
	}

	public void SetCameraPosition(int index)
	{
		if (CameraPositionArray != null && CameraDistanceArray != null && CameraPositionArray.Count != 0 && CameraDistanceArray.Count != 0 && (bool)_touchMovePerson)
		{
			CurrentCameraPosition = Mathf.Clamp(index, 0, CameraPositionArray.Count - 1);
			_touchMovePerson.CameraPivot = CameraPositionArray[CurrentCameraPosition];
			_touchMovePerson.CameraDistance = CameraDistanceArray[CurrentCameraPosition];
			_touchMovePerson.CameraDistanceGlobal = _touchMovePerson.CameraDistance;
			if (CurrentCameraPosition == 3)
			{
				_touchMovePerson.angleMax = 60f;
				_touchMovePerson.angleMaxUp = -30f;
				_touchMovePerson.CamPosLerpSpeed = 1000f;
			}
			else
			{
				_touchMovePerson.CamPosLerpSpeed = 7f;
				_touchMovePerson.angleMax = 45f;
				_touchMovePerson.angleMaxUp = -10f;
			}
			PlayerPrefs.SetInt("lastPersonCamIndex", CurrentCameraPosition);
		}
	}

	public void StopMove()
	{
		PersonAnimationsController?.StopMoveAnimation();
	}

	private void RotCamAroundCharacterFunc()
	{
		bool flag = Mathf.Abs(InputStorage.LeftJoyInputX) < 0.01f && Mathf.Abs(InputStorage.LeftJoyInputY) < 0.01f;
		bool flag2 = (bool)_touchMovePerson && _touchMovePerson.IsKeyboardMovement;
		RotateCameraAroundPerson = flag && !flag2;
	}

	private void JumpAddForce()
	{
		if ((bool)_mainPerson && (bool)_rigidbody)
		{
			float num = (((int)_mainPerson.Gender == 0) ? 100f : 150f);
			_rigidbody.AddForce(base.transform.forward * num);
		}
	}

	private void SetJumpForce()
	{
		_disposableJumpAddForce?.Dispose();
		_disposableJumpAddForce = Observable.Timer(TimeSpan.FromSeconds(0.20000000298023224)).Take(1).Subscribe((long _) =>
		{
			JumpAddForce();
		})
			.AddTo(this);
	}
}
