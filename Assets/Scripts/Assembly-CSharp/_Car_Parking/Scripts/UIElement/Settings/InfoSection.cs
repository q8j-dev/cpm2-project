using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class InfoSection : SettingsSectionBase
	{
		private const string EulaUrl = "https://olzhass.net/Car_Parking_Multiplayer-2_EULA.html";

		private const string PrivacyPolicyUrl = "https://olzhass.net/Car_Parking_Multiplayer2_Privacy_Policy_ios.html";

		[FormerlySerializedAs("_gameVersionText")]
		[SerializeField]
		private TMP_Text GameVersionText = default;

		[FormerlySerializedAs("_deviceModelText")]
		[SerializeField]
		private TMP_Text DeviceModelText = default;

		[SerializeField]
		private Button EulaButton = default;

		[SerializeField]
		private Button PrivacyPolicyButton = default;

		[SerializeField]
		private TextAsset EulaText = default;

		[SerializeField]
		private TextAsset PrivacyPolicyText = default;

		protected override void Awake()
		{
			base.Awake();
			if ((bool)EulaButton)
			{
				EulaButton.onClick.AddListener(OpenEula);
			}
			if ((bool)PrivacyPolicyButton)
			{
				PrivacyPolicyButton.onClick.AddListener(OpenPrivacyPolicy);
			}
		}

		private void OnDestroy()
		{
			if ((bool)EulaButton)
			{
				EulaButton.onClick.RemoveListener(OpenEula);
			}
			if ((bool)PrivacyPolicyButton)
			{
				PrivacyPolicyButton.onClick.RemoveListener(OpenPrivacyPolicy);
			}
		}

		private void OpenEula()
		{
			if ((bool)ManagerUI)
			{
				ManagerUI.ShowLegalDocumentWindow("Eula", EulaText ? EulaText.text : string.Empty, "https://olzhass.net/Car_Parking_Multiplayer-2_EULA.html").Forget();
			}
		}

		private void OpenPrivacyPolicy()
		{
			if ((bool)ManagerUI)
			{
				ManagerUI.ShowLegalDocumentWindow("Privacy Policy", PrivacyPolicyText ? PrivacyPolicyText.text : string.Empty, "https://olzhass.net/Car_Parking_Multiplayer2_Privacy_Policy_ios.html").Forget();
			}
		}

		private void OnEnable()
		{
			if (AppVersionLabel.IsInitialized)
			{
				if ((bool)GameVersionText)
				{
					GameVersionText.text = AppVersionLabel.Version;
				}
				if ((bool)DeviceModelText)
				{
					DeviceModelText.text = AppVersionLabel.DeviceModel;
				}
			}
		}
	}
}
