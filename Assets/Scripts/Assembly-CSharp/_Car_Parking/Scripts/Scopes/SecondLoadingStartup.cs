using CPM.Account;
using CPM.Connection;
using CPM.Database;
using VContainer.Unity;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Scopes
{
	public class SecondLoadingStartup : IStartable
	{
		private readonly IAccountController _accountController;

		private readonly IPlayerStorageService _playerStorageService;

		private readonly ISlotService _slotService;

		private readonly DataBase _dataBase;

		private readonly IConnectionDataHandler _connectionDataHandler;

		public SecondLoadingStartup(IAccountController accountController, IPlayerStorageService playerStorageService, ISlotService slotService, DataBase dataBase, IConnectionDataHandler connectionDataHandler)
		{
			_accountController = accountController;
			_playerStorageService = playerStorageService;
			_slotService = slotService;
			_dataBase = dataBase;
			_connectionDataHandler = connectionDataHandler;
		}

		public void Start()
		{
		}
	}
}
