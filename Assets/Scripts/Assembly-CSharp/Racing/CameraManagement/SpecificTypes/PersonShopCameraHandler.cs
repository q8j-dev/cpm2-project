using System;
using System.Linq;
using CPM.Scripts.Garage;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UnityEngine;
using VContainer;

namespace Racing.CameraManagement.SpecificTypes
{
	public class PersonShopCameraHandler : FreeLookCameraHandler
	{
		private Tweener _FOVTween;

		private IDisposable _recenterDisposable;

		private IDisposable _deadZoneDisposable;

		private CinemachineComposer _composer;

		private float _initDeadZoneHeight;

		private float _initDeadZoneWidth;

		private GaragePositionsConfig _garagePositionsConfig;

		private IGarageSelectorService _garageSelectorService;

		public float MiddleRigInitScreenXCam { get; private set; }

		[Inject]
		public void Construct(GaragePositionsConfig config, IGarageSelectorService garageSelectorService)
		{
			_garagePositionsConfig = config;
			_garageSelectorService = garageSelectorService;
		}

		protected override void Awake()
		{
			base.Awake();
			_composer = _camera.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
			_initDeadZoneHeight = _composer.m_DeadZoneHeight;
			_initDeadZoneWidth = _composer.m_DeadZoneWidth;
			MiddleRigInitScreenXCam = _composer.m_ScreenX;
		}

		public override void SetZoomValue(float value)
		{
			float currentFOV = _camera.m_Lens.FieldOfView;
			_FOVTween?.Kill();
			_FOVTween = DOTween.To(() => currentFOV, (float x) =>
			{
				currentFOV = x;
			}, value, 1f).OnUpdate(() =>
			{
				_camera.m_Lens.FieldOfView = currentFOV;
			});
		}

		public override void SetActive(bool active, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			base.SetActive(active, target, onLiveCallback);
			float personShopCameraXAxis = _garagePositionsConfig.GaragePositionsDatas.First((GarageData x) => x.Index == _garageSelectorService.CurrentGarageIndex).PersonShopCameraXAxis;
			base.XAxisValue = personShopCameraXAxis;
			if (active)
			{
				float startTime = Time.time;
				float duration = Mathf.Max(base.VirtualCameraBlend.m_Time + 0.2f, 0.5f);
				_recenterDisposable?.Dispose();
				_recenterDisposable = Observable.EveryUpdate().TakeWhile((long _) => Time.time - startTime < duration).Subscribe((long _) =>
				{
					base.XAxisValue = personShopCameraXAxis;
				})
					.AddTo(this);
				ResetDeadZone();
			}
		}

		public void ResetDeadZone()
		{
			_deadZoneDisposable?.Dispose();
			_composer.m_DeadZoneHeight = 0f;
			_composer.m_DeadZoneWidth = 0f;
			_composer.m_HorizontalDamping = 0f;
			_composer.m_VerticalDamping = 0f;
			_deadZoneDisposable = Observable.Timer(TimeSpan.FromSeconds(1.5)).Subscribe((long _) =>
			{
				_composer.m_DeadZoneHeight = _initDeadZoneHeight;
				_composer.m_DeadZoneWidth = _initDeadZoneWidth;
				_composer.m_HorizontalDamping = 1f;
				_composer.m_VerticalDamping = 1f;
			}).AddTo(this);
		}

		public void SetMiddleRigScreenX(float value)
		{
			if ((bool)_composer)
			{
				_composer.m_ScreenX = value;
			}
		}

		public void LockCamera(bool isLocked)
		{
			CinemachineFreeLook camera = _camera;
			if ((bool)camera)
			{
				if (isLocked)
				{
					camera.m_XAxis.m_MaxSpeed = 0f;
					camera.m_YAxis.m_MaxSpeed = 0f;
				}
				else
				{
					camera.m_XAxis.m_MaxSpeed = 300f;
					camera.m_YAxis.m_MaxSpeed = 2f;
				}
			}
		}

		public void SetXAxisValue(float value)
		{
			CinemachineFreeLook camera = _camera;
			if ((bool)camera)
			{
				camera.m_XAxis.Value = value;
			}
		}

		public void SetOriginalZoomValue(float value)
		{
			base.SetZoomValue(value);
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
	}
}
