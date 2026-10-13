using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "ConnectionRod", menuName = "CarPartsTuning/EngineParts/BlockParts/ConnectionRod", order = 21)]
	public class ConnectionRodsPart : ReciprocatingEnginePart
	{
		[SerializeField]
		private float Length;

		public float GetLength => Length;

		public bool GetValue(float condition)
		{
			return condition > 0.5f;
		}
	}
}
