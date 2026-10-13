using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public class Audio
	{
		private const string PauseSoundFilterState = "State_All_Sound_Filter";

		private const string PauseSoundFilterOn = "filter_on";

		private const string PauseSoundFilterOff = "filter_off";

		private const string VolumeWwiseEvent = "RTPC_MAIN_VOLUME";

		private const string AmbienceVolumeEvent = "RTPC_VOLUME_AMB";

		private const string CarVolumeEvent = "RTPC_VOLUME_CAR";

		private const string UIVolumeEvent = "RTPC_VOLUME_UI";

		private const string SfxVolumeEvent = "RTPC_VOLUME_SFX";

		private const string TiresVolumeEvent = "RTPC_VOLUME_TIRES";

		private const string MusicVolumeEvent = "RTPC_VOLUME_MUSIC";

		private const string VolumePrefsKey = "volume";

		private const string AmbienceVolumePrefsKey = "AmbienceVolume";

		private const string CarVolumePrefsKey = "CarVolume";

		private const string UIVolumePrefsKey = "UIVolume";

		private const string SFXVolumePrefsKey = "SFXVolume";

		private const string TiresVolumePrefsKey = "TiresVolume";

		private const string MusicVolumePrefsKey = "MusicVolume";

		public const float ValueToReduceWiseSounds = 0.125f;

		public static float Volume { get; private set; }

		public static float AmbienceVolume { get; private set; }

		public static float CarVolume { get; private set; }

		public static float UIVolume { get; private set; }

		public static float SFXVolume { get; private set; }

		public static float TiresVolume { get; private set; }

		public static float MusicVolume { get; private set; }

		public static void SetAllVolume(float value, bool setDefaultGameValue = false, bool savePrefs = false)
		{
			AudioWrapper.SetRTPCValue("RTPC_MAIN_VOLUME", value * 100f);
			if (setDefaultGameValue)
			{
				Volume = value;
			}
			if (savePrefs)
			{
				PlayerPrefs.SetFloat("volume", value);
			}
		}

		public static void SetAmbienceVolume(float value, bool setDefaultGameValue = false, bool savePrefs = false)
		{
			AudioWrapper.SetRTPCValue("RTPC_VOLUME_AMB", value * 100f);
			if (setDefaultGameValue)
			{
				AmbienceVolume = value;
			}
			if (savePrefs)
			{
				PlayerPrefs.SetFloat("AmbienceVolume", value);
			}
		}

		public static void SetCarVolume(float value, bool setDefaultGameValue = false, bool savePrefs = false)
		{
			AudioWrapper.SetRTPCValue("RTPC_VOLUME_CAR", value * 100f);
			if (setDefaultGameValue)
			{
				CarVolume = value;
			}
			if (savePrefs)
			{
				PlayerPrefs.SetFloat("CarVolume", value);
			}
		}

		public static void SetUIVolume(float value, bool setDefaultGameValue = false, bool savePrefs = false)
		{
			AudioWrapper.SetRTPCValue("RTPC_VOLUME_UI", value * 100f);
			if (setDefaultGameValue)
			{
				UIVolume = value;
			}
			if (savePrefs)
			{
				PlayerPrefs.SetFloat("UIVolume", value);
			}
		}

		public static void SetSfxVolume(float value, bool setDefaultGameValue = false, bool savePrefs = false)
		{
			AudioWrapper.SetRTPCValue("RTPC_VOLUME_SFX", value * 100f);
			if (setDefaultGameValue)
			{
				SFXVolume = value;
			}
			if (savePrefs)
			{
				PlayerPrefs.SetFloat("SFXVolume", value);
			}
		}

		public static void SetTiresVolume(float value, bool setDefaultGameValue = false, bool savePrefs = false)
		{
			AudioWrapper.SetRTPCValue("RTPC_VOLUME_TIRES", value * 100f);
			if (setDefaultGameValue)
			{
				TiresVolume = value;
			}
			if (savePrefs)
			{
				PlayerPrefs.SetFloat("TiresVolume", value);
			}
		}

		public static void SetMusicVolume(float value, bool setDefaultGameValue = false, bool savePrefs = false)
		{
			AudioWrapper.SetRTPCValue("RTPC_VOLUME_MUSIC", value * 100f);
			if (setDefaultGameValue)
			{
				MusicVolume = value;
			}
			if (savePrefs)
			{
				PlayerPrefs.SetFloat("MusicVolume", value);
			}
		}

		public static void SetAllVolumeBySavedVolume(float value)
		{
			SetAllVolume(value * Volume);
		}

		public static float LoadVolumeFromPrefs(bool setAllVolume)
		{
			float num = (Volume = PlayerPrefs.GetFloat("volume", 1f));
			if (setAllVolume)
			{
				SetAllVolume(num);
			}
			return num;
		}

		public static float LoadAmbianceVolumeFromPrefs(bool setVolume = false)
		{
			float num = (AmbienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f));
			if (setVolume)
			{
				SetAmbienceVolume(num);
			}
			return num;
		}

		public static float LoadCarVolumeFromPrefs(bool setVolume = false)
		{
			float num = (CarVolume = PlayerPrefs.GetFloat("CarVolume", 1f));
			if (setVolume)
			{
				SetCarVolume(num);
			}
			return num;
		}

		public static float LoadUIVolumeFromPrefs(bool setVolume = false)
		{
			float num = (UIVolume = PlayerPrefs.GetFloat("UIVolume", 1f));
			if (setVolume)
			{
				SetUIVolume(num);
			}
			return num;
		}

		public static float LoadSfxVolumeFromPrefs(bool setVolume = false)
		{
			float num = (SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f));
			if (setVolume)
			{
				SetSfxVolume(num);
			}
			return num;
		}

		public static float LoadTiresVolumeFromPrefs(bool setVolume = false)
		{
			float num = (TiresVolume = PlayerPrefs.GetFloat("TiresVolume", 1f));
			if (setVolume)
			{
				SetTiresVolume(num);
			}
			return num;
		}

		public static float LoadMusicVolumeFromPrefs(bool setVolume = false)
		{
			float num = (MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f));
			if (setVolume)
			{
				SetMusicVolume(num);
			}
			return num;
		}

		public static void ResetAllVolume()
		{
			SetAllVolume(Volume);
			SetAmbienceVolume(AmbienceVolume);
			SetCarVolume(CarVolume);
			SetUIVolume(UIVolume);
			SetSfxVolume(SFXVolume);
			SetTiresVolume(TiresVolume);
			SetMusicVolume(MusicVolume);
			SetPauseSoundFilter(on: false);
		}

		public static void SetPauseSoundFilter(bool on)
		{
			string in_pszState = (on ? "filter_on" : "filter_off");
			AudioWrapper.SetState("State_All_Sound_Filter", in_pszState);
		}

		public static void MuteExcludeUI()
		{
			SetAmbienceVolume(0f);
			SetCarVolume(0f);
			SetSfxVolume(0f);
			SetTiresVolume(0f);
			SetMusicVolume(0f);
		}
	}
}
