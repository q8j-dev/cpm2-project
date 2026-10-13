using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Firebase.Impl
{
	[Preserve]
	public class CallAsyncResTwoSteps<TRes>
	{
		public string result;

		[JsonIgnore]
		public TRes Result
		{
			get
			{
				if (result != null)
				{
					return JsonConvert.DeserializeObject<TRes>(result);
				}
				return default;
			}
		}
	}
}
