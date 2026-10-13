using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Wallet
{
	[Serializable]
	[Preserve]
	public class ItemData
	{
		public string ProductId;

		public object PayLoad;

		public ItemData()
		{
		}

		public ItemData(string productId)
		{
			ProductId = productId;
		}

		public ItemData(string productId, object payLoad)
		{
			ProductId = productId;
			PayLoad = payLoad;
		}
	}
}
