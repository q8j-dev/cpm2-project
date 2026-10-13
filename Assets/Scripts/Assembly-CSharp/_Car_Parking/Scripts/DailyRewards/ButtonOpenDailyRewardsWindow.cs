using System.Collections.Generic;
using CPM.Account;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class ButtonOpenDailyRewardsWindow : MonoBehaviour
	{
		private const float ConfirmWindowBackgroundAlpha = 0.925f;

		[SerializeField]
		private Button Button;

		[SerializeField]
		private GameObject AmountRewardsRoot;

		[SerializeField]
		private TMP_Text AmountRewardsText;

		private IDailyRewardsServer _dailyRewardsServer;

		private IAccountController _accountController;

		private ManagerUI _managerUI;

		[Inject]
		public void Construct(IDailyRewardsServer dailyRewardsServer, IAccountController accountController, ManagerUI managerUI)
		{
			_dailyRewardsServer = dailyRewardsServer;
			_accountController = accountController;
			_managerUI = managerUI;
		}

		private void OnEnable()
		{
			UpdateUI();
			_dailyRewardsServer.OnDailyRewardsChanged += OnDailyRewardsChanged;
			Button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			_dailyRewardsServer.OnDailyRewardsChanged -= OnDailyRewardsChanged;
			Button.onClick.RemoveListener(OnButtonClick);
		}

		private void OnDailyRewardsChanged()
		{
			UpdateUI();
		}

		private void OnButtonClick()
		{
			if (_accountController.HasEmail)
			{
				_managerUI.ShowWindow("DailyRewardsWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: true).Forget();
				return;
			}
			UpdateWindowShowUI(state: false);
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new Color(0f, 0f, 0f, 0.925f));
			_managerUI.ShowConfirmWindow(() =>
			{
				UpdateWindowShowUI(state: true);
				AuthorizationWindow.AdditionalData value = new AuthorizationWindow.AdditionalData
				{
					BackWindowKey = "MainMenuWindow"
				};
				_managerUI.ShowWindow("AuthorizationWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: true, cleanScreen: true, JsonConvert.SerializeObject(value)).Forget();
			}, new LocalizedString("DailyRewards.SignInToGetRewards.Title"), new LocalizedString("DailyRewards.SignInToGetRewards.Content"), () =>
			{
				UpdateWindowShowUI(state: true);
			}, "", null, additionalData).Forget();
		}

		private void UpdateUI()
		{
			IReadOnlyList<DailyRewardData> rewards = _dailyRewardsServer.Rewards;
			int num = 0;
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].Status == DailyRewardData.StatusType.Opened)
				{
					num++;
				}
			}
			AmountRewardsText.text = num.ToString() ?? "";
			AmountRewardsRoot.SetActive(num > 0);
		}

		private void UpdateWindowShowUI(bool state)
		{
			WindowScript componentInParent = GetComponentInParent<WindowScript>();
			if ((bool)componentInParent)
			{
				componentInParent.ShowUI(state);
			}
		}
	}
}
