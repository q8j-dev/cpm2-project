using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	public interface IAutoSaveService : IDisposable
	{
		bool TrySendSaveData(AutoSaveService.AutoSaveFrom autoSaveFrom);

		UniTask<AutoSaveService.SaveResult> SaveDataIfNeedAsync(AutoSaveService.AutoSaveFrom autoSaveFrom, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<bool> WaitForOngoingSaveAsync(float timeoutSec);

		void Pause();

		void Resume();
	}
}
