using System;
using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode
{
	[Serializable]
	public class ParameterInfo
	{
		public int Id;

		public string NameKey;

		public string InfoKey;

		public int DefaultValue;

		public Sprite Icon;

		public PhotoModeScrollDefaultValues ScrollDefaultValues;
	}
}
