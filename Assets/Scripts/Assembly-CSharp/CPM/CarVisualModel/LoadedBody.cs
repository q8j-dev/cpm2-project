using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CPM.CarVisualModel
{
	public readonly struct LoadedBody
	{
		public readonly GameObject Body;

		public readonly AsyncOperationHandle Handle;

		public readonly bool Valid;

		public LoadedBody(GameObject body, AsyncOperationHandle handle, bool valid)
		{
			Body = body;
			Handle = handle;
			Valid = valid;
		}
	}
}
