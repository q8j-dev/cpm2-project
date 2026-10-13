using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Racing.CameraManagement
{
	public class CameraRegistrar : MonoBehaviour
	{
		[SerializeField]
		private CinemachineVirtualCameraBase VirtualCamera = default;

		[SerializeField]
		private float MinZoomDistance = -3f;

		[SerializeField]
		private float MaxZoomDistance = 5f;

		[SerializeField]
		private float PerspectiveZoomSpeed = 0.1f;

		public RacingCameraType RacingCameraType;

		public CinemachineBlendDefinition BlendMode = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);

		private float _zoomDistance;

		private CameraManager _cameraManager;

		private CinemachineVirtualCamera _virtualCamera;

		private CinemachinePOV _pov;

		private CinemachineFreeLook _freeLookCamera;

		private CinemachineCameraOffset _cameraOffset;

		private bool _isXRecenter;

		private bool _isYRecenter;

		public CinemachineVirtualCameraBase CinemachineCamera => VirtualCamera;

		[Inject]
		public void Construct(CameraManager cameraManager)
		{
			_cameraManager = cameraManager;
		}

		private void Awake()
		{
			CinemachineVirtualCameraBase virtualCamera = VirtualCamera;
			if (!(virtualCamera is CinemachineVirtualCamera cinemachineVirtualCamera))
			{
				if (virtualCamera is CinemachineFreeLook freeLookCamera)
				{
					_freeLookCamera = freeLookCamera;
					_isXRecenter = _freeLookCamera.m_RecenterToTargetHeading.m_enabled;
					_isYRecenter = _freeLookCamera.m_YAxisRecentering.m_enabled;
				}
			}
			else
			{
				_virtualCamera = cinemachineVirtualCamera;
				if (RacingCameraType == RacingCameraType.InCar)
				{
					_pov = cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
					_isXRecenter = _pov.m_HorizontalRecentering.m_enabled;
					_isYRecenter = _pov.m_VerticalRecentering.m_enabled;
				}
			}
			_cameraOffset = VirtualCamera.GetComponent<CinemachineCameraOffset>();
		}

		private void OnDestroy()
		{
		}

		internal void SetActive(bool active, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			if (active)
			{
				if (onLiveCallback != null)
				{
					if (_virtualCamera != null)
					{
						UnityAction<ICinemachineCamera, ICinemachineCamera> eventAction = null;
						eventAction = (ICinemachineCamera _, ICinemachineCamera _) =>
						{
							_virtualCamera.m_Transitions.m_OnCameraLive.RemoveListener(eventAction);
							onLiveCallback(VirtualCamera);
						};
						_virtualCamera.m_Transitions.m_OnCameraLive.AddListener(eventAction);
					}
					else if (_freeLookCamera != null)
					{
						UnityAction<ICinemachineCamera, ICinemachineCamera> eventAction2 = null;
						eventAction2 = (ICinemachineCamera _, ICinemachineCamera _) =>
						{
							_freeLookCamera.m_Transitions.m_OnCameraLive.RemoveListener(eventAction2);
							onLiveCallback(VirtualCamera);
						};
						_freeLookCamera.m_Transitions.m_OnCameraLive.AddListener(eventAction2);
					}
				}
				VirtualCamera.Follow = (target ? target : V.Car.transform);
				VirtualCamera.LookAt = (target ? target : V.Car.transform);
			}
			else
			{
				if (_cameraOffset != null)
				{
					_cameraOffset.m_Offset.z = 0f;
					_zoomDistance = 0f;
				}
				if (!CinemachineCore.Instance.IsLive(VirtualCamera))
				{
					VirtualCamera.LookAt = null;
					VirtualCamera.Follow = null;
				}
			}
			VirtualCamera.m_Priority = (active ? 10 : 0);
		}

		public void ChangeTarget(Transform target)
		{
			VirtualCamera.Follow = target;
			VirtualCamera.LookAt = target;
		}

		public void SetRecenter(bool isActive)
		{
			if (_virtualCamera != null)
			{
				if (RacingCameraType == RacingCameraType.InCar)
				{
					_pov.m_HorizontalRecentering.m_enabled = isActive && _isXRecenter;
					_pov.m_VerticalRecentering.m_enabled = isActive && _isYRecenter;
				}
			}
			else if (_freeLookCamera != null)
			{
				_freeLookCamera.m_RecenterToTargetHeading.m_enabled = isActive && _isXRecenter;
				_freeLookCamera.m_YAxisRecentering.m_enabled = isActive && _isYRecenter;
			}
		}

		public void InvokeCameraZoom(Touch touchZero, Touch touchOne)
		{
			if (!(_cameraOffset == null))
			{
				Vector2 vector = touchZero.position - touchZero.deltaPosition;
				Vector2 vector2 = touchOne.position - touchOne.deltaPosition;
				float magnitude = (vector - vector2).magnitude;
				float magnitude2 = (touchZero.position - touchOne.position).magnitude;
				float num = magnitude - magnitude2;
				_zoomDistance += num * PerspectiveZoomSpeed * 0.1f;
				_zoomDistance = Mathf.Clamp(_zoomDistance, MinZoomDistance, MaxZoomDistance);
				_cameraOffset.m_Offset.z = _zoomDistance;
			}
		}
	}
}
