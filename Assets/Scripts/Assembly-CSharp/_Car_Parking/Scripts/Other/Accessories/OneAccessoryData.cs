using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Accessories
{
	[Serializable]
	public class OneAccessoryData
	{
		[Header("Accessory item in UI")]
		public int index;

		public ObscuredInt price = 1000;

		public Texture texture;

		[Header("Accessory view")]
		public AccessoryType accessoryType;

		public GameObject accessoryPrefab;
	}
}
