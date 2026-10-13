using UnityEngine;

namespace _Car_Parking.Scripts.Car.TachometerCustom
{
	[CreateAssetMenu(menuName = "Configs/TachometerCustom/RPMPoints")]
	public class TachometerPointsConfig : ScriptableObject
	{
		public TachometerPoint[] Points;
	}
}
