using UnityEngine;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[CreateAssetMenu(fileName = "New SteeringWheelValueEvent", menuName = "SOAP/SteeringWheelValueEvent")]
	[Tooltip("There can only be one of these")]
	public class SteeringWheelValueEvent : BoolValueEvent
	{
		private int _force;

		public float MomentCoefficient = 0.01f;

		public float Multiplier = 0.05f;

		public int Force
		{
			get
			{
				return _force;
			}
			set
			{
				_force = value;
			}
		}
	}
}
