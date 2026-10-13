using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.CircularMenu
{
	public class CircularMenuItem : MonoBehaviour
	{
		public Action onSelectedEvent;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private Image _icon;

		[SerializeField]
		private float _highlightScale = 1.1f;

		[SerializeField]
		private float _highlightDuration = 0.15f;

		[Header("Animation Menu")]
		[Tooltip("Used only when the owning controller has _isAnimationMenu = true. >= 0: plays animation slot N (PlayerPrefs currentAnimation_N). -2: Shop item. -1: not an animation item.")]
		[SerializeField]
		private int _animationSlot = -1;

		private Tween _scaleTween;

		private Tween _colorTween;

		private bool _isActive = true;

		public int AnimationSlot => _animationSlot;

		public Image Icon => _icon;

		public bool IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				_isActive = value;
			}
		}

		public void SetDefault(Sprite sprite)
		{
			_image.sprite = sprite;
			_scaleTween?.Kill();
			_colorTween?.Kill();
			_scaleTween = base.transform.DOScale(1f, _highlightDuration).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true);
		}

		public void SetHovered(Sprite sprite)
		{
			_image.sprite = sprite;
			_scaleTween?.Kill();
			_colorTween?.Kill();
			_scaleTween = base.transform.DOScale(_highlightScale, _highlightDuration).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true);
		}

		public void SetPressed(Sprite sprite)
		{
			_image.sprite = sprite;
		}

		public void ChangeColorOfIcon(bool condition)
		{
			Color color = (condition ? Color.white : Color.gray);
			color.a = (condition ? 1f : 0.5f);
			Image[] componentsInChildren = Icon.GetComponentsInChildren<Image>(includeInactive: false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].color = color;
			}
		}

		public void OnSelect()
		{
			if (_isActive)
			{
				onSelectedEvent?.Invoke();
			}
		}

		private void OnDestroy()
		{
			_scaleTween?.Kill();
			_colorTween?.Kill();
		}
	}
}
