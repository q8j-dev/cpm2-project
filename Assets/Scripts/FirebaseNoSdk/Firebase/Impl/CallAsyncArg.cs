using UnityEngine.Scripting;

namespace Firebase.Impl
{
	[Preserve]
	public class CallAsyncArg
	{
		public object data;

		public CallAsyncArg(object data)
		{
			if (data != null)
			{
				this.data = data;
			}
		}
	}
}
