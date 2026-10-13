using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class PrivacySection : SettingsSectionBase
	{
		[SerializeField]
		private Button ConfirmAgeButton;

		[SerializeField]
		private MultiplayerSection MultiplayerSection;

		private ParentalControlUtility _parentalControlUtility;

		[Inject]
		public void Construct(ParentalControlUtility parentalControlUtility)
		{
			_parentalControlUtility = parentalControlUtility;
		}

		public override void Init()
		{
			SectionToggle.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: false);
		}

		private void OnDestroy()
		{
			_parentalControlUtility.OnAgeConfirmChanged -= ChangePrivacySettingsDelegate;
		}

		private void OnConfirmAgeButtonPressed()
		{
			ManagerUI.ShowWindow("PrivacyAgeConfirmationWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		private void ChangePrivacySettingsDelegate()
		{
			MultiplayerSection.UpdatePrivacySettings();
		}
	}
}
