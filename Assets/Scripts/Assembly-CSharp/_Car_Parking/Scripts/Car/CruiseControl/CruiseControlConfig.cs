using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Car.CruiseControl
{
	[CreateAssetMenu(fileName = "CruiseControlConfig", menuName = "Scriptable/CruiseControlConfig", order = 8)]
	[Preserve]
	public class CruiseControlConfig : ScriptableObject
	{
		[Header("Speed Step")]
		public int speedStep = 10;

		[Header("Speed Parameters")]
		public float speedTolerance = 20f;

		public float speedLimitForDisable = 3f;

		public float maxSpeedToleranceValue = 0.25f;

		public float minSpeedToleranceValue = 0.75f;

		[Preserve]
		public CruiseControlConfig()
		{
		}
	}
}
