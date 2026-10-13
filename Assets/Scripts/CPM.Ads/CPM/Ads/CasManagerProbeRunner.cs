using UnityEngine;

namespace CPM.Ads
{
	public class CasManagerProbeRunner : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Run the probe automatically shortly after this object starts.")]
		private bool RunOnStart = true;

		[Tooltip("Seconds to wait before running, so the app's own CAS initialization lands first. Set to 0 to probe before anything else has built a manager.")]
		[SerializeField]
		private float DelaySeconds = 5f;

		private void Start()
		{
			if (RunOnStart)
			{
				Invoke("RunProbe", DelaySeconds);
			}
		}

		[ContextMenu("Run CAS manager probe")]
		public void RunProbe()
		{
			CasManagerProbe.Run();
		}
	}
}
