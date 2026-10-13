using UnityEngine;

namespace _Car_Parking.Scripts.DynoRun
{
	public abstract class ConditionBase : ScriptableObject
	{
		public string FailMessage;

		public abstract bool CheckCondition(VehicleData vehicleData, GameObject tester);
	}
}
