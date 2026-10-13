using Photon.Pun;

namespace _Car_Parking.Scripts.Multiplayer
{
	public static class MultiplayerPosControlExtensions
	{
		public static bool IsThisCar(this MultiPlayerPosControl multiPlayerPosControl, string b)
		{
			int viewID = multiPlayerPosControl.CarComponents.PhotonView.ViewID;
			return int.Parse(b) == viewID;
		}

		public static bool IsMyVcarAndNotPassenger(this MultiPlayerPosControl multiPlayerPosControl, string b)
		{
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				int actorNumber = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				return int.Parse(b) == actorNumber;
			}
			return false;
		}

		public static bool VDriverPersonPhotonId(string b)
		{
			int actorNumber = V.DriverPerson.GetComponent<PhotonView>().Owner.ActorNumber;
			return int.Parse(b) == actorNumber;
		}
	}
}
