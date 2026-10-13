using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	public class AutoScrollController : IDisposable
	{
		private readonly ScrollSnapBase _scrollSnap;

		private readonly ScrollRect _scrollRect;

		private readonly float _delayToScroll;

		private CancellationTokenSource _cancellationTokenSourceAutoScroll;

		public AutoScrollController(ScrollSnapBase scrollSnap, float delayToScroll)
		{
			_scrollSnap = scrollSnap;
			_scrollRect = _scrollSnap.GetComponent<ScrollRect>();
			_delayToScroll = delayToScroll;
		}

		public void Dispose()
		{
			Stop();
		}

		public void Stop()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceAutoScroll);
		}

		public void Restart()
		{
			Stop();
			_cancellationTokenSourceAutoScroll = new CancellationTokenSource();
			AutoScrollAsync(_cancellationTokenSourceAutoScroll.Token).Forget();
		}

		private async UniTaskVoid AutoScrollAsync(CancellationToken cancellationToken)
		{
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
			while (true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(_delayToScroll), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				GameObject currentSelected = (eventSystem ? eventSystem.currentSelectedGameObject : null);
				if ((bool)currentSelected && _scrollSnap.ChildObjects.Any((GameObject t) => (bool)t && (currentSelected == t || currentSelected.transform.IsChildOf(t.transform))))
				{
					break;
				}
				if (_scrollRect.content.childCount > 1)
				{
					_scrollSnap.NextScreen();
				}
			}
		}
	}
}
