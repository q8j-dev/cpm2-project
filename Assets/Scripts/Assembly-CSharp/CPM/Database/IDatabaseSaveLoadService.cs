using System;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace CPM.Database
{
	public interface IDatabaseSaveLoadService
	{
		event Action StartSaving;

		event Action EndSaving;

		UniTask<SlotsOnDatabaseCollection> GetSlotsOnDatabaseCollection(string hash);

		void Init();

		UniTask<FunctionCallResult<FunctionResult>> SendPlayerRecordsPartially(PlayerStorage playerStorage, bool fullData, bool isAnon);

		UniTask<FunctionCallResult<FunctionResult>> SendWalletData(WalletData walletData);

		void PrepareRecords(PlayerStorageFromDataBase records);
	}
}
