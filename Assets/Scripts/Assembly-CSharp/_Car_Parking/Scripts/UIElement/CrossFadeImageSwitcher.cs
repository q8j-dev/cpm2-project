using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class CrossFadeImageSwitcher : MonoBehaviour
	{
		[SerializeField]
		private Image ImageA;

		[SerializeField]
		private Image ImageB;

		[SerializeField]
		private Sprite[] Sprites;

		[SerializeField]
		private float FadeDuration = 0.4f;

		private bool _useA = true;

		private bool _firstImageSet;

		private int _currentIndex = -1;

		private Sequence _fadeSequence;

		public void SwitchTo(int index)
		{
			if (ImageA == null || ImageB == null || Sprites == null || Sprites.Length == 0 || index < 0 || index >= Sprites.Length || index == _currentIndex)
			{
				return;
			}
			_currentIndex = index;
			if (!_firstImageSet)
			{
				ImageA.sprite = Sprites[index];
				ImageA.enabled = true;
				SetAlpha(ImageA, 1f);
				ImageB.enabled = false;
				_firstImageSet = true;
				return;
			}
			Image from = (_useA ? ImageA : ImageB);
			Image image = (_useA ? ImageB : ImageA);
			image.transform.SetAsLastSibling();
			image.sprite = Sprites[index];
			image.enabled = true;
			SetAlpha(image, 0f);
			_fadeSequence?.Kill();
			_fadeSequence = DOTween.Sequence();
			_fadeSequence.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			_fadeSequence.Join(image.DOFade(1f, FadeDuration));
			_fadeSequence.OnComplete(() =>
			{
				from.enabled = false;
			});
			_useA = !_useA;
		}

		private static void SetAlpha(Image image, float alpha)
		{
			Color color = image.color;
			color.a = alpha;
			image.color = color;
		}
	}
}
