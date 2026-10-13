using System;
using CPM.DailyTasks;
using CPM.DailyTasks.Daily;
using CPM.Database;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;

public class DailyTaskFullWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private Image timeProgress;

	[SerializeField]
	private TextMeshProUGUI timeLeftLabel;

	[SerializeField]
	private TaskLabel TaskPrefab;

	[SerializeField]
	private Transform TaskParent;

	[SerializeField]
	private Button BackButton;

	[SerializeField]
	private GameObject NoInternetConnectionPanel;

	private DailyTasksManager _dailyTasksManager;

	private IPrefabFactory _prefabFactory;

	private DateTime deadlineUtcDate;

	private FreeDriveDB _freeDriveDB;

	private IOfflineModeSystem _offlineModeSystem;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(DailyTasksManager dailyTasksManager, IPrefabFactory prefabFactory, FreeDriveDB freeDriveDB, IOfflineModeSystem offlineModeSystem)
	{
		_freeDriveDB = freeDriveDB;
		_dailyTasksManager = dailyTasksManager;
		_prefabFactory = prefabFactory;
		_offlineModeSystem = offlineModeSystem;
	}

	protected override void Start()
	{
		BackButton.onClick.AddListener(BackClickCallback);
		long milliseconds = Convert.ToInt64(_dailyTasksManager.timeMilliseconds);
		deadlineUtcDate = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).Date;
		deadlineUtcDate = deadlineUtcDate.AddDays(1.0);
		UpdateTimeLeft();
		GenerateTasks();
		if ((bool)_freeDriveDB && FlagsStorage.IsFreeDriveMode)
		{
			_freeDriveDB.SendRating(toMainMenu: false).Forget();
		}
		NoInternetConnectionPanel.SetActive(_offlineModeSystem.Enabled);
		base.Start();
		InitBackButton();
	}

	protected override void OnEnable()
	{
		BackButton.gameObject.SetActive(value: false);
		base.OnEnable();
	}

	private void UpdateTimeLeft()
	{
		if (deadlineUtcDate.Year <= 1970)
		{
			long milliseconds = Convert.ToInt64(_dailyTasksManager.timeMilliseconds);
			deadlineUtcDate = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).Date;
			deadlineUtcDate = deadlineUtcDate.AddDays(1.0);
		}
		TimeSpan timeSpan = deadlineUtcDate.Subtract(DateTime.UtcNow);
		int hours = timeSpan.Hours;
		long num = Convert.ToInt64(timeSpan.TotalMinutes);
		if (hours < 0)
		{
			timeProgress.fillAmount = 0f;
			timeLeftLabel.text = "";
			return;
		}
		string text = new LocalizedString("DailyTaskFullWindow.HoursleftHLeft");
		string text2 = new LocalizedString("DailyTaskFullWindow.MinutesleftMLeft");
		if (num > 60)
		{
			timeLeftLabel.text = text.Replace("{0}", hours.ToString());
		}
		else
		{
			timeLeftLabel.text = text2.Replace("{0}", num.ToString());
		}
		timeProgress.gameObject.SetActive(value: true);
		float val = Convert.ToSingle(timeSpan.TotalHours / 24.0);
		timeProgress.fillAmount = Math.Max(val, 0f);
	}

	private void GenerateTasks()
	{
		if (_dailyTasksManager != null && _dailyTasksManager.dailyTasks != null && _dailyTasksManager.dailyTasks.tasks != null && (bool)TaskPrefab)
		{
			DailyTask[] tasks = _dailyTasksManager.dailyTasks.tasks;
			foreach (DailyTask dailyTask in tasks)
			{
				_prefabFactory.Instantiate(TaskPrefab, TaskParent).Init(dailyTask).Forget();
			}
		}
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		BackClickCallback();
	}

	private void BackClickCallback()
	{
		if (!base.IsCloseCanceled)
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ShowMainMenuWindow();
				_managerUI.ShowUserBalanceWindow();
			}
			else
			{
				_managerUI.ShowPhoneWindowAsync().Forget();
			}
			base.Close();
		}
	}
}
