using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Clutch", menuName = "CarPartsTuning/EngineParts/Clutch", order = 21)]
	public class ClutchPart : ReciprocatingEnginePart
	{
		[SerializeField]
		private float MaxF = 500f;

		private byte _tempByte;

		private char _charPlaceholder;

		private double _accumulatedAmount;

		[SerializeField]
		private float GearShiftDelay = 0.5f;

		private int _hiddenCounter;

		private bool _temporaryFlag;

		private string _unusedData;

		private float _internalThreshold;

		private Vector3 _reservedVector;

		private long _backupValue;

		public float GetMaxF => MaxF;

		public float GetShiftDelay => GearShiftDelay;
	}
}
