using Cinemachine;
using UnityEngine;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class CinemachineCameraReferences : MonoBehaviour
	{
		[SerializeField]
		private CinemachineVirtualCamera[] VirtualCameras = default;

		[SerializeField]
		private CinemachineFreeLook[] FreeLooks = default;

		private void Start()
		{
			if (VirtualCameras.Length != 0)
			{
				CPMGraphicsSettingsManager.Instance.CamerasRenderDistance.AddCameras(VirtualCameras);
			}
			if (FreeLooks.Length != 0)
			{
				CPMGraphicsSettingsManager.Instance.CamerasRenderDistance.AddCameras(FreeLooks);
			}
		}

		private void OnDestroy()
		{
			CinemachineVirtualCamera[] virtualCameras = VirtualCameras;
			foreach (CinemachineVirtualCamera cinemachineVirtualCamera in virtualCameras)
			{
				CPMGraphicsSettingsManager.Instance.CamerasRenderDistance.RemoveCamera(cinemachineVirtualCamera);
			}
			CinemachineFreeLook[] freeLooks = FreeLooks;
			foreach (CinemachineFreeLook cinemachineFreeLook in freeLooks)
			{
				CPMGraphicsSettingsManager.Instance.CamerasRenderDistance.RemoveCamera(cinemachineFreeLook);
			}
		}
	}
}
