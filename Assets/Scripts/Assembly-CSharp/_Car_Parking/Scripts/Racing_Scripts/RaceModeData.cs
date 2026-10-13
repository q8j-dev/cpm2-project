using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Serializable]
	public class RaceModeData
	{
		public RacingModeType RaceMode;

		public int AdditiveLoadSceneIndex;

		public CineMachinePathData CineMachinePathData;

		public GameObject NeedToInstantiatePrefabForMode;

		public RacingCameraPreset CameraPreset;

		public RacingCameraPreset DefaultCameraPreset;
	}
}
