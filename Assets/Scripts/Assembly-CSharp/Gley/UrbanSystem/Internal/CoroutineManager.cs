using System.Collections;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class CoroutineManager : MonoBehaviour
	{
		private static CoroutineManager instance;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			instance = null;
		}

		public static void StartStaticCoroutine(IEnumerator coroutine)
		{
			if (instance != null)
			{
				instance.StartCoroutine(coroutine);
			}
			else
			{
				Debug.LogError("CoroutineManager instance is not initialized.");
			}
		}
	}
}
