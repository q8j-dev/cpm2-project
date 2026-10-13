using System;
using System.Threading;
using CPM.Database;
using CPM.Rating.Entities;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Multiplayer.Treasure
{
	public class MyTreasureManager : MonoBehaviourPunCallbacks
	{
		private const int k_RespawnTime = 600;

		private const int k_GenTime = 60;

		private const string k_TreasurePosKey = "TrP";

		[SerializeField]
		private MyTreasure TreasurePrefab;

		private int _positionIndex;

		private CancellationTokenSource _currentOperationCts;

		private CurrencyManagerController _currencyManagerController;

		private FreeDriveDB _freeDriveDB;

		private CoreGameManager _coreGameManager;

		private MyTreasure _treasureInstance;

		private bool _isFirstChecked;

		private IPrefabFactory _prefabFactory;

		private bool CanChangeTreasureState
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

		private MyTreasure Treasure
		{
			get
			{
				if (!_treasureInstance)
				{
					InstantiateTreasure();
				}
				return _treasureInstance;
			}
		}

		[Inject]
		public void Construct(CurrencyManagerController currencyManagerController, IPrefabFactory prefabFactory, FreeDriveDB freeDriveDB, CoreGameManager coreGameManager)
		{
			_prefabFactory = prefabFactory;
			_coreGameManager = coreGameManager;
			_freeDriveDB = freeDriveDB;
			_currencyManagerController = currencyManagerController;
		}

		private void Awake()
		{
			if (FlagsStorage.IsMultiFree && PhotonNetwork.InRoom)
			{
				FirstCheckTreasure();
			}
		}

		private void OnDestroy()
		{
			if (_currentOperationCts != null)
			{
				_currentOperationCts.Cancel();
				_currentOperationCts.Dispose();
				_currentOperationCts = null;
			}
		}

		private void FirstCheckTreasure()
		{
			if (!_isFirstChecked)
			{
				CheckTreasure();
				_isFirstChecked = true;
			}
		}

		private void InstantiateTreasure()
		{
			_treasureInstance = _prefabFactory.Instantiate(TreasurePrefab, Vector3.zero, Quaternion.identity, base.transform.parent);
			_treasureInstance.Init(this);
		}

		public override void OnJoinedRoom()
		{
			FirstCheckTreasure();
		}

		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			if (propertiesThatChanged.TryGetValue("TrP", out var value))
			{
				int posIndex = (int)value;
				UpdateTreasureState(posIndex);
			}
		}

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (_currentOperationCts == null && CanChangeTreasureState)
			{
				CheckTreasure();
			}
		}

		private void CheckTreasure()
		{
			if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("TrP", out var value))
			{
				int posIndex = (int)value;
				UpdateTreasureState(posIndex);
			}
			else
			{
				SetTreasureRoomProperty(-1);
			}
		}

		private void UpdateTreasureState(int posIndex)
		{
			_positionIndex = posIndex;
			Treasure.SetToNewPointOrDisable(posIndex);
			if (_currentOperationCts != null)
			{
				_currentOperationCts.Cancel();
				_currentOperationCts.Dispose();
				_currentOperationCts = null;
			}
			_currentOperationCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			DelayBeforeUpdateStateAsync(GetDelayTime(posIndex), (posIndex == -1) ? Treasure.GetRandomPositionIndex() : (-1), _currentOperationCts.Token).Forget();
		}

		private int GetDelayTime(int posIndex)
		{
			if (posIndex != -1)
			{
				return 600;
			}
			return 60;
		}

		private async UniTask DelayBeforeUpdateStateAsync(int delay, int posIndex, CancellationToken ct)
		{
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				_currentOperationCts = null;
				if (CanChangeTreasureState)
				{
					SetTreasureRoomProperty(posIndex);
				}
				else
				{
					UpdateTreasureState(-1);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void SetTreasureRoomProperty(int posIndex)
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "TrP", posIndex } });
		}

		public void Collided()
		{
			int num = UnityEngine.Random.Range(10, 40) * 1000;
			_currencyManagerController.AddMoney(new AdvancedInt(num));
			_coreGameManager.SetMessage(" + " + num, 3f);
			int num2 = ObscuredPrefs.GetInt("treasureCount");
			ObscuredPrefs.SetInt("treasureCount", ++num2);
			FreeDriveDB freeDriveDB = _freeDriveDB;
			int? treasure = 1;
			freeDriveDB.SendRatingData(new RatingData(null, null, null, null, treasure), forceSend: true).Forget();
			Treasure.SetToNewPointOrDisable(-1);
			SetTreasureRoomProperty(-1);
		}
	}
}
