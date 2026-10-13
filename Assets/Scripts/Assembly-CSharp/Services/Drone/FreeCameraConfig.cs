using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Services.Drone
{
	[CreateAssetMenu(fileName = "FreeCameraConfig", menuName = "Configs/Camera/Drone")]
	[Preserve]
	public class FreeCameraConfig : ScriptableObject
	{
		[Serializable]
		private class SpeedData
		{
			public float Acceleration = 17.5f;

			public float VerticalSpeed = 13f;

			public float RotationSpeed = 2f;
		}

		public float PerspectiveZoomSpeed = 0.5f;

		public float MaxSpeed = 30f;

		public float RotatioSpeed = 1f;

		public float SwingSpeed = 1f;

		public float AddValue = 5f;

		[SerializeField]
		private SpeedData[] SpeedDatas;

		public int SpeedModeAmount => SpeedDatas.Length;

		[Preserve]
		public FreeCameraConfig()
		{
		}

		public float GetAcceleration(int index)
		{
			return GetSpeedData(index).Acceleration;
		}

		public float GetVerticalSpeed(int index)
		{
			return GetSpeedData(index).VerticalSpeed;
		}

		public float GetRotationSpeed(int index)
		{
			return GetSpeedData(index).RotationSpeed;
		}

		private SpeedData GetSpeedData(int index)
		{
			return SpeedDatas[index % SpeedDatas.Length];
		}
	}
}
