using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;

public class LeaderWindow : MonoBehaviour
{
	public GameObject prefabList;

	public Transform parentList;

	public Transform parentScrollViewLeaderBoard;

	public GameObject loadingSign;

	public int currentBoarad;

	public int currentRacingType;

	public int currentRacingHPRange;

	public LeaderBoard leaderBoard;

	public Color FirstPlaceColor;

	public OnePlayerWindow oneUserWindow;

	public GameObject mainBackground;

	public GameObject allButonsParent;

	public GameObject accountRequiredWindow;

	private LeaderBoardData[] friendsLBD;

	public bool showFriendsLeaderboard;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
	}

	private void Awake()
	{
		_freeDriveProvider.LeaderWindow = this;
		oneUserWindow.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		_freeDriveProvider.LeaderWindow = null;
	}

	private void sendOwnData()
	{
		if (ObscuredPrefs.GetInt("sendOwnDataDay") != V.PassedDay)
		{
			if (ManagerKeysForPrefs.HasAccountId)
			{
				leaderBoard.UpdateMileage(this.GetCancellationTokenOnDestroy()).Forget();
			}
			ObscuredPrefs.SetInt("sendOwnDataDay", V.PassedDay);
		}
	}

	public void OnLeaderBoardOpen()
	{
		if (ManagerKeysForPrefs.HasEmail)
		{
			leaderBoard.UpdateMileage(this.GetCancellationTokenOnDestroy()).Forget();
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				afterRequest();
			}).AddTo(this);
			accountRequiredWindow.SetActive(value: false);
			loadingSign.SetActive(value: true);
			parentList.gameObject.SetActive(value: false);
			allButonsParent.SetActive(value: true);
		}
		else
		{
			accountRequiredWindow.SetActive(value: true);
			parentScrollViewLeaderBoard.gameObject.SetActive(value: false);
			loadingSign.gameObject.SetActive(value: false);
			allButonsParent.SetActive(value: false);
		}
	}

	private void afterRequest()
	{
		leaderBoard.GetOwnUser(this.GetCancellationTokenOnDestroy()).Forget();
		if (PlayerPrefs.GetInt("leaderboardType", 0) == 0)
		{
			ShowGlobalLB();
		}
		else
		{
			ShowFriendLB();
		}
	}

	public void ChangeBoard(GameObject parent)
	{
		if (!leaderBoard.waitingNow)
		{
			int nextBoard = int.Parse(parent.transform.name);
			ChangeBoard2(nextBoard);
		}
	}

	public void ShowFriendLB()
	{
		showFriendsLeaderboard = true;
		ChangeBoard2(currentBoarad);
		PlayerPrefs.GetInt("leaderboardType", 1);
	}

	public void ShowGlobalLB()
	{
		showFriendsLeaderboard = false;
		ChangeBoard2(currentBoarad);
		PlayerPrefs.GetInt("leaderboardType", 0);
	}

	public void ChangeBoard2(int nextBoard)
	{
		currentBoarad = nextBoard;
		if (!showFriendsLeaderboard)
		{
			if (nextBoard == 0)
			{
				leaderBoard.requestBoard("mileage");
			}
			if (nextBoard == 1)
			{
				if (currentRacingType == 0)
				{
					leaderBoard.requestBoard("racingWin");
				}
				else
				{
					leaderBoard.requestBoard(getRaceParameter());
				}
			}
			if (nextBoard == 2)
			{
				leaderBoard.requestBoard("taxiDoneCount");
			}
			if (nextBoard == 3)
			{
				leaderBoard.requestBoard("policeArrestCount");
			}
		}
		else
		{
			leaderBoard.GetUsersListByUUID(this.GetCancellationTokenOnDestroy()).Forget();
		}
		loadingSign.SetActive(value: true);
		parentList.gameObject.SetActive(value: false);
	}

	public void ChangeBoardAllRacing(GameObject parent)
	{
		if (!leaderBoard.waitingNow)
		{
			int nextBoard = int.Parse(parent.transform.name);
			ChangeBoardAllRacing2(nextBoard);
		}
	}

	public void ChangeBoardAllRacing2(int nextBoard)
	{
		currentRacingType = nextBoard;
		leaderBoard.requestBoard(getRaceParameter());
		loadingSign.SetActive(value: true);
		parentList.gameObject.SetActive(value: false);
	}

	public void OnRacingHpRangeChanged(GameObject parent)
	{
		if (!leaderBoard.waitingNow)
		{
			currentRacingHPRange = int.Parse(parent.transform.name);
			ChangeBoardAllRacing2(currentRacingType);
		}
	}

	private string getRaceParameter()
	{
		if (currentRacingType == 0)
		{
			return "racingWin";
		}
		if (currentRacingType == 1)
		{
			return currentRacingHPRange switch
			{
				0 => "racingFHM_hp0to200", 
				1 => "racingFHM_hp200to300", 
				2 => "racingFHM_hp300to400", 
				3 => "racingFHM_hp400to500", 
				4 => "racingFHM_hp500to600", 
				5 => "racingFHM_hp600to700", 
				6 => "racingFHM_hp700to800", 
				7 => "racingFHM_hp800to900", 
				8 => "racingFHM_hp900to1000", 
				9 => "racingFHM_hp1000plus", 
				_ => "racingFHM_hp0to200", 
			};
		}
		if (currentRacingType == 2)
		{
			return currentRacingHPRange switch
			{
				0 => "racingOTSH_hp0to200", 
				1 => "racingOTSH_hp200to300", 
				2 => "racingOTSH_hp300to400", 
				3 => "racingOTSH_hp400to500", 
				4 => "racingOTSH_hp500to600", 
				5 => "racingOTSH_hp600to700", 
				6 => "racingOTSH_hp700to800", 
				7 => "racingOTSH_hp800to900", 
				8 => "racingOTSH_hp900to1000", 
				9 => "racingOTSH_hp1000plus", 
				_ => "racingOTSH_hp0to200", 
			};
		}
		if (currentRacingType == 3)
		{
			return currentRacingHPRange switch
			{
				0 => "racingFRT_hp0to200", 
				1 => "racingFRT_hp200to300", 
				2 => "racingFRT_hp300to400", 
				3 => "racingFRT_hp400to500", 
				4 => "racingFRT_hp500to600", 
				5 => "racingFRT_hp600to700", 
				6 => "racingFRT_hp700to800", 
				7 => "racingFRT_hp800to900", 
				8 => "racingFRT_hp900to1000", 
				9 => "racingFRT_hp1000plus", 
				_ => "racingFRT_hp0to200", 
			};
		}
		if (currentRacingType == 4)
		{
			return currentRacingHPRange switch
			{
				0 => "racingSRT_hp0to200", 
				1 => "racingSRT_hp200to300", 
				2 => "racingSRT_hp300to400", 
				3 => "racingSRT_hp400to500", 
				4 => "racingSRT_hp500to600", 
				5 => "racingSRT_hp600to700", 
				6 => "racingSRT_hp700to800", 
				7 => "racingSRT_hp800to900", 
				8 => "racingSRT_hp900to1000", 
				9 => "racingSRT_hp1000plus", 
				_ => "racingSRT_hp0to200", 
			};
		}
		if (currentRacingType == 5)
		{
			return currentRacingHPRange switch
			{
				0 => "racing3KM_hp0to200", 
				1 => "racing3KM_hp200to300", 
				2 => "racing3KM_hp300to400", 
				3 => "racing3KM_hp400to500", 
				4 => "racing3KM_hp500to600", 
				5 => "racing3KM_hp600to700", 
				6 => "racing3KM_hp700to800", 
				7 => "racing3KM_hp800to900", 
				8 => "racing3KM_hp900to1000", 
				9 => "racing3KM_hp1000plus", 
				_ => "racing3KM_hp0to200", 
			};
		}
		return "";
	}

	private float GetParameterToShow(LeaderBoardData leaderBoardData)
	{
		return currentRacingType switch
		{
			1 => currentRacingHPRange switch
			{
				0 => leaderBoardData.racingFHM_hp0to200, 
				1 => leaderBoardData.racingFHM_hp200to300, 
				2 => leaderBoardData.racingFHM_hp300to400, 
				3 => leaderBoardData.racingFHM_hp400to500, 
				4 => leaderBoardData.racingFHM_hp500to600, 
				5 => leaderBoardData.racingFHM_hp600to700, 
				6 => leaderBoardData.racingFHM_hp700to800, 
				7 => leaderBoardData.racingFHM_hp800to900, 
				8 => leaderBoardData.racingFHM_hp900to1000, 
				9 => leaderBoardData.racingFHM_hp1000plus, 
				_ => leaderBoardData.racingFHM_hp0to200, 
			}, 
			2 => currentRacingHPRange switch
			{
				0 => leaderBoardData.racingOTSH_hp0to200, 
				1 => leaderBoardData.racingOTSH_hp200to300, 
				2 => leaderBoardData.racingOTSH_hp300to400, 
				3 => leaderBoardData.racingOTSH_hp400to500, 
				4 => leaderBoardData.racingOTSH_hp500to600, 
				5 => leaderBoardData.racingOTSH_hp600to700, 
				6 => leaderBoardData.racingOTSH_hp700to800, 
				7 => leaderBoardData.racingOTSH_hp800to900, 
				8 => leaderBoardData.racingOTSH_hp900to1000, 
				9 => leaderBoardData.racingOTSH_hp1000plus, 
				_ => leaderBoardData.racingOTSH_hp0to200, 
			}, 
			3 => currentRacingHPRange switch
			{
				0 => leaderBoardData.racingFRT_hp0to200, 
				1 => leaderBoardData.racingFRT_hp200to300, 
				2 => leaderBoardData.racingFRT_hp300to400, 
				3 => leaderBoardData.racingFRT_hp400to500, 
				4 => leaderBoardData.racingFRT_hp500to600, 
				5 => leaderBoardData.racingFRT_hp600to700, 
				6 => leaderBoardData.racingFRT_hp700to800, 
				7 => leaderBoardData.racingFRT_hp800to900, 
				8 => leaderBoardData.racingFRT_hp900to1000, 
				9 => leaderBoardData.racingFRT_hp1000plus, 
				_ => leaderBoardData.racingFRT_hp0to200, 
			}, 
			4 => currentRacingHPRange switch
			{
				0 => leaderBoardData.racingSRT_hp0to200, 
				1 => leaderBoardData.racingSRT_hp200to300, 
				2 => leaderBoardData.racingSRT_hp300to400, 
				3 => leaderBoardData.racingSRT_hp400to500, 
				4 => leaderBoardData.racingSRT_hp500to600, 
				5 => leaderBoardData.racingSRT_hp600to700, 
				6 => leaderBoardData.racingSRT_hp700to800, 
				7 => leaderBoardData.racingSRT_hp800to900, 
				8 => leaderBoardData.racingSRT_hp900to1000, 
				9 => leaderBoardData.racingSRT_hp1000plus, 
				_ => leaderBoardData.racingSRT_hp0to200, 
			}, 
			5 => currentRacingHPRange switch
			{
				0 => leaderBoardData.racing3KM_hp0to200, 
				1 => leaderBoardData.racing3KM_hp200to300, 
				2 => leaderBoardData.racing3KM_hp300to400, 
				3 => leaderBoardData.racing3KM_hp400to500, 
				4 => leaderBoardData.racing3KM_hp500to600, 
				5 => leaderBoardData.racing3KM_hp600to700, 
				6 => leaderBoardData.racing3KM_hp700to800, 
				7 => leaderBoardData.racing3KM_hp800to900, 
				8 => leaderBoardData.racing3KM_hp900to1000, 
				9 => leaderBoardData.racing3KM_hp1000plus, 
				_ => leaderBoardData.racing3KM_hp0to200, 
			}, 
			_ => 0f, 
		};
	}

	public void OnleaderBoardReceived(LeaderBoardData[] boardData)
	{
		if (leaderBoard == null)
		{
			return;
		}
		loadList(boardData);
		closeOneUserWindow();
		if (currentBoarad == 1)
		{
			if (currentRacingType == 0)
			{
				parentScrollViewLeaderBoard.transform.localPosition = new Vector3(0f, 139.6f, 0f);
				parentScrollViewLeaderBoard.transform.localScale = new Vector3(1f, 0.93f, 1f);
			}
			else
			{
				parentScrollViewLeaderBoard.transform.localPosition = new Vector3(0f, 81.74f, 0f);
				parentScrollViewLeaderBoard.transform.localScale = new Vector3(1f, 0.83f, 1f);
			}
		}
		else
		{
			parentScrollViewLeaderBoard.transform.localPosition = new Vector3(0f, 200f, 0f);
			parentScrollViewLeaderBoard.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private List<LeaderBoardData> sortLeaderBoardList(List<LeaderBoardData> toSort)
	{
		switch (currentBoarad)
		{
		case 0:
			toSort = toSort.OrderByDescending((LeaderBoardData o) => o.mileage).ToList();
			break;
		case 1:
			if (currentRacingType == 0)
			{
				toSort = toSort.OrderByDescending((LeaderBoardData o) => o.racingWin).ToList();
			}
			if (currentRacingType == 1)
			{
				if (currentRacingHPRange == 0)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp0to200).ToList();
				}
				if (currentRacingHPRange == 1)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp200to300).ToList();
				}
				if (currentRacingHPRange == 2)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp300to400).ToList();
				}
				if (currentRacingHPRange == 3)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp400to500).ToList();
				}
				if (currentRacingHPRange == 4)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp500to600).ToList();
				}
				if (currentRacingHPRange == 5)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp600to700).ToList();
				}
				if (currentRacingHPRange == 6)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp700to800).ToList();
				}
				if (currentRacingHPRange == 7)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp800to900).ToList();
				}
				if (currentRacingHPRange == 8)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp900to1000).ToList();
				}
				if (currentRacingHPRange == 9)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFHM_hp1000plus).ToList();
				}
			}
			if (currentRacingType == 2)
			{
				if (currentRacingHPRange == 0)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp0to200).ToList();
				}
				if (currentRacingHPRange == 1)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp200to300).ToList();
				}
				if (currentRacingHPRange == 2)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp300to400).ToList();
				}
				if (currentRacingHPRange == 3)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp400to500).ToList();
				}
				if (currentRacingHPRange == 4)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp500to600).ToList();
				}
				if (currentRacingHPRange == 5)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp600to700).ToList();
				}
				if (currentRacingHPRange == 6)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp700to800).ToList();
				}
				if (currentRacingHPRange == 7)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp800to900).ToList();
				}
				if (currentRacingHPRange == 8)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp900to1000).ToList();
				}
				if (currentRacingHPRange == 9)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingOTSH_hp1000plus).ToList();
				}
			}
			if (currentRacingType == 3)
			{
				if (currentRacingHPRange == 0)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp0to200).ToList();
				}
				if (currentRacingHPRange == 1)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp200to300).ToList();
				}
				if (currentRacingHPRange == 2)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp300to400).ToList();
				}
				if (currentRacingHPRange == 3)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp400to500).ToList();
				}
				if (currentRacingHPRange == 4)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp500to600).ToList();
				}
				if (currentRacingHPRange == 5)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp600to700).ToList();
				}
				if (currentRacingHPRange == 6)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp700to800).ToList();
				}
				if (currentRacingHPRange == 7)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp800to900).ToList();
				}
				if (currentRacingHPRange == 8)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp900to1000).ToList();
				}
				if (currentRacingHPRange == 9)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingFRT_hp1000plus).ToList();
				}
			}
			if (currentRacingType == 4)
			{
				if (currentRacingHPRange == 0)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp0to200).ToList();
				}
				if (currentRacingHPRange == 1)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp200to300).ToList();
				}
				if (currentRacingHPRange == 2)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp300to400).ToList();
				}
				if (currentRacingHPRange == 3)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp400to500).ToList();
				}
				if (currentRacingHPRange == 4)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp500to600).ToList();
				}
				if (currentRacingHPRange == 5)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp600to700).ToList();
				}
				if (currentRacingHPRange == 6)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp700to800).ToList();
				}
				if (currentRacingHPRange == 7)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp800to900).ToList();
				}
				if (currentRacingHPRange == 8)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp900to1000).ToList();
				}
				if (currentRacingHPRange == 9)
				{
					toSort = toSort.OrderBy((LeaderBoardData o) => o.racingSRT_hp1000plus).ToList();
				}
			}
			if (currentRacingType != 5)
			{
				break;
			}
			if (currentRacingHPRange == 0)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp0to200).ToList();
			}
			if (currentRacingHPRange == 1)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp200to300).ToList();
			}
			if (currentRacingHPRange == 2)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp300to400).ToList();
			}
			if (currentRacingHPRange == 3)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp400to500).ToList();
			}
			if (currentRacingHPRange == 4)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp500to600).ToList();
			}
			if (currentRacingHPRange == 5)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp600to700).ToList();
			}
			if (currentRacingHPRange == 6)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp700to800).ToList();
			}
			if (currentRacingHPRange == 7)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp800to900).ToList();
			}
			if (currentRacingHPRange == 8)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp900to1000).ToList();
			}
			if (currentRacingHPRange == 9)
			{
				toSort = toSort.OrderBy((LeaderBoardData o) => o.racing3KM_hp1000plus).ToList();
			}
			break;
		case 2:
			toSort = toSort.OrderByDescending((LeaderBoardData o) => o.taxiDoneCount).ToList();
			break;
		case 3:
			toSort = toSort.OrderByDescending((LeaderBoardData o) => o.policeArrestCount).ToList();
			break;
		}
		return toSort;
	}

	public void loadList(LeaderBoardData[] leaderBoardDatas)
	{
		foreach (Transform parent in parentList)
		{
			UnityEngine.Object.Destroy(parent.gameObject);
		}
		List<LeaderBoardData> toSort = leaderBoardDatas.ToList();
		toSort = sortLeaderBoardList(toSort);
		_ = ManagerKeysForPrefs.AccountId;
		loadingSign.SetActive(value: false);
		parentList.gameObject.SetActive(value: true);
	}

	public void onOneUserRequested()
	{
		parentScrollViewLeaderBoard.gameObject.SetActive(value: false);
		oneUserWindow.gameObject.SetActive(value: false);
		loadingSign.gameObject.SetActive(value: true);
	}

	public void onOneUserReceived(LeaderBoardData leaderBoardData)
	{
		parentScrollViewLeaderBoard.gameObject.SetActive(value: false);
		oneUserWindow.gameObject.SetActive(value: true);
		loadingSign.gameObject.SetActive(value: false);
		allButonsParent.gameObject.SetActive(value: false);
		mainBackground.gameObject.SetActive(value: false);
		oneUserWindow.SetAllLabel(leaderBoardData);
	}

	public void closeOneUserWindow()
	{
		parentScrollViewLeaderBoard.gameObject.SetActive(value: true);
		oneUserWindow.gameObject.SetActive(value: false);
		allButonsParent.gameObject.SetActive(value: true);
		mainBackground.gameObject.SetActive(value: true);
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
}
