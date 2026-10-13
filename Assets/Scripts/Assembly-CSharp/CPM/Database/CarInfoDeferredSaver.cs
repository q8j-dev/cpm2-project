using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.Database
{
	public static class CarInfoDeferredSaver
	{
		private static readonly HashSet<CarInfo> Pending = new HashSet<CarInfo>();

		private static bool _flushScheduled;

		private static readonly Dictionary<CarInfo, float> PendingDebounced = new Dictionary<CarInfo, float>();

		private static bool _debounceLoopRunning;

		private static bool _lifecycleHooked;

		public static void RequestSave(CarInfo carInfo)
		{
			if (carInfo != null)
			{
				Pending.Add(carInfo);
				if (!_flushScheduled)
				{
					_flushScheduled = true;
					FlushAtEndOfFrame().Forget();
				}
			}
		}

		private static async UniTaskVoid FlushAtEndOfFrame()
		{
			await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
			_flushScheduled = false;
			if (Pending.Count == 0)
			{
				return;
			}
			List<CarInfo> list = new List<CarInfo>(Pending);
			Pending.Clear();
			foreach (CarInfo item in list)
			{
				item.Save();
			}
		}

		public static void RequestSaveDebounced(CarInfo carInfo, float delaySeconds = 0.5f)
		{
			if (carInfo != null)
			{
				HookLifecycle();
				PendingDebounced[carInfo] = Time.realtimeSinceStartup + delaySeconds;
				if (!_debounceLoopRunning)
				{
					_debounceLoopRunning = true;
					DebounceLoop().Forget();
				}
			}
		}

		public static void FlushDebounced()
		{
			if (PendingDebounced.Count == 0)
			{
				return;
			}
			List<CarInfo> list = new List<CarInfo>(PendingDebounced.Keys);
			PendingDebounced.Clear();
			foreach (CarInfo item in list)
			{
				item.Save();
			}
		}

		private static async UniTaskVoid DebounceLoop()
		{
			List<CarInfo> ready = new List<CarInfo>();
			while (PendingDebounced.Count > 0)
			{
				await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				ready.Clear();
				foreach (KeyValuePair<CarInfo, float> item in PendingDebounced)
				{
					if (item.Value <= realtimeSinceStartup)
					{
						ready.Add(item.Key);
					}
				}
				foreach (CarInfo item2 in ready)
				{
					PendingDebounced.Remove(item2);
					item2.Save();
				}
			}
			_debounceLoopRunning = false;
		}

		private static void HookLifecycle()
		{
			if (!_lifecycleHooked)
			{
				_lifecycleHooked = true;
				Application.quitting += FlushDebounced;
				Application.focusChanged += OnFocusChanged;
			}
		}

		private static void OnFocusChanged(bool focused)
		{
			if (!focused)
			{
				FlushDebounced();
			}
		}
	}
}
