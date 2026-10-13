using System;
using TMPro;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EventSystem.View.Elements
{
	[RequireComponent(typeof(TMP_Text))]
	public class BannerTextElement : MonoBehaviour, IBannerElement
	{
		[SerializeField]
		private BannerTextKind Kind;

		[SerializeField]
		private bool HideWhenEmpty = true;

		private TMP_Text _text;

		private BannerContext _context;

		private IDisposable _timerSubscription;

		public void Bind(BannerContext context)
		{
			if (!_text)
			{
				_text = GetComponent<TMP_Text>();
			}
			_context = context;
			if (Kind == BannerTextKind.Timer)
			{
				BindTimer();
				return;
			}
			string text = ((Kind != BannerTextKind.Title) ? context?.DescriptionKey : context?.TitleKey);
			if (string.IsNullOrEmpty(text))
			{
				if (HideWhenEmpty)
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else
			{
				_text.text = new LocalizedString(text);
			}
		}

		private void OnDestroy()
		{
			_timerSubscription?.Dispose();
		}

		private void BindTimer()
		{
			if (_context?.Event == null || _context.Event.EndTime <= 0 || _context.ServerTimeService == null)
			{
				if (HideWhenEmpty)
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else
			{
				_timerSubscription?.Dispose();
				_timerSubscription = Observable.Interval(TimeSpan.FromSeconds(1.0)).StartWith(0L).Subscribe((long _) =>
				{
					UpdateTimer();
				})
					.AddTo(this);
			}
		}

		private void UpdateTimer()
		{
			if (_context?.Event == null || !_text)
			{
				return;
			}
			long num = _context.Event.EndTime - _context.ServerTimeService.CurrentServerUnixTimeSec;
			if (num < 0)
			{
				num = 0L;
			}
			else
			{
				double num2 = num;
				TimeSpan maxValue = TimeSpan.MaxValue;
				if (num2 > maxValue.TotalSeconds)
				{
					maxValue = TimeSpan.MaxValue;
					num = (long)maxValue.TotalSeconds;
				}
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			_text.text = ((timeSpan.TotalDays >= 1.0) ? string.Format("{0}{1} {2}{3}", timeSpan.Days, new LocalizedString("EventViewWindow.D"), timeSpan.Hours, new LocalizedString("EventViewWindow.H")) : string.Format("{0}{1} {2}{3}", timeSpan.Hours, new LocalizedString("EventViewWindow.H"), timeSpan.Minutes, new LocalizedString("EventViewWindow.M")));
		}
	}
}
