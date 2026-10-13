using System;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.UIElement
{
	public class OfferTimerController : IDisposable
	{
		private const int TimeOffset = -5;

		private TMP_Text _textTMP;

		private Text _text;

		private GameObject _root;

		private CancellationTokenSource _cancellationTokenSource;

		public OfferTimerController(TMP_Text textTMP, Text text, GameObject root)
		{
			_textTMP = textTMP;
			_text = text;
			_root = root;
		}

		public void Dispose()
		{
			StopTimer();
		}

		public void SetOffer(OfferModel offerModel, Action actionTimerIsOver)
		{
			_root.SetActive(value: false);
			StopTimer();
			_cancellationTokenSource = new CancellationTokenSource();
			TimerAsync(offerModel, actionTimerIsOver, _cancellationTokenSource.Token).Forget();
		}

		public void StopTimer()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSource);
		}

		private async UniTaskVoid TimerAsync(OfferModel offerModel, Action actionTimerIsOver, CancellationToken cancellationToken)
		{
			if (offerModel == null)
			{
				return;
			}
			_root.SetActive(value: true);
			while (offerModel.IsValid)
			{
				if ((bool)_textTMP)
				{
					_textTMP.text = Utils.SecondsToText(offerModel.SecondsToComplete + -5);
				}
				if ((bool)_text)
				{
					_text.text = Utils.SecondsToText(offerModel.SecondsToComplete + -5);
				}
				await UniTask.Delay(TimeSpan.FromSeconds(1.0));
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			_root.SetActive(value: false);
			if (offerModel.TimeIsOver)
			{
				actionTimerIsOver?.Invoke();
			}
		}
	}
}
