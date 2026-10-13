using System;
using System.Collections.Generic;

namespace GPS
{
	[Serializable]
	public class Grid
	{
		public List<uint> ids;

		public void Add(uint id)
		{
			if (ids == null)
			{
				ids = new List<uint>();
			}
			ids.Add(id);
		}
	}
}
