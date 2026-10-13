using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Infrastructure
{
	public class DotsLoader : MonoBehaviour
	{
		[Header("Dot objects (array of images/texts)")]
		public Graphic[] Dots;

		[Header("Settings")]
		public float FadeDuration = 0.4f;

		public float DelayBetweenDots = 0.2f;

		public bool PlayOnStart = true;

		private Tween _sequence;

		private void Start()
		{
			if (PlayOnStart)
			{
				Play();
			}
		}

		public void Play()
		{
			Stop();
			if (Dots == null || Dots.Length == 0)
			{
				return;
			}
			Sequence sequence = DOTween.Sequence().SetLoops(-1, LoopType.Restart);
			Graphic[] dots = Dots;
			foreach (Graphic graphic in dots)
			{
				if ((bool)graphic)
				{
					Color color = graphic.color;
					graphic.color = new Color(color.r, color.g, color.b, 0f);
				}
			}
			dots = Dots;
			foreach (Graphic graphic2 in dots)
			{
				if ((bool)graphic2)
				{
					sequence.Append(graphic2.DOFade(1f, FadeDuration));
					sequence.AppendInterval(DelayBetweenDots);
				}
			}
			dots = Dots;
			foreach (Graphic graphic3 in dots)
			{
				if ((bool)graphic3)
				{
					sequence.Append(graphic3.DOFade(0f, FadeDuration));
				}
			}
			_sequence = sequence;
		}

		public void Stop()
		{
			if (_sequence != null)
			{
				_sequence.Kill();
				_sequence = null;
			}
		}

		private void OnDisable()
		{
			Stop();
		}

		private void OnDestroy()
		{
			Stop();
		}
	}
}
