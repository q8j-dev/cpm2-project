using System;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.WwisePrefabs.WiseSoundScripts
{
	[Preserve]
	public class MoneyChangedSound : MonoBehaviour
	{
		private static uint SoundEvent;

		private static uint UiSwitch;

		private static uint MoneySwitch;

		private static uint CoinsSwitch;

		private static bool _wasInitialized;

		private float _lastMoneyCount;

		private int _lastCoinsCount;

		public void ResetMoneySound()
		{
			_lastMoneyCount = (int)CurrencyManagerController.Money;
		}

		private void Start()
		{
			_lastMoneyCount = (int)CurrencyManagerController.Money;
			_lastCoinsCount = CurrencyManagerController.Coins;
			CurrencyManagerController.MoneyChanged = (Action<float>)Delegate.Combine(CurrencyManagerController.MoneyChanged, new Action<float>(Sound));
			GetterCoinsManager.CoinsChanged = (Action<int, GetterCoinsManager.Source>)Delegate.Combine(GetterCoinsManager.CoinsChanged, new Action<int, GetterCoinsManager.Source>(CoinsSound));
		}

		private void OnDestroy()
		{
			CurrencyManagerController.MoneyChanged = (Action<float>)Delegate.Remove(CurrencyManagerController.MoneyChanged, new Action<float>(Sound));
			GetterCoinsManager.CoinsChanged = (Action<int, GetterCoinsManager.Source>)Delegate.Remove(GetterCoinsManager.CoinsChanged, new Action<int, GetterCoinsManager.Source>(CoinsSound));
		}

		private void Sound(float money)
		{
			if ((int)_lastMoneyCount == 0)
			{
				_lastMoneyCount = money;
			}
			else if ((int)_lastMoneyCount != (int)money)
			{
				_lastMoneyCount = money;
				TryInit();
				if (!ScenesHelper.IsLoadingSceneActive())
				{
					AudioWrapper.SetSwitch(UiSwitch, MoneySwitch, base.gameObject);
					AudioWrapper.PostEvent(SoundEvent, base.gameObject);
				}
			}
		}

		private void CoinsSound(int coins, GetterCoinsManager.Source source)
		{
			if (_lastCoinsCount != coins)
			{
				_lastCoinsCount = coins;
				TryInit();
				if (!ScenesHelper.IsLoadingSceneActive() && source != GetterCoinsManager.Source.Synchronization)
				{
					AudioWrapper.SetSwitch(UiSwitch, CoinsSwitch, base.gameObject);
					AudioWrapper.PostEvent(SoundEvent, base.gameObject);
				}
			}
		}

		private void TryInit()
		{
			if (!_wasInitialized || SoundEvent == 0)
			{
				_wasInitialized = true;
				SoundEvent = AudioWrapper.GetId("Ui_Event");
				UiSwitch = AudioWrapper.GetId("UiSwitch");
				MoneySwitch = AudioWrapper.GetId("Buy_Cell");
				CoinsSwitch = AudioWrapper.GetId("Coins");
			}
		}
	}
}
