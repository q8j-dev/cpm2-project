using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementDefaultData
	{
		public Transform Parent;

		public Vector2 Position;

		public Vector3 Scale;

		public int Index;

		public Vector2 ScreenPositionCentre;

		public ElementDefaultData(Transform parent, Vector2 position, Vector3 scale, int index, Vector2 screenPositionCentre)
		{
			Parent = parent;
			Position = position;
			Scale = scale;
			Index = index;
			ScreenPositionCentre = screenPositionCentre;
		}
	}
}
