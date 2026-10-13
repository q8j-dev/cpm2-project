using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace _Car_Parking.Scripts.Basket
{
	[CreateAssetMenu(fileName = "Configs", menuName = "Configs/BasketItemsVisualPersonKitConfig")]
	public class BasketItemsVisualPersonKitConfig : BasketItemsVisualConfig
	{
		private Dictionary<Enum, BasketItemVisual> _basketItemVisuals;

		[field: SerializeField]
		public BasketItemVisualPersonKit[] BasketItemVisualPersonKits { get; private set; }

		public override Dictionary<Enum, BasketItemVisual> GetBasketItemsVisuals()
		{
			if (_basketItemVisuals.IsNullOrEmpty())
			{
				_basketItemVisuals = new Dictionary<Enum, BasketItemVisual>();
				BasketItemVisualPersonKits.ForEach((BasketItemVisualPersonKit item) =>
				{
					_basketItemVisuals.Add(item.KitType, item);
				});
			}
			return _basketItemVisuals;
		}
	}
}
