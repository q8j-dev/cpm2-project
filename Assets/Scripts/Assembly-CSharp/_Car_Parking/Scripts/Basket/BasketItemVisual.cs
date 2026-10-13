using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Basket
{
	[Serializable]
	public class BasketItemVisual
	{
		[field: SerializeField]
		public Sprite Icon { get; private set; }

		[field: SerializeField]
		public string NameKey { get; private set; }
	}
}
