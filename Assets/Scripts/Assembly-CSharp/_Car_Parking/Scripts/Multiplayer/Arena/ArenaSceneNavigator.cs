using System;
using System.Threading;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaSceneNavigator
	{
		private const double TransitionSettleSeconds = 1.0;

		private readonly IArenaNetwork _net;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly CommonProvider _commonProvider;

		public ArenaSceneNavigator(IArenaNetwork net, FreeDriveProvider freeDriveProvider, CommonProvider commonProvider)
		{
			_net = net;
			_freeDriveProvider = freeDriveProvider;
			_commonProvider = commonProvider;
		}

		public async UniTask<ArenaReturnContext> EnterAsync(ArenaConfig config, ArenaEntryOptions entry, CancellationToken ct)
		{
			bool inRoom = _net.InRoom;
			bool fromArenaRoom = inRoom && _freeDriveProvider.RoomArenaId != ArenaId.Free;
			bool flag = ScenesHelper.IsFreeDriveSceneActive();
			ArenaReturnContext ret = ArenaReturnContext.FromMainMenu();
			V.RoomNameToJoin = entry.JoinRoomName ?? "";
			FlagsStorage.CreateForFriendOnly = entry.FriendsOnly || entry.CreateIfMissing;
			FlagsStorage.IsPrivateRoom = entry.FriendsOnly || entry.JoinRoomName != null;
			if (entry.FriendsOnly || entry.CreateIfMissing)
			{
				V.RoomPassword = entry.Password;
			}
			_freeDriveProvider.ArenaFriendRoomInstance = (entry.FriendsOnly ? ArenaRoomNames.GenerateInstance() : "");
			_freeDriveProvider.RoomArenaId = config.ArenaId;
			switch (config.WaitingLocation)
			{
			case ArenaLocation.Racing:
				V.PlaceToSpawn = V.placeToSpawn.Racing;
				break;
			case ArenaLocation.TropicalIsland:
				V.PlaceToSpawn = V.placeToSpawn.TropicalIsland;
				break;
			default:
				V.PlaceToSpawn = V.placeToSpawn.Racing;
				break;
			}
			FlagsStorage.IsMultiFree = true;
			if (flag)
			{
				if (inRoom)
				{
					_commonProvider.TradeService?.CancelOrDeclineAllOffers(TradeOfferCancelReason.PlayerOffline, notifyOtherPlayer: false);
				}
				SceneManager.LoadScene(48);
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				if (!fromArenaRoom)
				{
					await _net.ReconnectAndEnterLobbyAsync(PhotonConnectionProfile.Arena, ct);
				}
				else
				{
					await _net.EnterLobbyFromRoomAsync(ct);
				}
				V.GlobalOffset = Vector3.zero;
				SceneManager.LoadScene(13);
			}
			else
			{
				await _net.ReconnectAndEnterLobbyAsync(PhotonConnectionProfile.Arena, ct);
				V.CoreGameManager.menuControl.ShowLoadingAndLoadMultiFreeDrive();
			}
			return ret;
		}

		public async UniTask<bool> ReturnAsync(ArenaReturnContext ret, CancellationToken ct)
		{
			_freeDriveProvider.ResetArenaId();
			ResetPrivacyFlags();
			if (ret.Target == ArenaReturnTarget.FreeDriveRoom)
			{
				try
				{
					FlagsStorage.IsMultiFree = true;
					V.RoomNameToJoin = "";
					V.PlaceToSpawn = (V.placeToSpawn)ret.PreviousSpawnPlace;
					SceneManager.LoadScene(48);
					await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					await _net.ReconnectAndEnterLobbyAsync(PhotonConnectionProfile.Normal, ct);
					V.GlobalOffset = Vector3.zero;
					SceneManager.LoadScene(13);
				}
				catch (Exception)
				{
					return false;
				}
				return true;
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			MenuLevelParent menuLevelParent = UnityEngine.Object.FindObjectOfType<MenuLevelParent>();
			if ((bool)menuLevelParent)
			{
				menuLevelParent.OpenMainMenu();
			}
			return true;
		}

		private static void ResetPrivacyFlags()
		{
			FlagsStorage.OutOfCar = false;
			FlagsStorage.CreateForFriendOnly = false;
			FlagsStorage.IsPrivateRoom = false;
		}

		public void RecoverToMenu()
		{
			FlagsStorage.IsMultiFree = false;
			V.RoomNameToJoin = "";
			ResetPrivacyFlags();
			_net.Disconnect();
			if (!ScenesHelper.IsMenuSceneActive())
			{
				SceneManager.LoadScene(1);
			}
		}
	}
}
