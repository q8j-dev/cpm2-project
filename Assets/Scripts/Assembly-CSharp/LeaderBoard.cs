using System;
using System.Collections.Generic;
using System.Threading;
using CPM.PlayerData;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;

public class LeaderBoard : MonoBehaviour
{
	private float timeLastRequest;

	public bool waitingNow;

	private int countFastRequest;

	private PlayerDatas.FriendsUtility friendsUtility;

	private PlayerDataStorage playerDataStorage;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(PlayerDatas.FriendsUtility friendsUtility, PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
		this.friendsUtility = friendsUtility;
		this.playerDataStorage = playerDataStorage;
	}

	private void Awake()
	{
		_freeDriveProvider.LeaderBoard = this;
	}

	private void OnDestroy()
	{
		_freeDriveProvider.LeaderBoard = null;
	}

	private void Start()
	{
	}

	public void requestBoard(string param)
	{
		if (!waitingNow)
		{
			GetListByCharAsync(param, this.GetCancellationTokenOnDestroy()).Forget();
		}
	}

	private async UniTask GetListByCharAsync(string param, CancellationToken ct = default(CancellationToken))
	{
		_ = 1;
		try
		{
			string accountId = ManagerKeysForPrefs.AccountId;
			int num = 100;
			string _json = "{ \"uuid\": \"" + accountId + "\", \"size\":" + num + "}";
			new LeaderBoardData
			{
				uuid = ManagerKeysForPrefs.AccountId,
				email = ManagerKeysForPrefs.Email,
				mileage = 500f,
				racingWin = 125f,
				taxiDoneCount = 1025f
			};
			if (Time.time - timeLastRequest < 2f)
			{
				countFastRequest++;
				if (countFastRequest > 3)
				{
					waitingNow = true;
					await UniTask.Delay(TimeSpan.FromSeconds(3.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
			}
			else
			{
				countFastRequest--;
			}
			countFastRequest = Mathf.Clamp(countFastRequest, 0, 5);
			waitingNow = false;
			timeLastRequest = Time.time;
			using UnityWebRequest www = UnityWebRequest.Put("https://5.188.131.29:8080/getusersbycat", _json);
			www.SetRequestHeader("Content-Type", "application/json");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
			{
				string text = www.downloadHandler.text;
				text = "{\"Items\":" + text + "}";
				LeaderBoardData[] boardData = JsonHelper.FromJson<LeaderBoardData>(text);
				_freeDriveProvider.LeaderWindow.OnleaderBoardReceived(boardData);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask UpdateRacing(int win, int raceType, float hp, float time, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			if (!ManagerKeysForPrefs.HasAccountId)
			{
				return;
			}
			LeaderBoardData leaderBoardData = new LeaderBoardData();
			leaderBoardData = setHPRangeTime(hp, raceType, time, leaderBoardData);
			leaderBoardData.uuid = ManagerKeysForPrefs.AccountId;
			leaderBoardData.racingWin = win;
			leaderBoardData.racingCount = 1f;
			string bodyData = JsonUtility.ToJson(leaderBoardData);
			using UnityWebRequest www = UnityWebRequest.Put("https://5.188.131.29:8080/updateuser/" + leaderBoardData.uuid, bodyData);
			www.SetRequestHeader("Content-Type", "application/json");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError && !www.isHttpError)
			{
				_ = www.downloadHandler.text;
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask UpdatePolice(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			if (!ManagerKeysForPrefs.HasAccountId)
			{
				return;
			}
			LeaderBoardData leaderBoardData = new LeaderBoardData();
			leaderBoardData.uuid = ManagerKeysForPrefs.AccountId;
			leaderBoardData.policeArrestCount = 1f;
			leaderBoardData.policeArrestEarnedMoney = 2000f;
			string bodyData = JsonUtility.ToJson(leaderBoardData);
			using UnityWebRequest www = UnityWebRequest.Put("https://5.188.131.29:8080/updateuser/" + leaderBoardData.uuid, bodyData);
			www.SetRequestHeader("Content-Type", "application/json");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError)
			{
				_ = www.isHttpError;
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask UpdateTaxi(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			if (!ManagerKeysForPrefs.HasAccountId)
			{
				return;
			}
			LeaderBoardData leaderBoardData = new LeaderBoardData();
			leaderBoardData.uuid = ManagerKeysForPrefs.AccountId;
			leaderBoardData.taxiDoneCount = 1f;
			string bodyData = JsonUtility.ToJson(leaderBoardData);
			using UnityWebRequest www = UnityWebRequest.Put("https://5.188.131.29:8080/updateuser/" + leaderBoardData.uuid, bodyData);
			www.SetRequestHeader("Content-Type", "application/json");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError)
			{
				_ = www.isHttpError;
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask UpdateMileage(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			if (!ManagerKeysForPrefs.HasAccountId)
			{
				return;
			}
			LeaderBoardData leaderBoardData = new LeaderBoardData();
			leaderBoardData.uuid = ManagerKeysForPrefs.AccountId;
			leaderBoardData.mileage = GetOverAllMileage();
			leaderBoardData.nickname = ObscuredPrefs.GetString("PlayerName");
			if (leaderBoardData.mileage - V.LastSendMileage < 10f)
			{
				return;
			}
			V.LastSendMileage = leaderBoardData.mileage;
			string bodyData = JsonUtility.ToJson(leaderBoardData);
			using UnityWebRequest www = UnityWebRequest.Put("https://5.188.131.29:8080/updateuser/" + leaderBoardData.uuid, bodyData);
			www.SetRequestHeader("Content-Type", "application/json");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError)
			{
				_ = www.isHttpError;
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private LeaderBoardData setHPRangeTime(float hpInput, int racingType, float time, LeaderBoardData leaderBoardData)
	{
		if (racingType == 0)
		{
			if (hpInput < 200f)
			{
				leaderBoardData.racingFHM_hp0to200 = time;
			}
			else if (hpInput >= 200f && hpInput < 300f)
			{
				leaderBoardData.racingFHM_hp200to300 = time;
			}
			else if (hpInput >= 300f && hpInput < 400f)
			{
				leaderBoardData.racingFHM_hp300to400 = time;
			}
			else if (hpInput >= 400f && hpInput < 500f)
			{
				leaderBoardData.racingFHM_hp400to500 = time;
			}
			else if (hpInput >= 500f && hpInput < 600f)
			{
				leaderBoardData.racingFHM_hp500to600 = time;
			}
			else if (hpInput >= 600f && hpInput < 700f)
			{
				leaderBoardData.racingFHM_hp600to700 = time;
			}
			else if (hpInput >= 700f && hpInput < 800f)
			{
				leaderBoardData.racingFHM_hp700to800 = time;
			}
			else if (hpInput >= 800f && hpInput < 900f)
			{
				leaderBoardData.racingFHM_hp800to900 = time;
			}
			else if (hpInput >= 900f && hpInput < 1000f)
			{
				leaderBoardData.racingFHM_hp900to1000 = time;
			}
			else if (hpInput >= 1000f)
			{
				leaderBoardData.racingFHM_hp1000plus = time;
			}
		}
		if (racingType == 1)
		{
			if (hpInput < 200f)
			{
				leaderBoardData.racingOTSH_hp0to200 = time;
			}
			else if (hpInput >= 200f && hpInput < 300f)
			{
				leaderBoardData.racingOTSH_hp200to300 = time;
			}
			else if (hpInput >= 300f && hpInput < 400f)
			{
				leaderBoardData.racingOTSH_hp300to400 = time;
			}
			else if (hpInput >= 400f && hpInput < 500f)
			{
				leaderBoardData.racingOTSH_hp400to500 = time;
			}
			else if (hpInput >= 500f && hpInput < 600f)
			{
				leaderBoardData.racingOTSH_hp500to600 = time;
			}
			else if (hpInput >= 600f && hpInput < 700f)
			{
				leaderBoardData.racingOTSH_hp600to700 = time;
			}
			else if (hpInput >= 700f && hpInput < 800f)
			{
				leaderBoardData.racingOTSH_hp700to800 = time;
			}
			else if (hpInput >= 800f && hpInput < 900f)
			{
				leaderBoardData.racingOTSH_hp800to900 = time;
			}
			else if (hpInput >= 900f && hpInput < 1000f)
			{
				leaderBoardData.racingOTSH_hp900to1000 = time;
			}
			else if (hpInput >= 1000f)
			{
				leaderBoardData.racingOTSH_hp1000plus = time;
			}
		}
		if (racingType == 2)
		{
			if (hpInput < 200f)
			{
				leaderBoardData.racingFRT_hp0to200 = time;
			}
			else if (hpInput >= 200f && hpInput < 300f)
			{
				leaderBoardData.racingFRT_hp200to300 = time;
			}
			else if (hpInput >= 300f && hpInput < 400f)
			{
				leaderBoardData.racingFRT_hp300to400 = time;
			}
			else if (hpInput >= 400f && hpInput < 500f)
			{
				leaderBoardData.racingFRT_hp400to500 = time;
			}
			else if (hpInput >= 500f && hpInput < 600f)
			{
				leaderBoardData.racingFRT_hp500to600 = time;
			}
			else if (hpInput >= 600f && hpInput < 700f)
			{
				leaderBoardData.racingFRT_hp600to700 = time;
			}
			else if (hpInput >= 700f && hpInput < 800f)
			{
				leaderBoardData.racingFRT_hp700to800 = time;
			}
			else if (hpInput >= 800f && hpInput < 900f)
			{
				leaderBoardData.racingFRT_hp800to900 = time;
			}
			else if (hpInput >= 900f && hpInput < 1000f)
			{
				leaderBoardData.racingFRT_hp900to1000 = time;
			}
			else if (hpInput >= 1000f)
			{
				leaderBoardData.racingFRT_hp1000plus = time;
			}
		}
		if (racingType == 3)
		{
			if (hpInput < 200f)
			{
				leaderBoardData.racingSRT_hp0to200 = time;
			}
			else if (hpInput >= 200f && hpInput < 300f)
			{
				leaderBoardData.racingSRT_hp200to300 = time;
			}
			else if (hpInput >= 300f && hpInput < 400f)
			{
				leaderBoardData.racingSRT_hp300to400 = time;
			}
			else if (hpInput >= 400f && hpInput < 500f)
			{
				leaderBoardData.racingSRT_hp400to500 = time;
			}
			else if (hpInput >= 500f && hpInput < 600f)
			{
				leaderBoardData.racingSRT_hp500to600 = time;
			}
			else if (hpInput >= 600f && hpInput < 700f)
			{
				leaderBoardData.racingSRT_hp600to700 = time;
			}
			else if (hpInput >= 700f && hpInput < 800f)
			{
				leaderBoardData.racingSRT_hp700to800 = time;
			}
			else if (hpInput >= 800f && hpInput < 900f)
			{
				leaderBoardData.racingSRT_hp800to900 = time;
			}
			else if (hpInput >= 900f && hpInput < 1000f)
			{
				leaderBoardData.racingSRT_hp900to1000 = time;
			}
			else if (hpInput >= 1000f)
			{
				leaderBoardData.racingSRT_hp1000plus = time;
			}
		}
		if (racingType == 4)
		{
			if (hpInput < 200f)
			{
				leaderBoardData.racing3KM_hp0to200 = time;
			}
			else if (hpInput >= 200f && hpInput < 300f)
			{
				leaderBoardData.racing3KM_hp200to300 = time;
			}
			else if (hpInput >= 300f && hpInput < 400f)
			{
				leaderBoardData.racing3KM_hp300to400 = time;
			}
			else if (hpInput >= 400f && hpInput < 500f)
			{
				leaderBoardData.racing3KM_hp400to500 = time;
			}
			else if (hpInput >= 500f && hpInput < 600f)
			{
				leaderBoardData.racing3KM_hp500to600 = time;
			}
			else if (hpInput >= 600f && hpInput < 700f)
			{
				leaderBoardData.racing3KM_hp600to700 = time;
			}
			else if (hpInput >= 700f && hpInput < 800f)
			{
				leaderBoardData.racing3KM_hp700to800 = time;
			}
			else if (hpInput >= 800f && hpInput < 900f)
			{
				leaderBoardData.racing3KM_hp800to900 = time;
			}
			else if (hpInput >= 900f && hpInput < 1000f)
			{
				leaderBoardData.racing3KM_hp900to1000 = time;
			}
			else if (hpInput >= 1000f)
			{
				leaderBoardData.racing3KM_hp1000plus = time;
			}
		}
		return leaderBoardData;
	}

	private float GetOverAllMileage()
	{
		float num = 0f;
		for (int i = 0; i < 200; i++)
		{
			num += ObscuredPrefs.GetFloat("mileage" + i);
		}
		return num;
	}

	public async UniTask CreateUser(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			LeaderBoardData_CreateUser leaderBoardData_CreateUser = new LeaderBoardData_CreateUser();
			leaderBoardData_CreateUser.uuid = ManagerKeysForPrefs.AccountId;
			leaderBoardData_CreateUser.email = ManagerKeysForPrefs.Email;
			leaderBoardData_CreateUser.mileage = GetOverAllMileage();
			leaderBoardData_CreateUser.nickname = ObscuredPrefs.GetString("PlayerName");
			leaderBoardData_CreateUser.personIndex = ObscuredPrefs.GetInt("SelectedPerson");
			leaderBoardData_CreateUser.innerid = playerDataStorage.PlayerID;
			string bodyData = JsonUtility.ToJson(leaderBoardData_CreateUser);
			using UnityWebRequest www = UnityWebRequest.Put("https://5.188.131.29:8080/createuser/" + leaderBoardData_CreateUser.uuid, bodyData);
			www.SetRequestHeader("Content-Type", "application/json");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError && !www.isHttpError)
			{
				_ = www.downloadHandler.text;
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void requestUser(string _playerID)
	{
		GetUserAsync(_playerID, this.GetCancellationTokenOnDestroy()).Forget();
	}

	private void UserNotFound()
	{
		CreateUser(this.GetCancellationTokenOnDestroy()).Forget();
	}

	public async UniTask CheckUser(string _playerID, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			using UnityWebRequest www = UnityWebRequest.Get("https://5.188.131.29:8080/users/" + _playerID);
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (www.isNetworkError || www.isHttpError)
			{
				UserNotFound();
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTask GetUserAsync(string _playerID, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			new List<IMultipartFormSection>();
			using UnityWebRequest www = UnityWebRequest.Get("https://5.188.131.29:8080/users/" + _playerID);
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError && !www.isHttpError)
			{
				LeaderBoardData leaderBoardData = JsonUtility.FromJson<LeaderBoardData>(www.downloadHandler.text);
				_freeDriveProvider.LeaderWindow.onOneUserReceived(leaderBoardData);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask GetOwnUser(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			new List<IMultipartFormSection>();
			string accountId = ManagerKeysForPrefs.AccountId;
			using UnityWebRequest www = UnityWebRequest.Get("https://5.188.131.29:8080/users/" + accountId);
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError)
			{
				_ = www.isHttpError;
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTask GetUsersListAsync(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			using UnityWebRequest www = UnityWebRequest.Get("https://5.188.131.29:8080/users?limit=150&offset=1");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError && !www.isHttpError)
			{
				string text = www.downloadHandler.text;
				text = "{\"Items\":" + text + "}";
				JsonHelper.FromJson<LeaderBoardData>(text);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask GetUsersListByUUID(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			string[] array = new string[friendsUtility.Friends.Count];
			int num = 0;
			foreach (PrefFriendData friend in friendsUtility.Friends)
			{
				array[num] = friend.accountID ?? "";
				num++;
			}
			string text = JsonHelper.ToJson(array);
			text = text.Remove(0, 9);
			text = text.Remove(text.Length - 1);
			using UnityWebRequest www = UnityWebRequest.Put("https://5.188.131.29:8080/getusersbyuuid", text);
			www.SetRequestHeader("Content-Type", "application/json");
			if (!(await TrySend(www, ct)))
			{
				return;
			}
			if (!www.isNetworkError && !www.isHttpError)
			{
				string text2 = www.downloadHandler.text;
				text2 = "{\"Items\":" + text2 + "}";
				LeaderBoardData[] boardData = JsonHelper.FromJson<LeaderBoardData>(text2);
				_freeDriveProvider.LeaderWindow.OnleaderBoardReceived(boardData);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private static async UniTask<bool> TrySend(UnityWebRequest www, CancellationToken ct)
	{
		try
		{
			await www.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, ct);
			return www.result == UnityWebRequest.Result.Success;
		}
		catch (OperationCanceledException)
		{
			return false;
		}
		catch (UnityWebRequestException)
		{
			return false;
		}
		catch (InvalidOperationException)
		{
			return false;
		}
	}
}
