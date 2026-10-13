using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public class EmoteLockerItem : MonoBehaviour
	{
		private const float DisabledIconAlpha = 0.4f;

		private const float TapScale = 1.2f;

		private const float TapDuration = 0.2f;

		private const float SelectionStartScale = 1.1f;

		private const float SelectionDuration = 0.05f;

		[SerializeField]
		private Button SelectButton = default;

		[SerializeField]
		private Image Icon = default;

		[SerializeField]
		private GameObject StarBadge = default;

		[SerializeField]
		private GameObject CoinBadge = default;

		[SerializeField]
		private GameObject OwnedBadge = default;

		[SerializeField]
		private Image SelectionFrame = default;

		private int _id;

		private bool _disabled;

		private bool _selected;

		private float _selectionAlpha = 1f;

		private Tween _tapTween;

		private Tween _selectionTween;

		public int Id => _id;

		public event Action<EmoteLockerItem> Tapped;

		private void Awake()
		{
			if ((bool)SelectionFrame)
			{
				_selectionAlpha = SelectionFrame.color.a;
			}
			if ((bool)SelectButton && (bool)Icon)
			{
				SelectButton.onClick.AddListener(OnClick);
			}
		}

		private void OnDisable()
		{
			KillTapTween();
			ResetTapVisual();
			_selectionTween?.Complete();
			_selectionTween = null;
		}

		private void OnDestroy()
		{
			KillTapTween();
			KillSelectionTween();
		}

		public void Init(int id, Sprite sprite)
		{
			if ((bool)SelectButton && (bool)Icon)
			{
				KillTapTween();
				_id = id;
				Icon.sprite = sprite;
				SetSelected(value: false);
				SetFavorite(value: false);
				SetCoinBadge(show: false);
				SetOwnedBadge(show: false);
				SetDisabled(disabled: false);
				ResetTapVisual();
			}
		}

		public void SetFavorite(bool value)
		{
			if ((bool)StarBadge)
			{
				StarBadge.SetActive(value);
			}
		}

		public void SetSelected(bool value)
		{
			if ((bool)SelectionFrame && (!value || !_selected))
			{
				_selected = value;
				KillSelectionTween();
				if (!value)
				{
					SelectionFrame.gameObject.SetActive(value: false);
					return;
				}
				SelectionFrame.gameObject.SetActive(value: true);
				SetSelectionAlpha(0f);
				SelectionFrame.rectTransform.localScale = Vector3.one * 1.1f;
				_selectionTween = DOTween.Sequence().Append(SelectionFrame.rectTransform.DOScale(Vector3.one, 0.05f)).Join(SelectionFrame.DOFade(_selectionAlpha, 0.05f))
					.SetUpdate(isIndependentUpdate: true)
					.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			}
		}

		public void SetCoinBadge(bool show)
		{
			if ((bool)CoinBadge)
			{
				CoinBadge.SetActive(show);
			}
		}

		public void SetOwnedBadge(bool show)
		{
			if ((bool)OwnedBadge)
			{
				OwnedBadge.SetActive(show);
			}
		}

		public void SetDisabled(bool disabled)
		{
			_disabled = disabled;
			SelectButton.interactable = !disabled;
			Color color = Icon.color;
			color.a = (disabled ? 0.4f : 1f);
			Icon.color = color;
		}

		private void OnClick()
		{
			if (_tapTween == null || !_tapTween.IsActive())
			{
				int tappedId = _id;
				Icon.rectTransform.localScale = Vector3.one;
				_tapTween = DOTween.Sequence().Append(Icon.rectTransform.DOScale(1.2f, 0.2f)).Join(Icon.DOFade(0f, 0.2f))
					.SetUpdate(isIndependentUpdate: true)
					.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy)
					.OnComplete(() =>
					{
						CompleteTap(tappedId);
					});
			}
		}

		private void CompleteTap(int tappedId)
		{
			_tapTween = null;
			ResetTapVisual();
			if (tappedId == _id)
			{
				Tapped?.Invoke(this);
			}
		}

		private void KillTapTween()
		{
			_tapTween?.Kill();
			_tapTween = null;
		}

		private void KillSelectionTween()
		{
			_selectionTween?.Kill();
			_selectionTween = null;
		}

		private void SetSelectionAlpha(float alpha)
		{
			Color color = SelectionFrame.color;
			color.a = alpha;
			SelectionFrame.color = color;
		}

		private void ResetTapVisual()
		{
			if ((bool)Icon)
			{
				Icon.rectTransform.localScale = Vector3.one;
				Color color = Icon.color;
				color.a = (_disabled ? 0.4f : 1f);
				Icon.color = color;
			}
		}
	}
}
