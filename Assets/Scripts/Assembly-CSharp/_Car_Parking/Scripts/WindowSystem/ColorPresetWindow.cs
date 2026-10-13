using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ColorPresetWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button CloseButton;

		private UserPreferenceAnalytic _userPreferenceAnalytic;

		private CPMGraphicsSettings.ColorCorrectionLut _startColorCorrectionLut;

		[Inject]
		public void Construct(UserPreferenceAnalytic userPreferenceAnalytic)
		{
			_userPreferenceAnalytic = userPreferenceAnalytic;
		}

		protected override void Start()
		{
			base.Start();
			CloseButton.onClick.AddListener(OpenSettings);
			InitBackButton();
		}

		protected override void OnEnable()
		{
			_startColorCorrectionLut = CPMGraphicsSettingsManager.Instance.UseColorCorrectionLut;
			base.OnEnable();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			OpenSettings();
		}

		private void OnDisable()
		{
			LogAnalytics();
		}

		public void OpenSettings()
		{
			Close();
		}

		private void LogAnalytics()
		{
			CPMGraphicsSettings.ColorCorrectionLut useColorCorrectionLut = CPMGraphicsSettingsManager.Instance.UseColorCorrectionLut;
			if (useColorCorrectionLut != _startColorCorrectionLut)
			{
				string mappedValue = AnalyticKeyMapper.GetMappedValue(_startColorCorrectionLut);
				string mappedValue2 = AnalyticKeyMapper.GetMappedValue(useColorCorrectionLut);
				_userPreferenceAnalytic.LogUserPreference(UserPreferenceType.color_correction, mappedValue, mappedValue2);
			}
		}
	}
}
