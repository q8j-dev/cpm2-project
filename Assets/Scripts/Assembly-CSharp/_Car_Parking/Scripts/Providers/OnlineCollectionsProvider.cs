using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car.Physics;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	public class OnlineCollectionsProvider
	{
		private readonly int _maxPlayersCount = 32;

		public MultiPlayerPosControl[] CarsOnlineNow { get; }

		public MultiPersonControl[] PlayersOnlineNow { get; }

		public OnlineCollectionsProvider()
		{
			CarsOnlineNow = new MultiPlayerPosControl[_maxPlayersCount];
			PlayersOnlineNow = new MultiPersonControl[_maxPlayersCount];
		}

		public void RegisterCar(MultiPlayerPosControl car)
		{
			if (!car)
			{
				return;
			}
			int num = -1;
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				if (CarsOnlineNow[i] == car)
				{
					return;
				}
				if (num < 0 && !CarsOnlineNow[i])
				{
					num = i;
				}
			}
			if (num < 0)
			{
				Debug.LogWarning("[OnlineCollections] no free car slot for " + car.name);
				return;
			}
			CarsOnlineNow[num] = car;
			car.VehicleData.idForOnlineCarsArray = num;
		}

		public void UnregisterCar(MultiPlayerPosControl car)
		{
			if (!car)
			{
				return;
			}
			if ((bool)car.VehicleData)
			{
				int idForOnlineCarsArray = car.VehicleData.idForOnlineCarsArray;
				if (idForOnlineCarsArray >= 0 && idForOnlineCarsArray < CarsOnlineNow.Length && CarsOnlineNow[idForOnlineCarsArray] == car)
				{
					CarsOnlineNow[idForOnlineCarsArray] = null;
					return;
				}
			}
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				if (CarsOnlineNow[i] == car)
				{
					CarsOnlineNow[i] = null;
					break;
				}
			}
		}

		public bool IsOnlineCarsEmpty()
		{
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				if ((bool)CarsOnlineNow[i])
				{
					return false;
				}
			}
			return true;
		}

		public GameObject GetCarByOwnerID(int ownerID)
		{
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				MultiPlayerPosControl multiPlayerPosControl = CarsOnlineNow[i];
				if ((bool)multiPlayerPosControl)
				{
					VehicleData vehicleData = multiPlayerPosControl.VehicleData;
					if ((bool)vehicleData && vehicleData.CarOwnerID == ownerID)
					{
						return vehicleData.gameObject;
					}
				}
			}
			return null;
		}

		public GameObject GetCarByPhotonViewID(int viewID)
		{
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				MultiPlayerPosControl multiPlayerPosControl = CarsOnlineNow[i];
				if ((bool)multiPlayerPosControl)
				{
					PhotonView photonView = multiPlayerPosControl.photonView;
					if ((bool)photonView && photonView.ViewID == viewID)
					{
						return photonView.gameObject;
					}
				}
			}
			return null;
		}

		public GameObject GetEmptyCar(GameObject carWillDestroy)
		{
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				MultiPlayerPosControl multiPlayerPosControl = CarsOnlineNow[i];
				if ((bool)multiPlayerPosControl && !(multiPlayerPosControl.gameObject == carWillDestroy) && (bool)multiPlayerPosControl.VehicleData && !multiPlayerPosControl.VehicleData.Driver)
				{
					return multiPlayerPosControl.gameObject;
				}
			}
			return null;
		}

		public void EnableOnlineCarsCollision(Collider other)
		{
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				MultiPlayerPosControl multiPlayerPosControl = CarsOnlineNow[i];
				if ((bool)multiPlayerPosControl)
				{
					CarColliders.SetIgnore(multiPlayerPosControl.VehicleData, other, ignore: false);
				}
			}
		}

		public void DisableOnlineCarsCollision(GameObject myCar, Collider other)
		{
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				MultiPlayerPosControl multiPlayerPosControl = CarsOnlineNow[i];
				if ((bool)multiPlayerPosControl && !(multiPlayerPosControl.gameObject == myCar))
				{
					CarColliders.SetIgnore(multiPlayerPosControl.VehicleData, other, ignore: true);
				}
			}
		}

		public void RegisterPlayer(MultiPersonControl player)
		{
			if (!player)
			{
				return;
			}
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				if (!PlayersOnlineNow[i])
				{
					PlayersOnlineNow[i] = player;
					break;
				}
			}
		}

		public void UnregisterPlayer(MultiPersonControl player)
		{
			if (!player)
			{
				return;
			}
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				if (PlayersOnlineNow[i] == player)
				{
					PlayersOnlineNow[i] = null;
					break;
				}
			}
		}

		public MultiPersonControl GetPlayerByPlayerID(string playerID)
		{
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				MultiPersonControl multiPersonControl = PlayersOnlineNow[i];
				if ((bool)multiPersonControl && multiPersonControl.ThisPlayerID == playerID)
				{
					return multiPersonControl;
				}
			}
			return null;
		}

		public PhotonView GetPlayerByPhotonViewID(int photonViewID)
		{
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				MultiPersonControl multiPersonControl = PlayersOnlineNow[i];
				if ((bool)multiPersonControl)
				{
					PhotonView photonView = multiPersonControl.photonView;
					if ((bool)photonView && photonView.ViewID == photonViewID)
					{
						return photonView;
					}
				}
			}
			return null;
		}

		public MultiPersonControl GetPlayerByOwnerActorNr(int ownerActorNr)
		{
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				MultiPersonControl multiPersonControl = PlayersOnlineNow[i];
				if ((bool)multiPersonControl && (bool)multiPersonControl.photonView && multiPersonControl.photonView.OwnerActorNr == ownerActorNr)
				{
					return multiPersonControl;
				}
			}
			return null;
		}

		public MultiPersonControl GetPlayerWithCar(GameObject car)
		{
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				if ((bool)PlayersOnlineNow[i] && PlayersOnlineNow[i].FirstAndOwnCar == car)
				{
					return PlayersOnlineNow[i];
				}
			}
			return null;
		}

		public GameObject GetPlayerByOwnerActorNumber(int actorNumber)
		{
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				MultiPersonControl multiPersonControl = PlayersOnlineNow[i];
				if ((bool)multiPersonControl && (bool)multiPersonControl.photonView && multiPersonControl.photonView.Owner != null && multiPersonControl.photonView.Owner.ActorNumber == actorNumber)
				{
					return multiPersonControl.gameObject;
				}
			}
			return null;
		}

		public void ResetAllCollections()
		{
			ResetOnlineCars();
			ResetOnlinePlayers();
		}

		private void ResetOnlineCars()
		{
			for (int i = 0; i < CarsOnlineNow.Length; i++)
			{
				if ((bool)CarsOnlineNow[i])
				{
					Object.Destroy(CarsOnlineNow[i].gameObject);
				}
				CarsOnlineNow[i] = null;
			}
		}

		private void ResetOnlinePlayers()
		{
			for (int i = 0; i < PlayersOnlineNow.Length; i++)
			{
				if ((bool)PlayersOnlineNow[i])
				{
					Object.Destroy(PlayersOnlineNow[i].gameObject);
				}
				PlayersOnlineNow[i] = null;
			}
		}
	}
}
