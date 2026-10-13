using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonActivation : IPersonActivation
	{
		public bool IsActiveDriver(GameObject self)
		{
			if (!FlagsStorage.UseNewLocomotion)
			{
				return false;
			}
			if (!FlagsStorage.OutOfCar || self != V.DriverPerson)
			{
				return false;
			}
			if (FlagsStorage.IsMainMenu && !FlagsStorage.FreeWalk && (!V.CoreGameManager || !V.CoreGameManager.HasFirstCar()))
			{
				return false;
			}
			if (PhotonNetwork.IsConnected)
			{
				self.TryGetComponent<PhotonView>(out var component);
				if (!component || !component.IsMine)
				{
					return false;
				}
			}
			return true;
		}
	}
}
