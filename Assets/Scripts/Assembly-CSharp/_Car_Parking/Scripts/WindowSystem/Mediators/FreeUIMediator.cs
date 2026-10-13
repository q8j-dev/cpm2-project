using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem.Mediators
{
	public class FreeUIMediator : MonoBehaviour
	{
		[Inject]
		private FreeDriveControl _freeDriveControl;

		[Inject]
		private MapCamera _mapCamera;

		public void HideBigMap()
		{
			_freeDriveControl.HideBigMap();
		}

		public void ShowBigMap()
		{
			_freeDriveControl.ShowBigMap();
		}

		public void Navigate()
		{
			_mapCamera.Navigate();
		}
	}
}
