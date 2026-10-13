using UnityEngine;

namespace _Car_Parking.Scripts.Car.Configs
{
	[CreateAssetMenu(fileName = "VinylsTuningConfig", menuName = "Scriptable/VinylsTuningConfig", order = 16)]
	public class VinylsTuningConfig : ScriptableObject
	{
		public OneVinylData[] AllVinyls;
	}
}
