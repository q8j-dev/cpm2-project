using System.Collections.Generic;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	public class UITouchHelper
	{
		private List<string> _touchIds = new List<string>();

		public bool HaveTouch => _touchIds.Count > 0;

		public void AddTouchId(string id)
		{
			if (!_touchIds.Contains(id))
			{
				_touchIds.Add(id);
			}
		}

		public void RemoveTouchId(string id)
		{
			_touchIds.Remove(id);
		}
	}
}
