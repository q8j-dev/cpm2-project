using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.DynoRun.Conditions
{
	[CreateAssetMenu(fileName = "TestCondition", menuName = "TestZone/VelocityCondition")]
	public class VelocityCondition : ConditionBase
	{
		public override bool CheckCondition(VehicleData vehicleData, GameObject tester)
		{
			return vehicleData.GetComponent<Powertrain>().SpeedMps <= 10f;
		}
	}
}
