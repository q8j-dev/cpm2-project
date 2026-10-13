using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public static class ScrollPositionMemory
	{
		public static void Save(ScrollRect scroll, string key)
		{
			if ((bool)scroll)
			{
				Vector2 normalizedPosition = scroll.normalizedPosition;
				if (!float.IsNaN(normalizedPosition.x) && !float.IsNaN(normalizedPosition.y))
				{
					PlayerPrefs.SetFloat(key + ".X", Mathf.Clamp01(normalizedPosition.x));
					PlayerPrefs.SetFloat(key + ".Y", Mathf.Clamp01(normalizedPosition.y));
				}
			}
		}

		public static async UniTask RestoreAsync(ScrollRect scroll, string key, CancellationToken token)
		{
			await UniTask.NextFrame(token);
			if ((bool)scroll && TryLoad(key, out var position))
			{
				Canvas.ForceUpdateCanvases();
				scroll.normalizedPosition = position;
			}
		}

		private static bool TryLoad(string key, out Vector2 position)
		{
			string key2 = key + ".Y";
			if (!PlayerPrefs.HasKey(key2))
			{
				position = Vector2.zero;
				return false;
			}
			position = new Vector2(Mathf.Clamp01(PlayerPrefs.GetFloat(key + ".X", 0f)), Mathf.Clamp01(PlayerPrefs.GetFloat(key2)));
			return true;
		}
	}
}
