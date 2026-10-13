using UnityEngine;

namespace _Car_Parking.Scripts.DynoRun.Conditions
{
	[CreateAssetMenu(fileName = "TestCondition", menuName = "TestZone/AngleCondition")]
	public class AngleCondition : ConditionBase
	{
		public override bool CheckCondition(VehicleData vehicleData, GameObject tester)
		{
			Vector3 forward = vehicleData.transform.forward;
			Vector3 forward2 = tester.transform.forward;
			return Vector2.Angle(new Vector2(forward.x, forward.z), new Vector2(forward2.x, forward2.z)) <= 10f;
		}
	}
}
