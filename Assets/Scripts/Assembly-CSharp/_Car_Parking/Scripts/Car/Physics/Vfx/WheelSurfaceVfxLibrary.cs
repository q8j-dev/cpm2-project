using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Physics.Vfx
{
	[CreateAssetMenu(fileName = "WheelSurfaceVfxLibrary", menuName = "CPM/Vfx/Wheel Surface Vfx Library")]
	public class WheelSurfaceVfxLibrary : ScriptableObject
	{
		public List<WheelSurfaceVfxEntry> entries = new List<WheelSurfaceVfxEntry>();
	}
}
