using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.StartGameLoadingScreen.Scripts
{
	public class SetterImage : MonoBehaviour
	{
		[SerializeField]
		private Image ImageA;

		[SerializeField]
		private Image ImageB;

		[SerializeField]
		private Sprite[] Sprites;

		[SerializeField]
		private float FadeDuration = 1f;

		[SerializeField]
		private float ChangeInterval = 5f;

		private readonly string _keyCurrentImage = "CurrentImage";

		private int _currentIndex = -1;

		private bool _useA = true;

		private bool _firstImageSet;

		private void Start()
		{
			Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(ChangeInterval)).Subscribe((long _) =>
			{
				CrossFadeToNextImage();
			}).AddTo(this);
		}

		private void CrossFadeToNextImage()
		{
			if (Sprites == null || Sprites.Length == 0)
			{
				return;
			}
			int num = UnityEngine.Random.Range(0, Sprites.Length);
			if (num == _currentIndex)
			{
				num++;
				if (num >= Sprites.Length)
				{
					num = 0;
				}
			}
			if (!_firstImageSet)
			{
				num = Mathf.Clamp(PlayerPrefs.GetInt(_keyCurrentImage), 0, Sprites.Length - 1);
			}
			_currentIndex = num;
			Image target = (_useA ? ImageA : ImageB);
			Image image = (_useA ? ImageB : ImageA);
			if (!_firstImageSet)
			{
				ImageA.sprite = Sprites[num];
				ImageA.color = Color.white;
				ImageB.color = new Color(1f, 1f, 1f, 0f);
				_firstImageSet = true;
			}
			else
			{
				image.sprite = Sprites[num];
				image.color = new Color(1f, 1f, 1f, 0f);
				Sequence sequence = DOTween.Sequence();
				sequence.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
				sequence.Join(target.DOFade(0f, FadeDuration));
				sequence.Join(image.DOFade(1f, FadeDuration));
				_useA = !_useA;
			}
		}
	}
}
