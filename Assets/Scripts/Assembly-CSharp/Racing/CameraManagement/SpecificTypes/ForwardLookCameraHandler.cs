using System;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Racing.CameraManagement.SpecificTypes
{
	public class ForwardLookCameraHandler : FreeLookCameraHandler
	{
		private CinemachineCollider _collider;

		private IDisposable _observer;

		private Tweener _tween;

		[SerializeField]
		private float delayForCollider = 0.25f;

		[SerializeField]
		private float defaultYAxis = 0.5f;

		private float _startingRadius;

		protected override bool UseUngatedLook => true;

		protected override void Awake()
		{
			base.Awake();
			_collider = _camera.GetComponent<CinemachineCollider>();
			_startingRadius = _camera.m_Orbits[1].m_Radius;
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

		public override void SetActive(bool isActive, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			if (isActive)
			{
				_collider.enabled = false;
				_camera.m_Orbits[1].m_Radius = 2f;
				base.XAxisValue = 0f;
				base.YAxisValue = defaultYAxis;
			}
			_tween?.Kill();
			_observer?.Dispose();
			if ((bool)target)
			{
				Transform transform = target.Find("CarCommonCameraTarget");
				if ((bool)transform)
				{
					target = transform;
				}
			}
			base.SetActive(isActive, target, onLiveCallback);
			if (!isActive)
			{
				return;
			}
			_observer = Observable.Timer(TimeSpan.FromSeconds(delayForCollider)).Subscribe((long _) =>
			{
				_collider.enabled = true;
				_tween = DOVirtual.Float(2f, _startingRadius, base.VirtualCameraBlend.m_Time, (float value) =>
				{
					_camera.m_Orbits[1].m_Radius = value;
				});
			}).AddTo(this);
		}
	}
}
