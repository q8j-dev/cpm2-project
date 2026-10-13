using System;

namespace Racing.CameraManagement
{
	[Serializable]
	public class PresetCameraWrapper
	{
		public int GarageIndex;

		public CameraOverridePreset[] Presets;
	}
}
