using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode
{
	public class PhotoModeSavesReseter
	{
		private PhotoModeConfig _config;

		public PhotoModeSavesReseter(PhotoModeConfig config)
		{
			_config = config;
		}

		public void ResetAllSaves()
		{
			ResetParametersSaves(0);
			ResetParametersSaves(1);
			ResetPanningSaves();
		}

		private void ResetParametersSaves(int category)
		{
			ParameterInfo[] parameters = _config.Categories[category].Parameters;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				PlayerPrefs.SetInt(parameterInfo.NameKey, parameterInfo.ScrollDefaultValues.DefaultItemIndex);
			}
		}

		private void ResetPanningSaves()
		{
			PlayerPrefs.SetInt(_config.Categories[2].Name, 0);
		}
	}
}
