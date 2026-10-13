using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Account;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Database;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	public class PreMenuState
	{
		private List<IReInitPreMainMenu> _reInits;

		private IAccountController _accountController;

		[Inject]
		public PreMenuState(IEnumerable<IReInitPreMainMenu> reInits, IAccountController accountController)
		{
			_accountController = accountController;
			_reInits = reInits.ToList();
		}

		public async UniTask BeforeLoadMenu(Action onEndAllPreparations)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612));
			new AlphaWipe(_accountController).AlphaVersionWipeCheck();
			await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612));
			try
			{
				Debug.Log("ReInit in Pre Menu State");
				foreach (IReInitPreMainMenu reInit in _reInits)
				{
					reInit.ReInit();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Pre Menu State Error: " + ex);
			}
			try
			{
				await UniTask.WaitWhile(() => !FirebaseInitializer.IsInitialized).Timeout(TimeSpan.FromSeconds(20.0));
			}
			catch (Exception e)
			{
				ReleaseDebug.LogError(e);
			}
			onEndAllPreparations?.Invoke();
		}
	}
}
