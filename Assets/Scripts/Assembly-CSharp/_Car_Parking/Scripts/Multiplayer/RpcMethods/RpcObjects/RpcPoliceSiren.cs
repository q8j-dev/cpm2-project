using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcPoliceSiren : RpcObjectBase
	{
		public override byte Code => 101;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (string.IsNullOrEmpty(b) || string.IsNullOrEmpty(name))
			{
				return;
			}
			GameObject carByPhotonViewID = multiPlayerPosControl.FreeDriveProvider.MultiPlayer.GetCarByPhotonViewID(int.Parse(b));
			if ((bool)carByPhotonViewID && carByPhotonViewID.TryGetComponent<VehicleData>(out var component) && carByPhotonViewID.TryGetComponent<PhotonView>(out var component2) && !component2.IsMine)
			{
				if (name == "1")
				{
					component.PoliceCarController.PoliceCarSirenControllerInGameplay.StartSiren();
				}
				else if (name == "0")
				{
					component.PoliceCarController.PoliceCarSirenControllerInGameplay.StopSiren();
				}
			}
		}
	}
}
