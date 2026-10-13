using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public sealed class RebindRowHighlighter
	{
		public const string OverlayName = "RebindHighlight";

		private const float MinStepSeconds = 0.05f;

		private readonly RebindHighlightStyle _style;

		private Image _overlay;

		private Tween _tween;

		public RebindRowHighlighter(RebindHighlightStyle style)
		{
			_style = style.OrDefault();
		}

		public void Pulse(RectTransform row)
		{
			if ((bool)row)
			{
				Loop(AttachTo(row), _style.Accent, _style.Period);
			}
		}

		public void Flash()
		{
			Image overlay = _overlay;
			if ((bool)overlay && overlay.gameObject.activeSelf)
			{
				overlay.transform.SetAsFirstSibling();
				Loop(overlay, _style.Reject, _style.FlashSeconds);
			}
		}

		private void Loop(Image overlay, Color color, float period)
		{
			Kill();
			float duration = Mathf.Max(0.05f, period * 0.5f);
			overlay.color = color;
			_tween = overlay.DOFade(Mathf.Min(_style.PulseMinAlpha, color.a), duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(isIndependentUpdate: true)
				.SetLink(overlay.gameObject);
		}

		public void Stop()
		{
			Kill();
			if ((bool)_overlay)
			{
				_overlay.gameObject.SetActive(value: false);
			}
		}

		private Image AttachTo(RectTransform row)
		{
			if (!_overlay)
			{
				_overlay = CreateOverlay();
			}
			RectTransform rectTransform = (RectTransform)_overlay.transform;
			if (rectTransform.parent != row)
			{
				rectTransform.SetParent(row, worldPositionStays: false);
				_overlay.gameObject.layer = row.gameObject.layer;
			}
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			rectTransform.SetAsFirstSibling();
			_overlay.gameObject.SetActive(value: true);
			return _overlay;
		}

		private Image CreateOverlay()
		{
			GameObject gameObject = new GameObject("RebindHighlight", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
			gameObject.GetComponent<LayoutElement>().ignoreLayout = true;
			Image component = gameObject.GetComponent<Image>();
			component.raycastTarget = false;
			component.sprite = _style.Sprite;
			component.type = (((bool)_style.Sprite && _style.Sprite.border != Vector4.zero) ? Image.Type.Sliced : Image.Type.Simple);
			return component;
		}

		private void Kill()
		{
			_tween?.Kill();
			_tween = null;
		}
	}
}
