using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class NestedCanvasScaler : MonoBehaviour
	{
		public enum ScreenMatchMode
		{
			MatchWidthOrHeight = 0,
			Expand = 1,
			Shrink = 2
		}

		private const float LogBase = 2f;

		[Tooltip("Higher reference resolution → smaller UI. Mirrors CanvasScaler.referenceResolution.")]
		[SerializeField]
		private Vector2 _referenceResolution = new Vector2(2560f, 1440f);

		[Tooltip("How the scale is chosen relative to the screen aspect. Expand = fit without cropping (min ratio).")]
		[SerializeField]
		private ScreenMatchMode _screenMatchMode = ScreenMatchMode.Expand;

		[SerializeField]
		[Tooltip("Only used in MatchWidthOrHeight mode: 0 = match width, 1 = match height.")]
		[Range(0f, 1f)]
		private float _matchWidthOrHeight = 1f;

		[Tooltip("Divide out the parent's lossyScale so this subtree scales independently of the parent canvas scaler.")]
		[SerializeField]
		private bool _ignoreParentScale = true;

		[Tooltip("On enable, force the root RectTransform to center anchors/pivot and sizeDelta = reference resolution. Turn on once after setup.")]
		[SerializeField]
		private bool _applyReferenceRect;

		[SerializeField]
		[Tooltip("When this component is DISABLED, restore the RectTransform to full-stretch (anchors 0–1, sizeDelta 0, pivot center, scale 1) — the pre-scaler 'as was' state, for A/B testing. Turn off if this window's original layout was NOT full-stretch.")]
		private bool _restoreToStretchOnDisable = true;

		private RectTransform _rt;

		private float _lastScreenW;

		private float _lastScreenH;

		private float _lastParentScale;

		private void OnEnable()
		{
			_rt = (RectTransform)base.transform;
			if (_applyReferenceRect)
			{
				ApplyReferenceRect();
			}
			Apply(force: true);
		}

		private void OnDisable()
		{
			if (_restoreToStretchOnDisable)
			{
				if (!_rt)
				{
					_rt = (RectTransform)base.transform;
				}
				_rt.anchorMin = Vector2.zero;
				_rt.anchorMax = Vector2.one;
				_rt.pivot = new Vector2(0.5f, 0.5f);
				_rt.anchoredPosition = Vector2.zero;
				_rt.sizeDelta = Vector2.zero;
				_rt.localScale = Vector3.one;
			}
		}

		private void OnRectTransformDimensionsChange()
		{
			Apply(force: false);
		}

		private void LateUpdate()
		{
			Apply(force: false);
		}

		private void ApplyReferenceRect()
		{
			if (!_rt)
			{
				_rt = (RectTransform)base.transform;
			}
			_rt.anchorMin = new Vector2(0.5f, 0.5f);
			_rt.anchorMax = new Vector2(0.5f, 0.5f);
			_rt.pivot = new Vector2(0.5f, 0.5f);
			_rt.anchoredPosition = Vector2.zero;
			_rt.sizeDelta = _referenceResolution;
		}

		private void Apply(bool force)
		{
			if (!_rt)
			{
				_rt = (RectTransform)base.transform;
			}
			float num = Screen.width;
			float num2 = Screen.height;
			if (num <= 0f || num2 <= 0f)
			{
				return;
			}
			float num3 = 1f;
			if (_ignoreParentScale && _rt.parent is RectTransform rectTransform)
			{
				num3 = rectTransform.lossyScale.y;
				if (num3 <= 0.0001f)
				{
					num3 = 1f;
				}
			}
			if (force || !Mathf.Approximately(num, _lastScreenW) || !Mathf.Approximately(num2, _lastScreenH) || !Mathf.Approximately(num3, _lastParentScale))
			{
				_lastScreenW = num;
				_lastScreenH = num2;
				_lastParentScale = num3;
				float num4 = ComputeScale(num, num2);
				if (_ignoreParentScale)
				{
					num4 /= num3;
				}
				_rt.localScale = new Vector3(num4, num4, 1f);
			}
		}

		private float ComputeScale(float screenW, float screenH)
		{
			float num = screenW / _referenceResolution.x;
			float num2 = screenH / _referenceResolution.y;
			switch (_screenMatchMode)
			{
			case ScreenMatchMode.Shrink:
				return Mathf.Max(num, num2);
			case ScreenMatchMode.MatchWidthOrHeight:
			{
				float a = Mathf.Log(num, 2f);
				float b = Mathf.Log(num2, 2f);
				float p = Mathf.Lerp(a, b, _matchWidthOrHeight);
				return Mathf.Pow(2f, p);
			}
			default:
				return Mathf.Min(num, num2);
			}
		}
	}
}
