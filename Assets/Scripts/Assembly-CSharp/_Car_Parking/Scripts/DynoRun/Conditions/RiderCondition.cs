using CarInteractionAnimationsScripts;
using UnityEngine;

namespace _Car_Parking.Scripts.DynoRun.Conditions
{
	[CreateAssetMenu(fileName = "TestCondition", menuName = "TestZone/RiderCondition")]
	public class RiderCondition : ConditionBase
	{
		public override bool CheckCondition(VehicleData vehicleData, GameObject tester)
		{
			if (vehicleData.Driver == V.DriverPerson && V.Car == vehicleData.gameObject)
			{
				if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<DriverActionController>(out var component))
				{
					PersonAnimationsController personAnimationsController = component.PersonAnimationsController;
					if ((bool)personAnimationsController && personAnimationsController.SitGetOutCarSystem != null && personAnimationsController.SitGetOutCarSystem.SitGetOutData != null)
					{
						if (!personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate)
						{
							return !personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
						}
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}
