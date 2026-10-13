using System;
using UnityEngine;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.Basket
{
	[Serializable]
	public class BasketItemVisualPersonKit : BasketItemVisual
	{
		[field: SerializeField]
		public KitType KitType { get; private set; }
	}
}
