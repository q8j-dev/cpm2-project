using UnityEngine;
using UnityEngine.Scripting;

namespace CPM.Database
{
	[Preserve]
	public class DatabaseOutputHandler : IDatabaseOutputHandler
	{
		public void OnProgress(float progress)
		{
			if (progress != 0f)
			{
				Debug.Log($"progress:{progress}");
			}
		}

		public void OnMessage(string message)
		{
			Debug.Log("message:" + message);
		}
	}
}
