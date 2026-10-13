using UnityEngine;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[CreateAssetMenu(fileName = "PlateCameraMoveConfig", menuName = "Configs/Camera/PlateMoveConfig", order = 1)]
	public class PlateCameraMoveConfig : ScriptableObject
	{
		public RemapVar VelocityByFOV;

		public float ClampXPosition;

		public float PosYMax;

		public float PosYMin;
	}
}
