using CommonUtility.Contracts;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.DataStorageService
{
	public class StorageService : IStorageService, IFirstSignInDependant
	{
		private GaragePositionsConfig _garagePositionsConfig;

		public DataStorageResources Data { get; set; }

		[Inject]
		public StorageService(GaragePositionsConfig garagePositionsConfig)
		{
			_garagePositionsConfig = garagePositionsConfig;
			Init();
		}

		private void Init()
		{
			Data = new DataStorageResources();
			Data.InventoryData = new InventoryData();
			Data.PlayerStorage = new PlayerStorage();
			Data.GarageSpotCollection = new GarageSpotCollection();
			Data.EngineKitsInventoryData = new EngineKitsInventoryData();
		}

		public UniTask OnFirstSignIn()
		{
			return UniTask.CompletedTask;
		}

		public void OnSignOut()
		{
			Init();
		}
	}
}
