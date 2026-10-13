using System;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

public class MultiplayerEventStartHandler : MonoBehaviourPunCallbacks
{
	private V.placeToSpawn _placeToSpawn = V.placeToSpawn.Racing;

	private ManagerUI _managerUI;

	private PhotonReconnectionHandler _photonReconnectionHandler;

	private IDisposable _closer;

	private const int TimeOutTime = 90;

	private bool _wasTryJoinToEventRoom;

	[Inject]
	public void Construct(ManagerUI managerUI, PhotonReconnectionHandler photonReconnectionHandler)
	{
		_managerUI = managerUI;
		_photonReconnectionHandler = photonReconnectionHandler;
	}

	public void SetPlaceToSpawn(V.placeToSpawn placeToSpawn)
	{
		_placeToSpawn = placeToSpawn;
	}

	public void TryJoinToEventRoom()
	{
		if (!FlagsStorage.IsMainMenu)
		{
			return;
		}
		if (!InternetHelper.HasInternetConnection())
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f).Forget();
			return;
		}
		_wasTryJoinToEventRoom = true;
		_managerUI.ShowWaitingWindow(set: true).Forget();
		if (!PhotonNetwork.IsConnected)
		{
			_photonReconnectionHandler.ReconnectHard(PhotonConnectionProfile.Normal);
		}
		else
		{
			OnConnectedToMaster();
		}
		_closer?.Dispose();
		_closer = Observable.Timer(TimeSpan.FromSeconds(90.0)).Subscribe((long _) =>
		{
			_wasTryJoinToEventRoom = false;
			_managerUI.CloseWaitingWindow();
			_managerUI.ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f).Forget();
		}).AddTo(this);
	}

	public override void OnConnectedToMaster()
	{
		if (FlagsStorage.IsMainMenu && _wasTryJoinToEventRoom)
		{
			_wasTryJoinToEventRoom = false;
			FlagsStorage.IsLoadingFromEvent = true;
			FlagsStorage.CreateForFriendOnly = false;
			V.RoomNameToJoin = "";
			FlagsStorage.IsMultiFree = true;
			V.PlaceToSpawn = _placeToSpawn;
			V.CoreGameManager.menuControl.ShowLoadingAndLoadMultiFreeDrive();
		}
	}
}
