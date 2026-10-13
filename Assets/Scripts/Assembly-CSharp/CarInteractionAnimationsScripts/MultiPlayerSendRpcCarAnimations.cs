using Photon.Pun;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Multiplayer.RpcMethods;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;

namespace CarInteractionAnimationsScripts
{
	public class MultiPlayerSendRpcCarAnimations
	{
		public void SendRpcSeatIntoCarAnimation(GameObject person, GameObject car, int place, bool playAnimation)
		{
			SendRpc<RpcSitIntoCar>(person, car, new string[2]
			{
				place.ToString(),
				playAnimation ? "1" : "0"
			});
		}

		public void SendGetOutCar(GameObject person, GameObject car, int place, bool playAnimation)
		{
			SendRpc<RpcGetOutCar>(person, car, new string[2]
			{
				place.ToString(),
				playAnimation ? "1" : "0"
			});
		}

		public void SendOpenCloseCarDoor(GameObject person, GameObject car, int place, CarAnimationType carAnimationType)
		{
			string[] obj = new string[2]
			{
				place.ToString(),
				null
			};
			int num = (int)carAnimationType;
			obj[1] = num.ToString();
			SendRpc<RpcOpenCloseDoorAnimate>(person, car, obj);
		}

		private void SendRpc<TRpcType>(GameObject person, GameObject car, params string[] data) where TRpcType : RpcObjectBase
		{
			if (FlagsStorage.IsMultiFree && person.TryGetComponent<PhotonView>(out var component) && component.IsMine && car.TryGetComponent<PhotonView>(out var component2))
			{
				string b = string.Join(',', component.ViewID.ToString(), component2.ViewID.ToString());
				string name = string.Join(',', data);
				V.CoreGameManager.FreeDriveProvider.MultiPlayer.SendValueStr<TRpcType>(b, name);
			}
		}
	}
}
