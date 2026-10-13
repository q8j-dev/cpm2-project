using Photon.Pun;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	public class RpcSitToCarRequest : RpcObjectBase
	{
		public override byte Code => 105;

		public static void Send(MultiPlayer multiPlayer, int passengerPlace, int vehicleCreatorActorNr)
		{
			multiPlayer.SendValueStrToPlayer<RpcSitToCarRequest>($"{PhotonNetwork.LocalPlayer.ActorNumber}", $"{passengerPlace}", vehicleCreatorActorNr);
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			ExecuteOnPerson(multiPlayerPosControl.FreeDriveProvider, b, name);
		}

		public override void ExecuteOnPerson(FreeDriveProvider freeDriveProvider, string b, string name)
		{
			if (freeDriveProvider != null && (bool)freeDriveProvider.MultiPlayer && (bool)freeDriveProvider.FirstCarMultiPlayer && freeDriveProvider.FirstCarMultiPlayer.TryGetComponent<VehicleData>(out var component))
			{
				int actorNumber = int.Parse(b);
				int place = int.Parse(name);
				VehicleMembersController.TrySitPlaceResult trySitPlaceResult = component.VehicleMembersController.TrySitPlace(place, actorNumber);
				if (trySitPlaceResult != VehicleMembersController.TrySitPlaceResult.Success)
				{
					RpcSitToCarAnswer.Send(freeDriveProvider.MultiPlayer, actorNumber, trySitPlaceResult);
				}
			}
		}
	}
}
