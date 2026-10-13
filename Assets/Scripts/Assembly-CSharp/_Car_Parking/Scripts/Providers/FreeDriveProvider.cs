using System;
using System.Threading;
using CPM.Database;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Multiplayer.Chat;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	public class FreeDriveProvider
	{
		public struct MapCameraSetEventData
		{
			public MapCamera OldMapCamera;

			public MapCamera NewMapCamera;
		}

		public MoneySendService MoneySendService;

		public AddFriendService AddFriendService;

		private MapCamera _mapCamera;

		private GameObject _disabledCarHolder;

		public TallPerson TallPerson { get; set; }

		public FreeDriveDB FreeDriveDB { get; set; }

		public FreeDriveControl FreeDriveControl { get; set; }

		public QuickCarSettings QuickCarSettings { get; set; }

		public MultiPlayer MultiPlayer { get; set; }

		public MapControl MapControl { get; set; }

		public ArenaId RoomArenaId { get; set; }

		public string ArenaFriendRoomInstance { get; set; } = "";

		public MapCamera MapCamera
		{
			get
			{
				return _mapCamera;
			}
			set
			{
				MapCamera mapCamera = _mapCamera;
				_mapCamera = value;
				OnSetMapCameraEvent?.Invoke(new MapCameraSetEventData
				{
					OldMapCamera = mapCamera,
					NewMapCamera = _mapCamera
				});
			}
		}

		public LeaderWindow LeaderWindow { get; set; }

		public LeaderBoard LeaderBoard { get; set; }

		public SemaphoreSlim Signal { get; private set; }

		public BannerManager BannerManager { get; set; }

		public PlayersListControl PlayerListControl { get; set; }

		public CarCameras CarCameras { get; set; }

		public DragRacingControlMultiplayer DragRacingControlMultiplayer { get; set; }

		public CoinPurchaseWindow CoinPurchaseWindow { get; set; }

		public GameObject RacingCar2 { get; set; }

		public GameObject FirstCarMultiPlayer { get; set; }

		public Action<bool> OnKeepRowStateChanged { get; set; }

		public event Action<MapCameraSetEventData> OnSetMapCameraEvent;

		public void ResetArenaId()
		{
			RoomArenaId = ArenaId.Free;
			ArenaFriendRoomInstance = "";
		}

		public Transform GetDisabledCarHolder()
		{
			if (!_disabledCarHolder)
			{
				_disabledCarHolder = new GameObject("DisabledCarHolder");
				_disabledCarHolder.transform.SetParent(V.CoreGameManager.globalPoint.transform, worldPositionStays: false);
				_disabledCarHolder.SetActive(value: false);
			}
			return _disabledCarHolder.transform;
		}

		public void TryClearProvider()
		{
			TallPerson = null;
			Signal = null;
			BannerManager = null;
			RacingCar2 = null;
			FirstCarMultiPlayer = null;
			MoneySendService = null;
			AddFriendService = null;
			_disabledCarHolder = null;
		}
	}
}
