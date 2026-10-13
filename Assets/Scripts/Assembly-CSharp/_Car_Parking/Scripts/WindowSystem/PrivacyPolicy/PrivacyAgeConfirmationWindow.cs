using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.WindowSystem.PrivacyPolicy
{
	public class PrivacyAgeConfirmationWindow : WindowScript, IBackableWindow
	{
		public const string PrefsShownPrefsKey = "PrivacyAgeShown";

		public const string PrefabKey = "PrivacyAgeConfirmationWindow";

		[SerializeField]
		private Button ConfirmButton;

		[SerializeField]
		private Button RejectButton;

		private ParentalControlUtility _parentalControlUtility;

		[Inject]
		public void Construct(ParentalControlUtility parentalControlUtility)
		{
			_parentalControlUtility = parentalControlUtility;
		}

		protected override void OnEnable()
		{
			ConfirmButton.onClick.AddListener(Confirm);
			RejectButton.onClick.AddListener(Reject);
			base.OnEnable();
			InitBackButton();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
		}

		private void OnDisable()
		{
			ConfirmButton.onClick.RemoveAllListeners();
			RejectButton.onClick.RemoveAllListeners();
		}

		private void Confirm()
		{
			PlayerPrefs.SetInt("privacyAgeConfirmed", 1);
			PlayerPrefs.SetInt("VoiceChat", 1);
			_parentalControlUtility.InvokeAgeConfirmChanged();
			Shown();
			Close();
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		}

		private void Reject()
		{
			PlayerPrefs.SetInt("privacyAgeConfirmed", 2);
			PlayerPrefs.SetInt("VoiceChat", 0);
			_parentalControlUtility.InvokeAgeConfirmChanged();
			Shown();
			Close();
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		}

		private void Shown()
		{
			PlayerPrefs.SetInt("PrivacyAgeShown", 1);
		}

		public static bool ShouldShow()
		{
			return PlayerPrefs.GetInt("PrivacyAgeShown", 0) == 0;
		}
	}
}
