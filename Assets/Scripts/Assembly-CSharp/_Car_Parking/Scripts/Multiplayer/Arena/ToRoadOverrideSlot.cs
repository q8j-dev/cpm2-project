using UnityEngine;
using _Car_Parking.Scripts.ToRoad;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ToRoadOverrideSlot
	{
		private IToRoadHandler _handler;

		private bool HasHandler
		{
			get
			{
				if (_handler != null)
				{
					if (_handler is Component component)
					{
						return component;
					}
					return true;
				}
				return false;
			}
		}

		public void Set(IToRoadHandler handler)
		{
			_handler = handler;
		}

		public void Clear()
		{
			_handler = null;
		}

		public bool TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			result = ToRoadResult.NotHandled();
			if (HasHandler)
			{
				return _handler.TryResolve(in request, out result);
			}
			return false;
		}
	}
}
