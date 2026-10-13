using System;
using System.Collections.Generic;
using CarInteractionAnimationsScripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Free_drive
{
	[Preserve]
	public class FreeDrivePlayerPropertyCarSitListener : IDisposable, IInRoomCallbacks
	{
		private const float ReconcileIntervalSeconds = 1f;

		private readonly Dictionary<int, int[]> _appliedCarSeats = new Dictionary<int, int[]>();

		private readonly IDisposable _reconcileLoop;

		public FreeDrivePlayerPropertyCarSitListener()
		{
			PhotonNetwork.AddCallbackTarget(this);
			_reconcileLoop = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				ReconcileAllPlayers();
			});
		}

		public void Dispose()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			_reconcileLoop?.Dispose();
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			ApplySeats(newPlayer, playAnimation: false);
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			_appliedCarSeats.Remove(otherPlayer.ActorNumber);
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			if (changedProps.ContainsKey("CarSeats"))
			{
				ApplySeats(targetPlayer, playAnimation: true);
			}
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
		}

		private void ReconcileAllPlayers()
		{
			if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
			{
				return;
			}
			foreach (Player value in PhotonNetwork.CurrentRoom.Players.Values)
			{
				ApplySeats(value, playAnimation: false);
			}
		}

		private void ApplySeats(Player targetPlayer, bool playAnimation)
		{
			if (targetPlayer == null || !V.CoreGameManager || V.CoreGameManager.FreeDriveProvider == null || !V.CoreGameManager.FreeDriveProvider.MultiPlayer || targetPlayer.CustomProperties == null || !targetPlayer.CustomProperties.TryGetValue("CarSeats", out var value) || !(value is int[] array))
			{
				return;
			}
			int[] appliedSeats = GetAppliedSeats(targetPlayer.ActorNumber, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (appliedSeats[i] != array[i])
				{
					if (array[i] == -1)
					{
						appliedSeats[i] = array[i];
					}
					else if (TryApplySeat(targetPlayer, array[i], i, playAnimation))
					{
						appliedSeats[i] = array[i];
					}
				}
			}
		}

		private int[] GetAppliedSeats(int actorNumber, int length)
		{
			if (!_appliedCarSeats.TryGetValue(actorNumber, out var value) || value.Length != length)
			{
				value = new int[length];
				for (int i = 0; i < length; i++)
				{
					value[i] = -1;
				}
				_appliedCarSeats[actorNumber] = value;
			}
			return value;
		}

		private bool TryApplySeat(Player carOwner, int sittingActorNumber, int place, bool playAnimation)
		{
			MultiPlayer multiPlayer = V.CoreGameManager.FreeDriveProvider.MultiPlayer;
			MultiPersonControl multiPersonControl = multiPlayer.GetMultiPersonControl(sittingActorNumber);
			if (!multiPersonControl && PhotonNetwork.LocalPlayer.ActorNumber == sittingActorNumber && (bool)V.DriverPerson)
			{
				multiPersonControl = V.DriverPerson.GetComponent<MultiPersonControl>();
			}
			GameObject gameObject = multiPlayer.GetCarByOwnerID(carOwner.ActorNumber);
			if (!gameObject && PhotonNetwork.LocalPlayer.ActorNumber == sittingActorNumber && (bool)V.CoreGameManager.FreeDriveProvider.FirstCarMultiPlayer)
			{
				gameObject = V.CoreGameManager.FreeDriveProvider.FirstCarMultiPlayer;
			}
			if (!multiPersonControl || !gameObject || !gameObject.TryGetComponent<VehicleData>(out var component))
			{
				return false;
			}
			if (component.VehicleMembersController.HaveMember(multiPersonControl.gameObject))
			{
				return true;
			}
			if (!multiPersonControl.gameObject.TryGetComponent<MainPerson>(out var component2))
			{
				return false;
			}
			if (IsAnimatingSitOrGetOut(component2))
			{
				return false;
			}
			if (sittingActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				if (!component2.PersonAnimationsController || component2.PersonAnimationsController.SitGetOutCarSystem == null || !component.MainModel)
				{
					return false;
				}
				if (place == 0)
				{
					component2.PersonAnimationsController.SitGetOutCarSystem.SitIntoCarDrivingForce(gameObject);
				}
				else
				{
					component2.PersonAnimationsController.SitGetOutCarSystem.SitIntoCarPassengerForce(gameObject, playAnimation: true, place);
				}
			}
			else
			{
				multiPlayer.PlayerEnteredToCar(component2.gameObject, gameObject, place, playAnimation);
			}
			return component.VehicleMembersController.HaveMember(multiPersonControl.gameObject);
		}

		private static bool IsAnimatingSitOrGetOut(MainPerson mainPerson)
		{
			if (!mainPerson.PersonAnimationsController || mainPerson.PersonAnimationsController.SitGetOutCarSystem == null)
			{
				return false;
			}
			SitGetOutData sitGetOutData = mainPerson.PersonAnimationsController.SitGetOutCarSystem.SitGetOutData;
			if (sitGetOutData != null)
			{
				if (!sitGetOutData.IsSittingIntoCarAnimate)
				{
					return sitGetOutData.IsGettingOutOfTheCarAnimate;
				}
				return true;
			}
			return false;
		}
	}
}
