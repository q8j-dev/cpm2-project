using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts
{
	[Preserve]
	public class CheckCarDataBase : ICheckCarDataBase
	{
		private const float DelayToRestartGame = 5f;

		private bool _isProgress;

		public bool Enabled { get; set; }

		public async UniTask CheckDataBaseAndRestartIfNeedsAsync()
		{
			if (_isProgress || !V.CoreGameManager || !Enabled)
			{
				return;
			}
			Enabled = false;
			if (await NewSaveCarSystemExtension.DataBaseNeedsToSyncAsync(V.CoreGameManager.SlotService, V.CoreGameManager.CarInfoService))
			{
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
				{
					V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("DataBase.CarsIsNotActual"), 4.5f).Forget();
					V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true).Forget();
				}
				await UniTask.Delay(TimeSpan.FromSeconds(5.0));
				SceneManager.LoadScene(0);
			}
			_isProgress = false;
		}
	}
}
