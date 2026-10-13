using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace CPM.CarVisualModel
{
	public readonly struct LoadedKitInstance
	{
		public readonly GameObject Go;

		public readonly BodyKitType Type;

		public LoadedKitInstance(GameObject go, BodyKitType type)
		{
			Go = go;
			Type = type;
		}
	}
}
