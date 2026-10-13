using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class SoundSection : SettingsSectionBase
	{
		[SerializeField]
		private SliderValueHandler SoundVolume;

		[SerializeField]
		private SliderValueHandler AmbienceVolume;

		[SerializeField]
		private SliderValueHandler CarVolume;

		[SerializeField]
		private SliderValueHandler SfxVolume;

		[SerializeField]
		private SliderValueHandler TiresVolume;

		[SerializeField]
		private SliderValueHandler UIVolume;

		[SerializeField]
		private SliderValueHandler MusicVolume;

		private void Start()
		{
			Restore();
			SoundVolume.AddListener(OnAllVolumeChanging);
			SoundVolume.OnReleased += OnAllVolumeChanged;
			AmbienceVolume.AddListener((float value) =>
			{
				_Car_Parking.Scripts.Audio.Audio.SetAmbienceVolume(value, setDefaultGameValue: true);
			});
			AmbienceVolume.OnReleased += OnAmbienceVolumeChanged;
			CarVolume.AddListener((float value) =>
			{
				_Car_Parking.Scripts.Audio.Audio.SetCarVolume(value, setDefaultGameValue: true);
			});
			CarVolume.OnReleased += OnCarVolumeChanged;
			SfxVolume.AddListener((float value) =>
			{
				_Car_Parking.Scripts.Audio.Audio.SetSfxVolume(value, setDefaultGameValue: true);
			});
			SfxVolume.OnReleased += OnSfxVolumeChanged;
			TiresVolume.AddListener((float value) =>
			{
				_Car_Parking.Scripts.Audio.Audio.SetTiresVolume(value, setDefaultGameValue: true);
			});
			TiresVolume.OnReleased += OnTiresVolumeChanged;
			UIVolume.AddListener((float value) =>
			{
				_Car_Parking.Scripts.Audio.Audio.SetUIVolume(value, setDefaultGameValue: true);
			});
			UIVolume.OnReleased += OnUIVolumeChanged;
			MusicVolume.AddListener((float value) =>
			{
				_Car_Parking.Scripts.Audio.Audio.SetMusicVolume(value, setDefaultGameValue: true);
			});
			MusicVolume.OnReleased += OnMusicVolumeChanged;
		}

		private void OnAllVolumeChanging(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetAllVolume(value, setDefaultGameValue: true);
			_Car_Parking.Scripts.Audio.Audio.SetAmbienceVolume(_Car_Parking.Scripts.Audio.Audio.AmbienceVolume);
			_Car_Parking.Scripts.Audio.Audio.SetCarVolume(_Car_Parking.Scripts.Audio.Audio.CarVolume);
		}

		private void OnAllVolumeChanged(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetAllVolume(value, setDefaultGameValue: true, savePrefs: true);
		}

		private void OnAmbienceVolumeChanged(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetAmbienceVolume(value, setDefaultGameValue: true, savePrefs: true);
		}

		private void OnCarVolumeChanged(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetCarVolume(value, setDefaultGameValue: true, savePrefs: true);
		}

		private void OnTiresVolumeChanged(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetTiresVolume(value, setDefaultGameValue: true, savePrefs: true);
		}

		private void OnSfxVolumeChanged(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetSfxVolume(value, setDefaultGameValue: true, savePrefs: true);
		}

		private void OnUIVolumeChanged(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetUIVolume(value, setDefaultGameValue: true, savePrefs: true);
		}

		private void OnMusicVolumeChanged(float value)
		{
			_Car_Parking.Scripts.Audio.Audio.SetMusicVolume(value, setDefaultGameValue: true, savePrefs: true);
		}

		public override void Restore()
		{
			SoundVolume.SetValue(_Car_Parking.Scripts.Audio.Audio.LoadVolumeFromPrefs(setAllVolume: false));
			AmbienceVolume.SetValue(_Car_Parking.Scripts.Audio.Audio.LoadAmbianceVolumeFromPrefs());
			CarVolume.SetValue(_Car_Parking.Scripts.Audio.Audio.LoadCarVolumeFromPrefs());
			SfxVolume.SetValue(_Car_Parking.Scripts.Audio.Audio.LoadSfxVolumeFromPrefs());
			TiresVolume.SetValue(_Car_Parking.Scripts.Audio.Audio.LoadTiresVolumeFromPrefs());
			UIVolume.SetValue(_Car_Parking.Scripts.Audio.Audio.LoadUIVolumeFromPrefs());
			MusicVolume.SetValue(_Car_Parking.Scripts.Audio.Audio.LoadMusicVolumeFromPrefs());
		}

		protected override void DeselectSection()
		{
			base.DeselectSection();
			if (Time.timeScale == 0f)
			{
				_Car_Parking.Scripts.Audio.Audio.SetAmbienceVolume(0f);
				_Car_Parking.Scripts.Audio.Audio.SetCarVolume(0f);
				_Car_Parking.Scripts.Audio.Audio.SetSfxVolume(0f);
				_Car_Parking.Scripts.Audio.Audio.SetTiresVolume(0f);
				_Car_Parking.Scripts.Audio.Audio.SetMusicVolume(0f);
			}
		}
	}
}
