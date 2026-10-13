using System.Collections.Generic;
using System.Linq;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchaseHandlersContainer
	{
		private readonly List<IPurchaseHandler> _handlers;

		public PurchaseHandlersContainer(IEnumerable<IPurchaseHandler> handlers)
		{
			_handlers = handlers.ToList();
		}

		public IPurchaseHandler GetHandler(string productID)
		{
			if (_handlers == null)
			{
				return null;
			}
			for (int i = 0; i < _handlers.Count; i++)
			{
				if (_handlers[i].IsHandlerForProduct(productID))
				{
					return _handlers[i];
				}
			}
			return null;
		}
	}
}
