using UnityEngine.Scripting;
using _Car_Parking.Scripts.Database.Cars;

namespace _Car_Parking.Scripts.CommonUtility.Contracts
{
	[Preserve]
	public class PhotonCarValidator : IPhotonCarValidator
	{
		public bool IsCarInvalid()
		{
			return UtilsVehicles.IsBlackVehicle();
		}
	}
}
