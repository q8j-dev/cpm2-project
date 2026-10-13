using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Car.Configs
{
	[CreateAssetMenu(fileName = "WheelsTuningConfig", menuName = "Scriptable/WheelsTuningConfig", order = 15)]
	[Preserve]
	public class WheelsDiskTuningConfig : ScriptableObject
	{
		private ObscuredInt _innerOffsetD;

		private bool _tempStatusFlag;

		private long _invisibleCount;

		private char _dummyChar;

		private float _unusedFloatValue;

		private int internalIndex;

		private string hiddenString;

		private ushort magicShortNumber;

		public ObscuredInt WheelsCoinsPrice;

		private decimal decimalTempValue;

		private WheelDiskData[] _allWheels;

		private bool _controlFlag;

		private int[] _wheelsArray;

		private string _anotherHiddenString;

		private float _extraTempFloat;

		private double _backupOffset;

		public WheelDiskData[] AllWheels;

		private bool _dummyToggle;

		private char _secondaryDummyChar;

		private int _miscIndex;

		[Preserve]
		public WheelsDiskTuningConfig()
		{
		}
	}
}
