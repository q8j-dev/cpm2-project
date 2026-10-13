using UnityEngine;

namespace _Car_Parking.Scripts.DynoRun.Conditions
{
	[CreateAssetMenu(fileName = "TestCondition", menuName = "TestZone/DistanceCondition")]
	public class DistanceCondition : ConditionBase
	{
		public override bool CheckCondition(VehicleData vehicleData, GameObject tester)
		{
			Vector3 position = vehicleData.transform.position;
			Vector3 position2 = tester.transform.position;
			position2.y = position.y;
			return Vector3.Distance(position, position2) <= 1f;
		}
	}
}
