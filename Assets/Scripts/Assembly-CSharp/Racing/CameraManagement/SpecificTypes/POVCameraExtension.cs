using Cinemachine;
using UnityEngine;

namespace Racing.CameraManagement.SpecificTypes
{
	public class POVCameraExtension : CinemachineExtension
	{
		private CinemachinePOV _pov;

		protected override void Awake()
		{
			_pov = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
			base.Awake();
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (!(vcam.LookAt == null) && stage == CinemachineCore.Stage.Aim && !IsHorizontalAngleWithinLimit(base.transform, vcam.LookAt, 120f))
			{
				_pov.m_HorizontalAxis.Value -= Mathf.Sign(_pov.m_HorizontalAxis.Value) * 1f;
			}
		}

		private bool IsHorizontalAngleWithinLimit(Transform cameraTransform, Transform targetTransform, float angleLimit)
		{
			Vector3 normalized = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
			Vector3 normalized2 = Vector3.ProjectOnPlane(targetTransform.forward, Vector3.up).normalized;
			return Vector3.Angle(normalized, normalized2) <= angleLimit;
		}
	}
}
