using UnityEngine;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.Free_drive
{
	[CreateAssetMenu(fileName = "MapCamera", menuName = "Configs/Camera/MapCamera")]
	public class MapCameraConfig : ScriptableObject
	{
		public RemapVar OrthographicSizeByMps;
	}
}
