using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CPM.Common
{
	[Preserve]
	public class AppStateHandler : MonoBehaviour
	{
		public event Action<bool> ApplicationPause;

		public event Action Destroyed;

		private void Awake()
		{
			if (UnityEngine.Object.FindObjectOfType<AppStateHandler>() != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			ApplicationPause?.Invoke(pauseStatus);
		}

		private void OnDestroy()
		{
			Destroyed?.Invoke();
		}
	}
}
