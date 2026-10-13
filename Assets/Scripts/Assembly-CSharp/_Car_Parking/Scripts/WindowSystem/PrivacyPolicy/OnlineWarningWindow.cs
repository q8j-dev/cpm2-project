using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.PrivacyPolicy
{
	public class OnlineWarningWindow : WindowScript, IBackableWindow
	{
		public const string OnlineWarningWarningPrefs = "PrivacyOnlineWarningShown";

		public bool DontShowNexWindow;

		public const string PrefabKey = "OnlineWarningWindow";

		[SerializeField]
		private Button Button;

		public static bool ShouldShow()
		{
			return false;
		}

		public static void Shown()
		{
			PlayerPrefs.SetInt("PrivacyOnlineWarningShown", 1);
		}

		protected override void OnEnable()
		{
			Button.onClick.AddListener(Back);
			base.OnEnable();
			InitBackButton();
		}

		private void OnDisable()
		{
			Button.onClick.RemoveAllListeners();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			PlayerPrefs.SetInt("PrivacyOnlineWarningShown", 1);
			Close();
			_managerUI.TryShowPrivacyAgeConfirmationWindow(DontShowNexWindow).Forget();
		}
	}
}
