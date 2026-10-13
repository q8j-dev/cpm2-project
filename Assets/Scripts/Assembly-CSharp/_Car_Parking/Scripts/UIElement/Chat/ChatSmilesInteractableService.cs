using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ChatSmilesInteractableService
	{
		private const int SameTimeSmileMax = 2;

		private const int SmileSendWaitTime = 5;

		private IDisposable[] _smileTimers;

		private int _sendSmileCount;

		private CanvasGroup _canvasGroup;

		private GameObject _timerPanel;

		private TextMeshProUGUI _timerText;

		private CancellationTokenSource _timerCancellation;

		public ChatSmilesInteractableService(CanvasGroup canvasGroup, GameObject timerPanel, TextMeshProUGUI timerText)
		{
			_timerText = timerText;
			_timerPanel = timerPanel;
			_canvasGroup = canvasGroup;
			_smileTimers = new IDisposable[2];
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _timerCancellation);
			IDisposable[] smileTimers = _smileTimers;
			for (int i = 0; i < smileTimers.Length; i++)
			{
				smileTimers[i]?.Dispose();
			}
			_smileTimers = null;
		}

		public void ShowSmiles(bool show)
		{
			_canvasGroup.gameObject.SetActive(show);
		}

		private void SetInteractable(bool interactable)
		{
			_canvasGroup.interactable = interactable;
			_timerPanel.SetActive(!interactable);
		}

		public void OnNewSmile()
		{
			int index = _sendSmileCount;
			_smileTimers[index]?.Dispose();
			_smileTimers[index] = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				CheckForInteractable(index);
			});
			_sendSmileCount++;
			if (_sendSmileCount >= 2)
			{
				CommonExtensions.CancelAndDisposeToken(ref _timerCancellation);
				_timerCancellation = new CancellationTokenSource();
				DelayedSetInteractable(_timerCancellation.Token).Forget();
			}
		}

		private async UniTask DelayedSetInteractable(CancellationToken cancellationToken)
		{
			_timerText.text = 5.ToString();
			SetInteractable(interactable: false);
			int timeLeft = 5;
			while (timeLeft >= 0)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				timeLeft--;
				_timerText.text = timeLeft.ToString();
			}
			SetInteractable(interactable: true);
		}

		private void CheckForInteractable(int index)
		{
			_smileTimers[index]?.Dispose();
			_smileTimers[index] = null;
			_sendSmileCount--;
		}
	}
}
