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

		[SerializeField]
		[FormerlySerializedAs("_versionItem")]
		private GameObject VersionItem;

		[FormerlySerializedAs("_deviceModelItem")]
		[SerializeField]
		private GameObject DeviceModelItem;

		[FormerlySerializedAs("_androidVersionItem")]
		[SerializeField]
		private GameObject AndroidVersionItem;

		[FormerlySerializedAs("_iosVersionItem")]
		[SerializeField]
		private GameObject IosVersionItem;

		[FormerlySerializedAs("_graphicsDeviceItem")]
		[SerializeField]
		private GameObject GraphicsDeviceItem;

		[FormerlySerializedAs("_gameVersionText")]
		[SerializeField]
		private TMP_Text GameVersionText;

		[FormerlySerializedAs("_deviceModelText")]
		[SerializeField]
		private TMP_Text DeviceModelText;

		[FormerlySerializedAs("_androidVersionText")]
		[SerializeField]
		private TMP_Text AndroidVersionText;

		[FormerlySerializedAs("_iosVersionText")]
		[SerializeField]
		private TMP_Text IosVersionText;

		[SerializeField]
		[FormerlySerializedAs("_graphicDeviceVersionText")]
		private TMP_Text GraphicDeviceVersionText;

		[SerializeField]
		private Button EulaButton;

		[SerializeField]
		private Button PrivacyPolicyButton;

		[SerializeField]
		private TextAsset EulaText;

		[SerializeField]
		private TextAsset PrivacyPolicyText;

		[SerializeField]
		private GameObject AndroidVersionTextPanel;

		[SerializeField]
		private GameObject GraphicDeviceVersionTextPanel;

		[SerializeField]
		private GameObject IosVersionTextPanel;

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
