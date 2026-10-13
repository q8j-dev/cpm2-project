using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class UserPreferenceAnalytic : AnalyticProviderBase
	{
		private Dictionary<int, string> _carsControlsByType = new Dictionary<int, string>
		{
			{ 0, "wheel" },
			{ 1, "tilt" },
			{ 3, "arrows" }
		};

		public UserPreferenceAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogGraphicsQuality(GraphicsQuality newQuality)
		{
			string text = PlayerPrefs.GetString("graphics_quality");
			string text2 = newQuality.ToString();
			if (!(text == text2))
			{
				PlayerPrefs.SetString("graphics_quality", text2);
				LogUserPreference(UserPreferenceType.graphics_quality, text, text2);
			}
		}

		public void LogGraphicsQuality(CPMGraphicsSettings.AdditionalEffects newQuality, CPMGraphicsSettings.AdditionalEffects oldQuality)
		{
			LogUserPreference(UserPreferenceType.additional_details, oldQuality.ToString(), newQuality.ToString());
		}

		public void LogShadowsQuality(CPMGraphicsSettings.ShadowsQuality newQuality, CPMGraphicsSettings.ShadowsQuality oldQuality)
		{
			LogUserPreference(UserPreferenceType.shadows_change, oldQuality.ToString(), newQuality.ToString());
		}

		public void LogFogQuality(CPMGraphicsSettings.FogQuality newQuality, CPMGraphicsSettings.FogQuality oldQuality)
		{
			LogUserPreference(UserPreferenceType.fog_change, oldQuality.ToString(), newQuality.ToString());
		}

		public void LogShadersQuality(CPMGraphicsSettings.ShadersQuality newQuality, CPMGraphicsSettings.ShadersQuality oldQuality)
		{
			LogUserPreference(UserPreferenceType.shaders_lighting_quality, oldQuality.ToString(), newQuality.ToString());
		}

		public void LogTonemapping(TonemappingMode newQuality)
		{
			LogUserPreference(UserPreferenceType.tonemapping_change, AnalyticKeyMapper.GetMappedValue(CPMGraphicsSettingsManager.Instance.Tonemapping), AnalyticKeyMapper.GetMappedValue(newQuality));
		}

		public void LogCarControls(int newIndex)
		{
			LogUserPreference(UserPreferenceType.controls_change, _carsControlsByType[PlayerPrefs.GetInt("steering")], _carsControlsByType[newIndex]);
		}

		public void LogDisplayMode(FullScreenMode newDisplayMode, FullScreenMode oldDisplayMode)
		{
			LogUserPreference(UserPreferenceType.display_mode, oldDisplayMode.ToString(), newDisplayMode.ToString());
		}

		public void LogResolution(int[] newResolution, int[] oldResolution)
		{
			LogUserPreference(UserPreferenceType.resolution, oldResolution.ToString(), newResolution.ToString());
		}

		public void LogUserPreference(UserPreferenceType userPreference, string oldValue, string newValue)
		{
			base.AnalyticEventFactory.CreateEvent<UserPreferenceEvent, UserPreferenceContext>(new UserPreferenceContext
			{
				UserPreferenceAction = userPreference,
				OldValue = oldValue,
				NewValue = newValue
			}).Send();
		}
	}
}
