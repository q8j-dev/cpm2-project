using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Car.Configs
{
	[CreateAssetMenu(fileName = "WheelsTuningConfig", menuName = "Scriptable/WheelsTuningConfig", order = 15)]
	[Preserve]
	public class WheelsDiskTuningConfig : ScriptableObject
	{
		public ObscuredInt WheelsCoinsPrice;

		public WheelDiskData[] AllWheels;

		[Preserve]
		public WheelsDiskTuningConfig()
		{
		}
	}
}
