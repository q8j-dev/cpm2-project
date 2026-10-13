using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Basket
{
	public abstract class BasketItemsVisualConfig : ScriptableObject
	{
		public abstract Dictionary<Enum, BasketItemVisual> GetBasketItemsVisuals();
	}
}
