using System;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class CarCamerasParametersConfig : ScriptableObject
	{
		[Serializable]
		private class Config
		{
			public ParameterData RotationDamping;

			public ParameterData HeightDamping;

			public ParameterData Distance;

			public ParameterData Height;

			public ParameterData PitchAngle;

			public ParameterData YawAngle;

			public ParameterData RotationAngle;

			public ParameterData SmoothFollowAdd;
		}

		[Serializable]
		private class ParameterData
		{
			public bool Enabled;

			public float Value;

			public bool SpecialFreeDriveMode;

			public float ValueFreeDriveMode;

			public bool SpecialIsTrack;

			public float ValueTrack;

			public bool SpecialIsTall;

			public float ValueTall;

			public bool SpecialIsLevel;

			public float ValueLevel;

			private bool ShowSpecialFreeDriveMode
			{
				get
				{
					if (SpecialFreeDriveMode)
					{
						return Enabled;
					}
					return false;
				}
			}

			private bool ShowSpecialIsTrack
			{
				get
				{
					if (SpecialIsTrack)
					{
						return Enabled;
					}
					return false;
				}
			}

			private bool ShowSpecialIsTall
			{
				get
				{
					if (SpecialIsTall)
					{
						return Enabled;
					}
					return false;
				}
			}

			private bool ShowSpecialIsLevel
			{
				get
				{
					if (SpecialIsLevel)
					{
						return Enabled;
					}
					return false;
				}
			}

			public float GetValue(bool isTrack, bool isTall, float defaultValue)
			{
				if (!Enabled)
				{
					return defaultValue;
				}
				if (isTrack && SpecialIsTrack)
				{
					return ValueTrack;
				}
				if (isTall && SpecialIsTall)
				{
					return ValueTall;
				}
				if (SpecialFreeDriveMode && FlagsStorage.IsFreeDriveMode)
				{
					return ValueFreeDriveMode;
				}
				if (SpecialIsLevel && FlagsStorage.IsLevelScene)
				{
					return ValueLevel;
				}
				return Value;
			}
		}

		[SerializeField]
		private Config[] Configs;

		public void Apply(CarCameras carCameras, int index, bool isTrack, bool isTall)
		{
			if (Configs != null && index < Configs.Length && index >= 0)
			{
				Config config = Configs[index];
				carCameras.rotationDamping = config.RotationDamping.GetValue(isTrack, isTall, carCameras.rotationDamping);
				carCameras.heightDamping = config.HeightDamping.GetValue(isTrack, isTall, carCameras.heightDamping);
				carCameras.distance = config.Distance.GetValue(isTrack, isTall, carCameras.distance);
				carCameras.height = config.Height.GetValue(isTrack, isTall, carCameras.height);
				carCameras.pitchAngle = config.PitchAngle.GetValue(isTrack, isTall, carCameras.pitchAngle);
				carCameras.yawAngle = config.YawAngle.GetValue(isTrack, isTall, carCameras.yawAngle);
				carCameras.rotationAngle = config.RotationAngle.GetValue(isTrack, isTall, carCameras.rotationAngle);
				carCameras.SmoothFollowAdd = config.SmoothFollowAdd.GetValue(isTrack, isTall, carCameras.SmoothFollowAdd);
			}
		}

		public float GetPitch(int index, bool isTrack, bool isTall)
		{
			if (Configs == null || index < 0 || index >= Configs.Length)
			{
				return 0f;
			}
			return Configs[index].PitchAngle.GetValue(isTrack, isTall, 0f);
		}

		public void GetChaseParams(int index, bool isTrack, bool isTall, out float distance, out float height, out float pitch)
		{
			distance = 5f;
			height = 1.5f;
			pitch = -1f;
			if (Configs != null && index >= 0 && index < Configs.Length)
			{
				Config config = Configs[index];
				float value = config.Distance.GetValue(isTrack, isTall, distance);
				if (value > 0.01f)
				{
					distance = value;
				}
				height = config.Height.GetValue(isTrack, isTall, height);
				pitch = config.PitchAngle.GetValue(isTrack, isTall, pitch);
			}
		}
	}
}
