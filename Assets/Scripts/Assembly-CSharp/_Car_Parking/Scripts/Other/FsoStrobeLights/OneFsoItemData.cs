using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	[Serializable]
	public class OneFsoItemData
	{
		[Header("Accessory item in UI")]
		public int index;

		public ObscuredInt price = 100;

		public Texture texture;

		[Header("View")]
		public GameObject prefab;
	}
}
