using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Backend
{
	[Preserve]
	public class BackendEnvelope<T>
	{
		[JsonProperty("code")]
		[Preserve]
		public int Code;

		[Preserve]
		[JsonProperty("data")]
		public T Data;
	}
}
