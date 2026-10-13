using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class ExposureParameter : PhotoModeCameraParameterWithScroll
	{
		private const string RangeMigrationKey = "PhotoMode.Exposure.Range5Migrated";

		private const float LegacyMin = -3f;

		private const float LegacyMax = 3f;

		protected override ParameterInfo GetParameterInfo()
		{
			return Config.Categories[1].Parameters[3];
		}

		protected override void OnScrollValueChanged(float value)
		{
			PhotoMode.SetCameraExposure(value);
		}

		protected override int GetSavedValue()
		{
			MigrateLegacyRangeValue();
			return base.GetSavedValue();
		}

		private void MigrateLegacyRangeValue()
		{
			if (PlayerPrefs.GetInt("PhotoMode.Exposure.Range5Migrated", 0) != 1)
			{
				PlayerPrefs.SetInt("PhotoMode.Exposure.Range5Migrated", 1);
				if (PlayerPrefs.HasKey(base.Info.NameKey))
				{
					int legacyIndex = PlayerPrefs.GetInt(base.Info.NameKey, base.Info.ScrollDefaultValues.DefaultItemIndex);
					PlayerPrefs.SetInt(base.Info.NameKey, RemapLegacyIndex(legacyIndex, base.Info.ScrollDefaultValues));
				}
			}
		}

		public static int RemapLegacyIndex(int legacyIndex, PhotoModeScrollDefaultValues newValues)
		{
			int num = newValues.ItemsCount - 1;
			int num2 = Mathf.Clamp(legacyIndex, 0, num);
			float value = Mathf.Lerp(-3f, 3f, (float)num2 / (float)num);
			return Mathf.RoundToInt(Mathf.InverseLerp(newValues.Min, newValues.Max, value) * (float)num);
		}
	}
}
