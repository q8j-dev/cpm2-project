using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class CamerasRenderDistance
	{
		private Camera _mainCam;

		private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();

		private List<CinemachineFreeLook> _freeLookCameras = new List<CinemachineFreeLook>();

		public void AddCamera(CinemachineVirtualCamera cinemachineVirtualCamera)
		{
			_virtualCameras.Add(cinemachineVirtualCamera);
		}

		public void AddCamera(CinemachineFreeLook cinemachineFreeLook)
		{
			_freeLookCameras.Add(cinemachineFreeLook);
		}

		public void AddCameras(CinemachineVirtualCamera[] virtualCameras)
		{
			_virtualCameras.AddRange(virtualCameras);
		}

		public void AddCameras(CinemachineFreeLook[] freeLooks)
		{
			_freeLookCameras.AddRange(freeLooks);
		}

		public void RemoveCamera(CinemachineVirtualCamera cinemachineVirtualCamera)
		{
			if ((bool)cinemachineVirtualCamera && _virtualCameras.Contains(cinemachineVirtualCamera))
			{
				_virtualCameras.Remove(cinemachineVirtualCamera);
			}
		}

		public void RemoveCamera(CinemachineFreeLook cinemachineFreeLook)
		{
			if ((bool)cinemachineFreeLook && _freeLookCameras.Contains(cinemachineFreeLook))
			{
				_freeLookCameras.Remove(cinemachineFreeLook);
			}
		}

		public void UpdateCamerasDistance(float distance)
		{
			if (!_mainCam && (bool)V.CoreGameManager)
			{
				_mainCam = V.CoreGameManager.GetComponent<Camera>();
			}
			if ((bool)_mainCam)
			{
				_mainCam.farClipPlane = distance;
			}
			if (_virtualCameras != null)
			{
				foreach (CinemachineVirtualCamera virtualCamera in _virtualCameras)
				{
					if ((bool)virtualCamera)
					{
						virtualCamera.m_Lens.FarClipPlane = distance;
					}
				}
			}
			if (_freeLookCameras == null)
			{
				return;
			}
			foreach (CinemachineFreeLook freeLookCamera in _freeLookCameras)
			{
				if ((bool)freeLookCamera)
				{
					freeLookCamera.m_Lens.FarClipPlane = distance;
				}
			}
		}
	}
}
