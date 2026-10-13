using System;
using UnityEngine;

namespace Services.PriceStyleService
{
	[Serializable]
	public class PriceStyleData
	{
		public CostType CostType;

		public Sprite PriceSprite;

		public Color PriceColor;
	}
}
