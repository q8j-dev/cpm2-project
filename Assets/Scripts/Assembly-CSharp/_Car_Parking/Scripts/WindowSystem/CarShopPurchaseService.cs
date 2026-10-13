using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	public class CarShopPurchaseService
	{
		private readonly List<ICarPurchaseHandler> _handlers = new List<ICarPurchaseHandler>();

		public void Register(ICarPurchaseHandler handler)
		{
			if (handler != null && !_handlers.Contains(handler))
			{
				_handlers.Add(handler);
			}
		}

		public void Unregister(ICarPurchaseHandler handler)
		{
			_handlers.Remove(handler);
		}

		public bool TryHandle(int carId, Action hideShop)
		{
			CarPurchaseContext context = new CarPurchaseContext(carId, hideShop);
			for (int i = 0; i < _handlers.Count; i++)
			{
				if (_handlers[i].TryHandle(context))
				{
					return true;
				}
			}
			return false;
		}
	}
}
