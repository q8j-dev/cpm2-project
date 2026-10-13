using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CPM.PlayerData;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using _Car_Parking;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Racing
{
	public class TimeBannerController : MonoBehaviourPunCallbacks
	{
		private TimeAttackContentData _timeAttackContentData = new TimeAttackContentData();

		public bool IsSortLargest;

		public const char DetermineSymbol = ';';

		private Dictionary<string, float> _cachedDataDictionary = new Dictionary<string, float>();

		public string TKey;

		public int BannerIndex;

		public TimeAttackMode TimeAttackMode;

		public V.placeToSpawn PlaceToSpawn;

		private PlayerDataStorage _playerDataStorage;

		private List<BannerData> _bannerContents = new List<BannerData>();

		private MultiPersonControl _cachedUserMultiPersonControl;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private BannerData _loseUserData;

		private bool _isDestroyed;

		private bool _isFirstChecked;

		private IDisposable _subscription;

		private List<BannerContent> _horizontalBannerContents = new List<BannerContent>();

		private bool CanChangeTimerState
		{
			get
			{
				if (PhotonNetwork.IsMasterClient)
				{
					return PhotonNetwork.CurrentRoom.PlayerCount >= 2;
				}
				return false;
			}
		}

		public void SetReferences(PlayerDataStorage playerDataStorage, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider)
		{
			_playerDataStorage = playerDataStorage;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_freeDriveProvider = freeDriveProvider;
		}

		private async UniTaskVoid FirstCheckBannerAsync()
		{
			if (_isDestroyed || !FlagsStorage.IsMultiFree || !PhotonNetwork.InRoom || _isFirstChecked)
			{
				return;
			}
			CancellationToken ct = this.GetCancellationTokenOnDestroy();
			while (!HasAnyPlayerRegistered())
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(500.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				if (_isDestroyed || !PhotonNetwork.InRoom)
				{
					return;
				}
			}
			if (!_isFirstChecked)
			{
				CheckBanners();
				_isFirstChecked = true;
			}
		}

		private bool HasAnyPlayerRegistered()
		{
			MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
			for (int i = 0; i < playersOnlineNow.Length; i++)
			{
				if ((bool)playersOnlineNow[i])
				{
					return true;
				}
			}
			return false;
		}

		private void CheckBanners()
		{
			if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TKey, out var value) && value != null)
			{
				string data = value.ToString();
				UpdateTable(data).Forget();
			}
		}

		public async UniTaskVoid EnableBannersAsync()
		{
			InitHorizontalBanner("TimeAttack!", BannerContentType.TimeAttack);
			await UniTask.Delay(TimeSpan.FromSeconds(3.0));
			if (!_isDestroyed)
			{
				UpdateContentData(BannerDataType.HorizontalBanner);
			}
		}

		public void InitHorizontalBanner(string text, BannerContentType contentType)
		{
			AddContent(text, contentType);
			_timeAttackContentData.HorizontalBannerTexts = _horizontalBannerContents;
			UpdateContentData(BannerDataType.HorizontalBanner);
		}

		private void AddContent(string newContentText, BannerContentType contentType)
		{
			BannerContent bannerContent = _horizontalBannerContents.FirstOrDefault((BannerContent c) => c.BannerContentType == contentType);
			if (bannerContent != null)
			{
				bannerContent.text = newContentText;
				return;
			}
			_horizontalBannerContents.Add(new BannerContent
			{
				text = newContentText,
				BannerContentType = contentType
			});
		}

		public void UpdateContentData(BannerDataType target)
		{
			_timeAttackContentData.BannerDataType = target;
			GlobalEventSystem.UpdateTimeAttackContentData(_timeAttackContentData);
		}

		private void Start()
		{
			_timeAttackContentData.Index = BannerIndex;
			if (FlagsStorage.IsMultiFree)
			{
				PersonMultiplayerDataSending.OnPersonDataRemoved += RemoveOnePlayer;
			}
			_subscription = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				FirstCheckBannerAsync().Forget();
			}).AddTo(this);
		}

		public string GetPersonFormattedResult(string userName, TimeAttackMode mode)
		{
			if (!_cachedDataDictionary.ContainsKey(userName))
			{
				return null;
			}
			return mode switch
			{
				TimeAttackMode.Drift => Utils.GetFormattedResultWithSpaces(_cachedDataDictionary[userName]), 
				TimeAttackMode.TimeAttack => Utils.FormatTime(_cachedDataDictionary[userName]), 
				_ => null, 
			};
		}

		private void OnDestroy()
		{
			_subscription?.Dispose();
			_isDestroyed = true;
			PersonMultiplayerDataSending.OnPersonDataRemoved -= RemoveOnePlayer;
		}

		private void RemoveOnePlayer(MultiPersonControl multiPersonControl)
		{
			if (PhotonNetwork.InRoom && _cachedDataDictionary.ContainsKey(multiPersonControl.ThisPlayerID))
			{
				_cachedDataDictionary.Remove(multiPersonControl.ThisPlayerID);
				SetRoomProperties();
			}
		}

		private void SetRoomProperties()
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { 
			{
				TKey,
				GetConvertedData()
			} });
		}

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (CanChangeTimerState)
			{
				SetRoomProperties();
			}
		}

		public override void OnJoinedRoom()
		{
			FirstCheckBannerAsync().Forget();
		}

		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			if (!string.IsNullOrEmpty(TKey) && propertiesThatChanged.TryGetValue(TKey, out var value) && value != null)
			{
				string data = value.ToString();
				UpdateTable(data).Forget();
			}
		}

		private string PrepareUserSendData(string userId, float time)
		{
			_cachedDataDictionary[userId] = time;
			return GetConvertedData();
		}

		private string GetConvertedData()
		{
			string text = "";
			foreach (KeyValuePair<string, float> item in _cachedDataDictionary)
			{
				text += $"{item.Key},{item.Value}{';'}";
			}
			return text;
		}

		private void CacheTableData(string[] data)
		{
			_cachedDataDictionary.Clear();
			foreach (string text in data)
			{
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				int num = text.IndexOf(',');
				if (num > 0 && num < text.Length - 1)
				{
					string text2 = text.Substring(0, num);
					if (!string.IsNullOrEmpty(text2) && float.TryParse(text.Substring(num + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out var result) && !_cachedDataDictionary.ContainsKey(text2) && IsUserExists(text2))
					{
						_cachedDataDictionary.Add(text2, result);
					}
				}
			}
		}

		private bool IsUserExists(string userId)
		{
			MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
			foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
			{
				if ((bool)multiPersonControl && multiPersonControl.ThisPlayerID == userId)
				{
					return true;
				}
			}
			if (_playerDataStorage.PlayerID == (ObscuredString)userId)
			{
				return true;
			}
			return false;
		}

		public void OnFinishUpdateTable(string userId, float time, float bestTime)
		{
			string value = PrepareUserSendData(userId, time);
			if (!(bestTime > 0f) || !(bestTime < time))
			{
				if (FlagsStorage.IsMultiFree)
				{
					PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { TKey, value } });
				}
				else
				{
					UpdateUiBanners();
				}
			}
		}

		public async UniTaskVoid UpdateTable(string data)
		{
			string[] data2 = data.Split(';');
			CacheTableData(data2);
			UpdateUiBanners();
		}

		private void UpdateUiBanners()
		{
			_loseUserData = null;
			_bannerContents.Clear();
			_bannerContents = UpdateTableData(_cachedDataDictionary);
			_timeAttackContentData.LeaderBoard = _bannerContents;
			_timeAttackContentData.LoseUser = _loseUserData;
			UpdateContentData(BannerDataType.LeaderBoard);
			if (_bannerContents.Count > 0)
			{
				BannerData bannerData = _bannerContents.First();
				string text = (string.IsNullOrEmpty(bannerData.UserName) ? bannerData.UserId : bannerData.UserName);
				string text2 = new LocalizedString("TimeAttack.RoomRecord");
				InitHorizontalBanner(text2 + " : " + text, BannerContentType.Mvp);
			}
			else
			{
				_horizontalBannerContents.Clear();
				InitHorizontalBanner("TimeAttack!", BannerContentType.TimeAttack);
			}
		}

		public void UpdateSpecificBanner(UITimeBannerController uiBannerController)
		{
			_bannerContents.Clear();
			if (_bannerContents != null && _bannerContents.Count == 0)
			{
				_bannerContents = UpdateTableData(_cachedDataDictionary);
			}
			uiBannerController.FillTable(_bannerContents, _loseUserData);
		}

		private List<BannerData> UpdateTableData(Dictionary<string, float> data)
		{
			List<BannerData> list = new List<BannerData>(data.Count);
			foreach (KeyValuePair<string, float> datum in data)
			{
				list.Add(new BannerData
				{
					UserId = datum.Key,
					PointValue = datum.Value,
					TimeAttackMode = TimeAttackMode
				});
			}
			if (!IsSortLargest)
			{
				list.Sort((BannerData a, BannerData b) => a.PointValue.CompareTo(b.PointValue));
			}
			else
			{
				list.Sort((BannerData a, BannerData b) => b.PointValue.CompareTo(a.PointValue));
			}
			if (FlagsStorage.IsMultiFree)
			{
				FillMultiPlayerTable(list);
			}
			else
			{
				FillSinglePlayerTable(list);
			}
			return list;
		}

		private void FillSinglePlayerTable(List<BannerData> newData)
		{
			if (newData.Count >= 1)
			{
				PrepareSingleBannerData(newData.First());
			}
		}

		private void FillMultiPlayerTable(List<BannerData> newData)
		{
			AddUserToMultiplayerList();
			Dictionary<string, MultiPersonControl> dictionary = new Dictionary<string, MultiPersonControl>();
			if ((bool)_cachedUserMultiPersonControl)
			{
				dictionary.Add(_playerDataStorage.PlayerID, _cachedUserMultiPersonControl);
			}
			MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
			foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
			{
				if ((bool)multiPersonControl)
				{
					dictionary.TryAdd(multiPersonControl.ThisPlayerID, multiPersonControl);
				}
			}
			int num = Math.Min(newData.Count, 4);
			bool flag = false;
			for (int j = 0; j < num; j++)
			{
				BannerData bannerData = newData[j];
				if (dictionary.TryGetValue(bannerData.UserId, out var value))
				{
					bannerData.Place = j;
					if (value.ownPlayer)
					{
						flag = true;
					}
					bannerData.IsUser = flag;
					PrepareBannerElement(bannerData, value);
				}
			}
			if (flag)
			{
				return;
			}
			int num2 = newData.FindIndex((BannerData x) => (ObscuredString)x.UserId == _playerDataStorage.PlayerID);
			if (num2 != -1)
			{
				newData[num2].Place = num2;
				if ((bool)_cachedUserMultiPersonControl)
				{
					PrepareBannerElement(newData[num2], _cachedUserMultiPersonControl);
				}
				_loseUserData = newData[num2];
			}
		}

		private void PrepareSingleBannerData(BannerData bannerData)
		{
			VehicleData component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			bannerData.CarId = component.CarId;
			bannerData.ClassId = Utils.GetClassVehicle(component.GetVehicleEntityMono.CarEntity.PerformancePoints, component.CarId);
			bannerData.Rank = GetPlayerRank();
			bannerData.IsUser = true;
			bannerData.UserName = _playerDataStorage.PlayerName;
		}

		private void PrepareBannerElement(BannerData bannerData, MultiPersonControl multiPersonControl)
		{
			if (!multiPersonControl || bannerData == null)
			{
				return;
			}
			VehicleData vehicleData = null;
			vehicleData = ((!multiPersonControl.ownPlayer) ? (((bool)multiPersonControl && (bool)multiPersonControl.FirstAndOwnCar) ? multiPersonControl.FirstAndOwnCar.GetComponent<VehicleData>() : null) : ((_freeDriveProvider != null && (bool)_freeDriveProvider.FirstCarMultiPlayer) ? _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>() : null));
			if ((bool)vehicleData)
			{
				bannerData.CarId = vehicleData.CarId;
				bannerData.ClassId = Utils.GetClassVehicle(vehicleData.GetVehicleEntityMono.CarEntity.PerformancePoints, vehicleData.CarId);
				bannerData.Rank = (multiPersonControl.ownPlayer ? new ObscuredInt?(GetPlayerRank()) : multiPersonControl.rank);
				bannerData.IsUser = multiPersonControl.ownPlayer;
				bannerData.UserName = (multiPersonControl.ownPlayer ? _playerDataStorage.PlayerName : multiPersonControl.ThisPlayerName);
				if (string.IsNullOrEmpty(bannerData.UserName))
				{
					bannerData.UserName = bannerData.UserId;
				}
			}
		}

		private void AddUserToMultiplayerList()
		{
			if ((bool)V.DriverPerson && !_cachedUserMultiPersonControl)
			{
				_cachedUserMultiPersonControl = V.DriverPerson.GetComponent<MultiPersonControl>();
			}
		}

		private ObscuredInt GetPlayerRank()
		{
			return ObscuredPrefs.GetInt("rank" + DateTime.Now.Day);
		}
	}
}
