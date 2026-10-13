using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	public class RpcSitToCarAnswer : RpcObjectBase
	{
		public override byte Code => 106;

		public static void Send(MultiPlayer multiPlayer, int actorNumber, VehicleMembersController.TrySitPlaceResult result)
		{
			multiPlayer.SendValueStrToPlayer<RpcSitToCarAnswer>($"{(int)result}", "", actorNumber);
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			ExecuteOnPerson(multiPlayerPosControl.FreeDriveProvider, b, name);
		}

		public override void ExecuteOnPerson(FreeDriveProvider freeDriveProvider, string b, string name)
		{
			if (freeDriveProvider == null || !freeDriveProvider.MultiPlayer)
			{
				return;
			}
			switch ((VehicleMembersController.TrySitPlaceResult)int.Parse(b))
			{
			case VehicleMembersController.TrySitPlaceResult.CarIsBusy:
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.CarOutInControl)
				{
					V.CoreGameManager.CarOutInControl.ShowBusyMessage(0);
				}
				break;
			case VehicleMembersController.TrySitPlaceResult.CarIsLocked:
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.CarOutInControl)
				{
					V.CoreGameManager.CarOutInControl.ShowBusyMessage(1);
				}
				break;
			}
		}
	}
}
