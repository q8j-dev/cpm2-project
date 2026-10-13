using System.Collections.Generic;

namespace _Car_Parking.Scripts.Backend
{
	public class InternalHttpRequest
	{
		public string Url { get; set; }

		public HttpVerb Method { get; set; }

		public IReadOnlyDictionary<string, string> Headers { get; set; }

		public byte[] Body { get; set; }

		public int TimeoutInSeconds { get; set; }
	}
}
