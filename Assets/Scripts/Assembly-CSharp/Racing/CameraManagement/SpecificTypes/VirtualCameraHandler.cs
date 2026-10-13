using Cinemachine;

namespace Racing.CameraManagement.SpecificTypes
{
	public class VirtualCameraHandler : CameraHandlerBase<CinemachineVirtualCamera>
	{
		public override ref CinemachineVirtualCameraBase.TransitionParams TransitionParams => ref _camera.m_Transitions;

		protected override void Awake()
		{
			base.Awake();
		}

		public override void SetRecenter(bool isActive)
		{
		}

		protected virtual void LateUpdate()
		{
			if (_isActive)
			{
				UpdateZoom();
				UpdateLook();
			}
		}
	}
}
