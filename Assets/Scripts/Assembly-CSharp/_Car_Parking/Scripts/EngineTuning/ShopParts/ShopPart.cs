using UnityEngine;

namespace _Car_Parking.Scripts.EngineTuning.ShopParts
{
	public class ShopPart<TPart> : ScriptableObject
	{
		[Header("General")]
		public string Name;

		public string Description;

		[Range(1f, 1000000f)]
		public int Price;

		public TPart Part;

		public string imgPath;

		public virtual string GetName()
		{
			return Name;
		}

		public virtual int GetPrice()
		{
			return Price;
		}
	}
}
