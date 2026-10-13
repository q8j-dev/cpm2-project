using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class MoneySetter : WindowScript
	{
		[SerializeField]
		private CurrencyItemView[] CurrencyItemViews = default;

		[SerializeField]
		private Button CreditsBtn = default;

		[SerializeField]
		private Button MoneyBtn = default;

		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		private RectTransform LayoutParent = default;

		protected override void Start()
		{
			base.Start();
			ManagerUI managerUI = _managerUI;
			managerUI.OnShowScreen = (Action<bool>)Delegate.Combine(managerUI.OnShowScreen, new Action<bool>(ShowUI));
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnActiveAllBalanceWindow = (Action<bool>)Delegate.Combine(managerUI2.OnActiveAllBalanceWindow, new Action<bool>(ActiveAllBalanceWindow));
			ManagerUI managerUI3 = _managerUI;
			managerUI3.OnBackButtonActivated = (Action<bool>)Delegate.Combine(managerUI3.OnBackButtonActivated, new Action<bool>(ShowBackButton));
			RestoreBalance();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ManagerUI managerUI = _managerUI;
			managerUI.OnShowScreen = (Action<bool>)Delegate.Remove(managerUI.OnShowScreen, new Action<bool>(ShowUI));
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnBackButtonActivated = (Action<bool>)Delegate.Remove(managerUI2.OnBackButtonActivated, new Action<bool>(ShowBackButton));
		}

		private void ShowBackButton(bool state)
		{
		}

		private void ActiveAllBalanceWindow(bool enable)
		{
			CurrencyManagerController.OnMoneyChanged -= UpdateM;
			CoinPurchaseWindow.OnCoinsChanged -= UpdateCR;
			if (enable)
			{
				CurrencyManagerController.OnMoneyChanged += UpdateM;
				CoinPurchaseWindow.OnCoinsChanged += UpdateCR;
				RestoreBalance();
			}
			CreditsBtn.SetActive(enable);
			MoneyBtn.SetActive(enable);
		}

		protected override void OnEnable()
		{
			RestoreBalance();
			BackButton.onClick.AddListener(BackButtonHandler);
			CreditsBtn.onClick.AddListener(CreditsBtnHandler);
			MoneyBtn.onClick.AddListener(MoneyBtnHandler);
			CurrencyManagerController.OnMoneyChanged += UpdateM;
			CoinPurchaseWindow.OnCoinsChanged += UpdateCR;
			base.OnEnable();
			BackButton.SetActive(active: false);
		}

		private void MoneyBtnHandler()
		{
			_managerUI.ShowMoneyPurchaseWindow();
		}

		private void CreditsBtnHandler()
		{
			_managerUI.ShowCoinsPurchaseWindow();
		}

		private void BackButtonHandler()
		{
			_managerUI.Back();
		}

		private void OnDisable()
		{
			ManagerUI managerUI = _managerUI;
			managerUI.OnActiveAllBalanceWindow = (Action<bool>)Delegate.Remove(managerUI.OnActiveAllBalanceWindow, new Action<bool>(ActiveAllBalanceWindow));
			BackButton.onClick.RemoveListener(BackButtonHandler);
			CreditsBtn.onClick.RemoveListener(CreditsBtnHandler);
			MoneyBtn.onClick.RemoveListener(MoneyBtnHandler);
			CurrencyManagerController.OnMoneyChanged -= UpdateM;
			CoinPurchaseWindow.OnCoinsChanged -= UpdateCR;
		}

		private void RestoreBalance()
		{
			SetBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins);
			SetBalance(CurrencyItemType.money, (int)CurrencyManagerController.Money);
		}

		private void UpdateM(object sender)
		{
			SetBalance(CurrencyItemType.money, (int)CurrencyManagerController.Money);
		}

		private void UpdateCR(object sender)
		{
			SetBalance(CurrencyItemType.coins, (int)CurrencyManagerController.Coins);
		}

		private void SetBalance(CurrencyItemType currencyId, float balance)
		{
			CurrencyItemView[] currencyItemViews = CurrencyItemViews;
			foreach (CurrencyItemView currencyItemView in currencyItemViews)
			{
				if (currencyId == currencyItemView.currencyItemType)
				{
					if (ScenesHelper.IsMenuSceneActive())
					{
						currencyItemView.SetBalance(balance);
					}
					else
					{
						currencyItemView.SetBalanceNow(balance);
					}
				}
			}
			ReloadLayout().Forget();
		}

		private async UniTaskVoid ReloadLayout()
		{
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
			LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutParent);
		}
	}
}
