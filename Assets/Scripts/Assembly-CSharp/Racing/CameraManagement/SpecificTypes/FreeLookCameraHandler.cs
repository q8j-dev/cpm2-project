using System;
using Cinemachine;
using UnityEngine;
using _Car_Parking.Scripts.GameInput.Settings;

namespace Racing.CameraManagement.SpecificTypes
{
	public class FreeLookCameraHandler : CameraHandlerBase<CinemachineFreeLook>
	{
		protected float TopInitHeight;

		protected float MidInitRadius;

		protected float BotInitRadius;

		[SerializeField]
		private bool KeepDistanceFromSurface = default;

		[SerializeField]
		private LayerMask SurfaceLayers = default;

		[SerializeField]
		private float SurfaceRayStandoff = 5f;

		[SerializeField]
		private float SurfaceClearance = default;

		private float _surfaceOffset;

		private CameraSurfaceTracker _surfaceTracker;

		private float _zoomTarget;

		private float _zoomVel;

		private const float ZoomSmoothTime = 0.15f;

		public float XAxisValue
		{
			get
			{
				return _camera.m_XAxis.Value;
			}
			set
			{
				_camera.m_XAxis.Value = value;
			}
		}

		public float YAxisValue
		{
			get
			{
				return _camera.m_YAxis.Value;
			}
			set
			{
				_camera.m_YAxis.Value = value;
			}
		}

		public float XAxisSpeed
		{
			get
			{
				return _camera.m_XAxis.m_MaxSpeed;
			}
			set
			{
				_camera.m_XAxis.m_MaxSpeed = value;
			}
		}

		public float YAxisSpeed
		{
			get
			{
				return _camera.m_YAxis.m_MaxSpeed;
			}
			set
			{
				_camera.m_YAxis.m_MaxSpeed = value;
			}
		}

		protected override Vector2 _multiplier => new Vector2(10.820556f, 20f / 255f);

		protected override CameraSettingsContext SettingsContext => CameraSettingsContext.Car;

		protected override float CurrentYaw
		{
			get
			{
				return _camera.m_XAxis.Value;
			}
			set
			{
				_camera.m_XAxis.Value = value;
			}
		}

		protected override float DebugTrackedYaw => _camera.m_XAxis.Value;

		protected override bool DebugIsPitchAtBottom => _camera.m_YAxis.Value >= _camera.m_YAxis.m_MaxValue - 0.001f;

		public override ref CinemachineVirtualCameraBase.TransitionParams TransitionParams => ref _camera.m_Transitions;

		private bool IsSurfaceModeEnabled
		{
			get
			{
				if (KeepDistanceFromSurface)
				{
					return IsZoomOrbitDependant;
				}
				return false;
			}
		}

		protected override void OnLookBackInstantStart()
		{
			CurrentYaw = 180f;
		}

		protected override void ApplyLook(Vector2 look)
		{
			if (!_lookOverrideActive)
			{
				_camera.m_XAxis.Value += look.x;
				_camera.m_YAxis.Value -= look.y;
			}
		}

		private void SmoothZoom()
		{
			if (!(Mathf.Abs(_zoomTarget) < 0.0001f) || !(Mathf.Abs(_zoomDistance) < 0.0001f))
			{
				_zoomDistance = Mathf.SmoothDamp(_zoomDistance, _zoomTarget, ref _zoomVel, 0.15f);
				ApplyZoomDistance(_zoomDistance);
			}
		}

		[ContextMenu("Debug/Rotate Full Circle")]
		private void DebugRotateFullCircleMenu()
		{
			DebugRotateFullCircle();
		}

		[ContextMenu("Debug/Rotate Top To Bottom")]
		private void DebugRotateTopToBottomMenu()
		{
			DebugRotateTopToBottom();
		}

		protected override void DebugMoveToPitchTop()
		{
			_camera.m_YAxis.Value = _camera.m_YAxis.m_MinValue;
		}

		protected override void Awake()
		{
			base.Awake();
			TopInitHeight = _camera.m_Orbits[0].m_Height;
			MidInitRadius = _camera.m_Orbits[1].m_Radius;
			BotInitRadius = _camera.m_Orbits[2].m_Radius;
			_isXRecenter = _camera.m_RecenterToTargetHeading.m_enabled;
			_isYRecenter = _camera.m_YAxisRecentering.m_enabled;
			_surfaceTracker = new CameraSurfaceTracker(SurfaceLayers);
			ApplyAlterSettings();
			_onLookFreeHandle.Get(_eventRegistry).Subscribe(OnLookFree);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_onLookFreeHandle.Get(_eventRegistry)?.Unsubscribe(OnLookFree);
		}

		public override void SetRecenter(bool isActive)
		{
			if (isActive)
			{
				_camera.m_RecenterToTargetHeading.m_enabled = _isXRecenter;
				_camera.m_YAxisRecentering.m_enabled = _isYRecenter;
			}
			else
			{
				_camera.m_RecenterToTargetHeading.m_enabled = false;
				_camera.m_YAxisRecentering.m_enabled = false;
			}
		}

		public override void CameraZoom(float delta)
		{
			float num = (IsZoomInverted ? (0f - delta) : delta) * ZoomSpeed * 0.1f;
			_zoomTarget = Mathf.Clamp(_zoomTarget + num, MinZoomDistance, MaxZoomDistance);
		}

		public override void SetActive(bool isActive, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			base.SetActive(isActive, target, onLiveCallback);
			if (!isActive)
			{
				_surfaceOffset = 0f;
				_camera.m_Orbits[0].m_Height = TopInitHeight;
				_camera.m_Orbits[1].m_Radius = MidInitRadius;
				_camera.m_Orbits[2].m_Radius = BotInitRadius;
				_zoomTarget = 0f;
				_zoomVel = 0f;
			}
		}

		public override void SetZoomValue(float value)
		{
			_zoomDistance = Mathf.Clamp(value, MinZoomDistance, MaxZoomDistance);
			_zoomTarget = _zoomDistance;
			ApplyZoomDistance(_zoomDistance);
		}

		private void ApplyZoomDistance(float zoomDistance)
		{
			_zoomDistance = zoomDistance;
			if (IsZoomOrbitDependant)
			{
				ApplyOrbitOffset(_zoomDistance + _surfaceOffset);
			}
			else if ((bool)_cameraOffset)
			{
				_cameraOffset.m_Offset.z = _zoomDistance;
			}
		}

		private void ApplyOrbitOffset(float offset)
		{
			_camera.m_Orbits[0].m_Height = TopInitHeight + offset;
			_camera.m_Orbits[1].m_Radius = MidInitRadius + offset;
			_camera.m_Orbits[2].m_Radius = BotInitRadius + offset;
		}

		protected void LateUpdate()
		{
			if (base.IsLiveOnBrain)
			{
				UpdateZoom();
				SmoothZoom();
				UpdateLook();
				_surfaceOffset = GetSurfaceOffset();
				ApplyOrbitOffset(_zoomDistance + _surfaceOffset);
			}
		}

		private float GetSurfaceOffset()
		{
			Transform transform = (_camera.LookAt ? _camera.LookAt : _camera.Follow);
			if (!transform)
			{
				return 0f;
			}
			Vector3 position = transform.position;
			Vector3 vector = position - _camera.State.FinalPosition;
			float magnitude = vector.magnitude;
			if (magnitude < 0.001f)
			{
				return 0f;
			}
			Vector3 vector2 = vector / magnitude;
			Vector3 origin = position - vector2 * SurfaceRayStandoff;
			if (!_surfaceTracker.TryGetSurfaceHit(origin, vector2, SurfaceRayStandoff, out var hit))
			{
				return 0f;
			}
			return Vector3.Distance(hit.point, position) + SurfaceClearance;
		}
	}
}
