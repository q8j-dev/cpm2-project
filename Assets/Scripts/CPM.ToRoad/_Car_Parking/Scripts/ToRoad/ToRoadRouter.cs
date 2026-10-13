using System.Collections.Generic;
using System.Linq;

namespace _Car_Parking.Scripts.ToRoad
{
	public sealed class ToRoadRouter
	{
		private readonly IToRoadHandler[] _handlers;

		public ToRoadRouter(IEnumerable<IToRoadHandler> handlers)
		{
			_handlers = handlers.OrderByDescending((IToRoadHandler h) => (int)h.Priority).ToArray();
			for (int num = 1; num < _handlers.Length; num++)
			{
				_ = _handlers[num].Priority;
				_ = _handlers[num - 1].Priority;
			}
		}

		public ToRoadResult Resolve(in ToRoadRequest request)
		{
			IToRoadHandler[] handlers = _handlers;
			for (int i = 0; i < handlers.Length; i++)
			{
				if (handlers[i].TryResolve(in request, out var result))
				{
					return result;
				}
			}
			return ToRoadResult.NotHandled();
		}
	}
}
