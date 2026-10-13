using System.Collections.Generic;
using System.Linq;

namespace GPS
{
	public class GraphNode
	{
		public uint id;

		public List<uint> next;

		public GraphNode(uint id, params uint[] next)
		{
			this.id = id;
			this.next = next.ToList();
		}
	}
}
