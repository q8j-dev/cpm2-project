using CPM.Account;
using CPM.DailyTasks.Daily;
using CPM.Database;
using CPM.Rating;
using Service;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;

namespace CPM.DailyTasks
{
	public class DailyTaskWindowManager : MonoBehaviour
	{
		[SerializeField]
		[Header("PopUp Window Data")]
		private Transform MiniTaskParent;

		[SerializeField]
		private MiniTaskLabel MiniTaskPrefab;

		[SerializeField]
		private Button OpenFullDailyWindow;

		private FreeDriveDB _freeDriveDB;

		private MultiPlayerMenu multiPlayerMenu;

		private OptionalDependency<MultiPlayerMenu> _multiPlayerMenuDep;

		private LevelScript levelScript;

		private OptionalDependency<LevelScript> _levelScriptDep;

		private DailyTasksManager dailyTasksManager;

		private IPrefabFactory prefabFactory;

		private ManagerUI _managerUI;

		private IAccountController _accountController;

		private const string DailyTasksNoAccountClosedPrefsKey = "DailyTasksNoAccountClosed";

		[Inject]
		public void Construct(FreeDriveDB freeDriveDB, DailyTasksManager dailyTasksManager, IPrefabFactory prefabFactory, ManagerUI managerUI, IAccountController accountController, OptionalDependency<MultiPlayerMenu> multiPlayerMenu, OptionalDependency<LevelScript> levelScript)
		{
			_freeDriveDB = freeDriveDB;
			_multiPlayerMenuDep = multiPlayerMenu;
			_levelScriptDep = levelScript;
			this.dailyTasksManager = dailyTasksManager;
			this.prefabFactory = prefabFactory;
			dailyTasksManager.dailyTaskWindowManager = this;
			_managerUI = managerUI;
			_accountController = accountController;
		}

		private void Awake()
		{
			multiPlayerMenu = _multiPlayerMenuDep?.Value;
			levelScript = _levelScriptDep?.Value;
			Global.PopUpWindows.Unlock();
			if (base.gameObject.name != "BattlePass App")
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private void Start()
		{
			OpenFullDailyWindow.onClick.AddListener(ShowDailyTaskFullWindow);
		}

		private void ShowDailyTaskFullWindow()
		{
			_managerUI.ShowDailyTasksFull();
			_managerUI.CloseWindow("MainMenuWindow");
			_managerUI.CloseWindow("UserBalanceWindow");
		}

		public void ShowCachedDailyTasks()
		{
			if (!_accountController.HasEmail)
			{
				if (PlayerPrefs.GetInt("DailyTasksNoAccountClosed", 0) == 1)
				{
					base.gameObject.SetActive(value: false);
				}
				else
				{
					InitNoAccountDailyTasks();
				}
			}
			else if (dailyTasksManager.CachedDailyTasks != null && _accountController.HasEmail)
			{
				Init(dailyTasksManager.CachedDailyTasks);
			}
		}

		private void DestroyChilds()
		{
			foreach (Transform item in MiniTaskParent)
			{
				Object.Destroy(item.gameObject);
			}
		}

		public void Init(DailyTask[] tasks)
		{
			DestroyChilds();
			if (tasks == null || tasks.Length == 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < tasks.Length; i++)
			{
				prefabFactory.Instantiate(MiniTaskPrefab, MiniTaskParent).Init(dailyTasksManager, tasks[i]);
				if (tasks[i].status == 1)
				{
					num++;
				}
			}
			if (num == 3)
			{
				AnaliticsGoogleOwn.OnAllTaskDone();
				_ = RateAppHelper.RateAppAsync();
			}
			base.gameObject.SetActive(value: true);
		}

		public void Open()
		{
			if (dailyTasksManager.dailyTasks != null && !dailyTasksManager.gotNewTasks && _accountController.HasEmail)
			{
				if (FlagsStorage.IsFreeDriveMode)
				{
					levelScript.SetActiveFalseEachMenu(17);
				}
				if ((bool)_freeDriveDB && FlagsStorage.IsFreeDriveMode)
				{
					_freeDriveDB.SendRating(toMainMenu: false).Forget();
				}
				_managerUI.ShowDailyTasksFull();
			}
		}

		private void CloseWindowNoAccount()
		{
			PlayerPrefs.SetInt("DailyTasksNoAccountClosed", 1);
			base.gameObject.SetActive(value: false);
		}

		private void InitNoAccountDailyTasks()
		{
			int coin = 60;
			int coin2 = 90;
			int coin3 = 150;
			DailyTask[] array = new DailyTask[3]
			{
				new DailyTask(),
				new DailyTask(),
				new DailyTask()
			};
			array[0].name = "It's Play Time";
			array[0].description = "Play the game for a 30 minutes";
			array[0].goal = 30f;
			array[0].current = 0f;
			array[0].status = 0;
			array[0].type = CPM.Rating.TYPE.time;
			array[0].prize = new Prize
			{
				coin = coin,
				cash = 20000
			};
			array[1].name = "Barter";
			array[1].description = new LocalizedString("DailyTaskWindowManager.Exchange2CarsInMultiplayer");
			array[1].goal = 2f;
			array[1].current = 0f;
			array[1].status = 0;
			array[1].type = CPM.Rating.TYPE.car_exchange;
			array[1].prize = new Prize
			{
				coin = coin2,
				cash = 30000
			};
			array[2].name = "Wingman";
			array[2].description = new LocalizedString("DailyTaskWindowManager.DriveAsAPassengerFor10Km");
			array[2].goal = 10f;
			array[2].current = 0f;
			array[2].status = 0;
			array[2].type = CPM.Rating.TYPE.passanger_distance;
			array[2].prize = new Prize
			{
				coin = coin3,
				cash = 50000
			};
			dailyTasksManager.dailyTasks = new CPM.DailyTasks.Daily.DailyTasks
			{
				tasks = array
			};
			Init(array);
		}
	}
}
