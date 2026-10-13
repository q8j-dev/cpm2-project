using System;
using System.Threading;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	public class MessageFromDeveloperUI : MonoBehaviour
	{
		private const string PlayerPrefsKeyMessageFromDeveloperID = "PlayerPrefsKeyMessageFromDeveloperID";

		[SerializeField]
		private CanvasGroup Root = default;

		[SerializeField]
		private TMP_Text[] MessageTexts = default;

		[SerializeField]
		private Button CloseButton = default;

		[SerializeField]
		private RectTransform RootSimple = default;

		[SerializeField]
		private RectTransform RootScrollView = default;

		private IRemoteConfigManager _remoteConfigManager;

		private ConfigValuesStorage _remoteConfigData;

		[Inject]
		public void Construct(IRemoteConfigManager remoteConfigManager)
		{
			_remoteConfigManager = remoteConfigManager;
		}

		private void Awake()
		{
			Root.gameObject.SetActive(value: false);
		}

		private void OnEnable()
		{
			if ((bool)CloseButton)
			{
				CloseButton.onClick.AddListener(ButtonCloseClickCallback);
			}
			Initialize().Forget();
			UpdateUI();
		}

		private void OnDisable()
		{
			if ((bool)CloseButton)
			{
				CloseButton.onClick.RemoveListener(ButtonCloseClickCallback);
			}
		}

		private async UniTaskVoid Initialize()
		{
			_remoteConfigData = _remoteConfigManager.GetDefaultConfigStorage();
			_remoteConfigData = await _remoteConfigManager.GetConfigValuesStorage();
			if ((bool)this)
			{
				UpdateUI();
			}
		}

		private void ButtonCloseClickCallback()
		{
			Root.gameObject.SetActive(value: false);
			if (!string.IsNullOrEmpty(_remoteConfigData.messageFromDevelopers))
			{
				PlayerPrefs.SetInt("PlayerPrefsKeyMessageFromDeveloperID", _remoteConfigData.messageFromDevelopers.GetHashCode());
			}
		}

		private void UpdateUI()
		{
			Root.gameObject.SetActive(value: false);
			if (_remoteConfigData != null && !string.IsNullOrEmpty(_remoteConfigData.messageFromDevelopers) && _remoteConfigData.messageFromDevelopers.GetHashCode() != PlayerPrefs.GetInt("PlayerPrefsKeyMessageFromDeveloperID"))
			{
				Root.alpha = 0f;
				Root.gameObject.SetActive(value: true);
				RootSimple.gameObject.SetActive(value: true);
				RootScrollView.gameObject.SetActive(value: false);
				for (int i = 0; i < MessageTexts.Length; i++)
				{
					MessageTexts[i].text = _remoteConfigData.messageFromDevelopers;
				}
				if (base.gameObject.activeInHierarchy)
				{
					CheckScrollViewAsync(this.GetCancellationTokenOnDestroy()).Forget();
				}
			}
		}

		private async UniTaskVoid CheckScrollViewAsync(CancellationToken ct = default(CancellationToken))
		{
			try
			{
				for (int i = 0; i < 3; i++)
				{
					await UniTask.Yield(ct);
				}
				Root.alpha = 1f;
				RootSimple.gameObject.SetActive(RootSimple.rect.height < RootScrollView.rect.height);
				RootScrollView.gameObject.SetActive(!RootSimple.gameObject.activeSelf);
			}
			catch (OperationCanceledException)
			{
			}
		}
	}
}
