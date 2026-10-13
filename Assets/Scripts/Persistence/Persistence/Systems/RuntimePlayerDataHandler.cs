using Cysharp.Threading.Tasks;
using VContainer;

namespace Persistence.Systems
{
	public class RuntimePlayerDataHandler
	{
		private DataClassesConfig dataClassesConfig;

		[Inject]
		public RuntimePlayerDataHandler(DataClassesConfig dataClassesConfig)
		{
			this.dataClassesConfig = dataClassesConfig;
		}

		public void ClearAllData()
		{
			DataHandler.ClearAllData();
		}

		public void SetAccountID(string accountID)
		{
			DataHandler.AccountID = accountID;
		}

		public async UniTask<T> GetDataObject<T>(string name)
		{
			return await DataHandler.GetDataAsync<T>(name);
		}

		public async UniTask<T> GetDataObject<T>()
		{
			return await DataHandler.GetDataAsync<T>();
		}

		public async UniTask<bool> SaveDataObject<T>(T data, string subPath)
		{
			return await DataHandler.SaveNewDataInFileAsync(data, subPath);
		}

		public async UniTask<bool> SaveDataObject<T>(T data)
		{
			return await DataHandler.SaveNewDataInFileAsync(data);
		}
	}
}
