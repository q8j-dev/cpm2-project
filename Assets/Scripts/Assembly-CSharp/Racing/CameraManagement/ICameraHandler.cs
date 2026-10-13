using System;
using Cinemachine;
using UnityEngine;

namespace Racing.CameraManagement
{
	public interface ICameraHandler
	{
		RacingCameraType VirtualCameraType { get; }

		CinemachineBlendDefinition VirtualCameraBlend { get; }

		float ZoomDistance { get; }

		void SetActive(bool isActive, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null);

		void ChangeTarget(Transform target);

		void SetRecenter(bool isActive);

		void CameraZoom();

		void CameraZoom(float delta);

		void SetZoomValue(float value);
	}
}
