using System.Collections.Generic;

namespace _Car_Parking.Scripts.Backend
{
	public class HttpResponse
	{
		public long StatusCode { get; set; }

		public IReadOnlyDictionary<string, string> Headers { get; set; }

		public byte[] Body { get; set; }
	}
}
