using System.Collections.Generic;
using CPM.Shifter.Contracts;
using CPM.Shifter.Logic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.CommonUtility;

[RequireComponent(typeof(RectTransform))]
public class ManualHShifterView : MonoBehaviour, IManualHShifterView
{
	[Header("Grid")]
	[SerializeField]
	[Tooltip("X = INSET (px) from ManualVisuals' RIGHT EDGE for the rightmost active column. Default 0 = flush with the edge; use small negative for inward padding. Computed in slotParent's local space via TransformPoint, so it works regardless of intermediate parents' anchors/pivots. Y = top-row Y position in slotParent local space.")]
	private Vector2 _gridOrigin = new Vector2(0f, 100f);

	[SerializeField]
	private Vector2 _cellSize = new Vector2(120f, 200f);

	[SerializeField]
	private float _channelThickness = 12f;

	[SerializeField]
	[Header("Slots (8 RectTransforms — indices 0..7 = Slot_1, Slot_2 … Slot_7, Slot_R)")]
	private RectTransform[] _slotTransforms;

	[Header("Channel UI lines")]
	[SerializeField]
	private Image[] _verticalChannels;

	[SerializeField]
	private Image _horizontalChannel;

	[SerializeField]
	[Tooltip("Radius of each slot's circle visual in pixels. Vertical channels are trimmed by this amount near every slot so the line ends at the circle's edge instead of running into its center. The knob target stays at the slot center (= circle center). Set to match the actual circle radius in the prefab; 0 disables trimming (line runs into circle center).")]
	private float _slotCircleRadius = 40f;

	[SerializeField]
	[Header("Knob")]
	private RectTransform _knob;

	[SerializeField]
	private CanvasGroup _gearTraceCanvasGroup;

	[Header("Slot gear labels (indices match _slotTransforms 0..7 = gears 1..7, R). New circle-based design: each Slot_N has a circle visual, label sits INSIDE the circle at slot center — set _slotLabelOffset = 0 in the inspector. Non-zero offset nudges the label up (top-row gears) or down (bottom-row gears) relative to the circle center.")]
	[SerializeField]
	private RectTransform[] _slotLabels;

	[SerializeField]
	private float _slotLabelOffset = 40f;

	[Header("Trace reveal (on knob touch)")]
	[SerializeField]
	private float _traceRevealDuration = 0.15f;

	[SerializeField]
	private float _traceHideDuration = 0.25f;

	[SerializeField]
	private float _traceHideDelay = 0.2f;

	[Header("Knob grab scale (visual feedback on touch)")]
	[SerializeField]
	private float _knobGrabScale = 1.16f;

	[SerializeField]
	private float _knobScaleDuration = 0.1f;

	private readonly List<Rect> _allowedZones = new List<Rect>();

	private IReadOnlyDictionary<int, Vector2> _slotPixelPositions;

	private Vector2 _neutralCenter;

	private int _lastAppliedHash;

	private bool _layoutReady;

	private Tween _knobTween;

	private Tween _traceFadeTween;

	private Tween _knobScaleTween;

	private Vector3 _knobBaseScale = Vector3.one;

	public bool IsLayoutReady => _layoutReady;

	public Vector2 KnobAnchoredPos
	{
		get
		{
			if (!(_knob != null))
			{
				return Vector2.zero;
			}
			return _knob.anchoredPosition;
		}
	}

	public IReadOnlyList<Rect> AllowedZones => _allowedZones;

	public float LocalContentCenterX => _neutralCenter.x;

	public void ApplyLayout(ShifterLayoutAsset layout)
	{
		if (layout == null)
		{
			Debug.LogError("ManualHShifterView.ApplyLayout: null layout");
			return;
		}
		int num = HashCode(layout);
		if (num == _lastAppliedHash && _layoutReady)
		{
			return;
		}
		for (int i = 0; i < _slotTransforms.Length; i++)
		{
			_slotTransforms[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _verticalChannels.Length; j++)
		{
			_verticalChannels[j].gameObject.SetActive(value: false);
		}
		if ((bool)_horizontalChannel)
		{
			_horizontalChannel.gameObject.SetActive(value: false);
		}
		if (_slotLabels != null)
		{
			for (int k = 0; k < _slotLabels.Length; k++)
			{
				if (_slotLabels[k] != null)
				{
					_slotLabels[k].gameObject.SetActive(value: false);
				}
			}
		}
		IReadOnlyDictionary<int, SlotCoord> readOnlyDictionary = ShifterMath.LayoutToSlotPositions(layout.NumGears, layout.ReversePos, layout.DoglegFirstGear);
		int num2 = 0;
		foreach (KeyValuePair<int, SlotCoord> item in readOnlyDictionary)
		{
			if (item.Value.Col > num2)
			{
				num2 = item.Value.Col;
			}
		}
		RectTransform rectTransform = (RectTransform)base.transform;
		RectTransform obj = ((_slotTransforms.Length != 0 && _slotTransforms[0] != null) ? ((RectTransform)_slotTransforms[0].parent) : rectTransform);
		Vector3 position = rectTransform.TransformPoint(new Vector3(rectTransform.rect.xMax, 0f, 0f));
		float num3 = obj.InverseTransformPoint(position).x + _gridOrigin.x - (float)num2 * _cellSize.x;
		Dictionary<int, Vector2> dictionary = new Dictionary<int, Vector2>(readOnlyDictionary.Count);
		foreach (KeyValuePair<int, SlotCoord> item2 in readOnlyDictionary)
		{
			int key = item2.Key;
			SlotCoord value = item2.Value;
			Vector2 anchoredPosition = (dictionary[key] = new Vector2(num3 + (float)value.Col * _cellSize.x, _gridOrigin.y - (float)value.Row * _cellSize.y));
			int num4 = SlotIndexForGear(key);
			if (num4 >= 0 && num4 < _slotTransforms.Length)
			{
				_slotTransforms[num4].gameObject.SetActive(value: true);
				_slotTransforms[num4].anchoredPosition = anchoredPosition;
				if (_slotLabels != null && num4 < _slotLabels.Length && _slotLabels[num4] != null)
				{
					_slotLabels[num4].gameObject.SetActive(value: true);
					_slotLabels[num4].anchoredPosition = ((value.Row == 0) ? new Vector2(0f, _slotLabelOffset) : new Vector2(0f, 0f - _slotLabelOffset));
				}
			}
		}
		_slotPixelPositions = dictionary;
		Dictionary<int, (float, float)> dictionary2 = new Dictionary<int, (float, float)>();
		foreach (KeyValuePair<int, Vector2> item3 in dictionary)
		{
			int key2 = Mathf.RoundToInt((item3.Value.x - num3) / _cellSize.x);
			if (!dictionary2.TryGetValue(key2, out var value2))
			{
				value2 = (item3.Value.y, item3.Value.y);
			}
			value2 = (dictionary2[key2] = (Mathf.Min(value2.Item1, item3.Value.y), Mathf.Max(value2.Item2, item3.Value.y)));
		}
		int num5 = int.MaxValue;
		foreach (int key4 in dictionary2.Keys)
		{
			if (key4 < num5)
			{
				num5 = key4;
			}
		}
		float num6 = _gridOrigin.y - _cellSize.y * 0.5f;
		float x = ((!readOnlyDictionary.TryGetValue(3, out var value3)) ? (num3 + (float)(num5 + num2) * 0.5f * _cellSize.x) : (num3 + (float)value3.Col * _cellSize.x));
		_neutralCenter = new Vector2(x, num6);
		foreach (KeyValuePair<int, (float, float)> item4 in dictionary2)
		{
			int key3 = item4.Key;
			if (key3 < _verticalChannels.Length)
			{
				float num7 = ((item4.Value.Item2 > num6) ? Mathf.Max(item4.Value.Item2 - _slotCircleRadius, num6) : num6);
				float num8 = ((item4.Value.Item1 < num6) ? Mathf.Min(item4.Value.Item1 + _slotCircleRadius, num6) : num6);
				Image obj2 = _verticalChannels[key3];
				obj2.gameObject.SetActive(value: true);
				RectTransform rectTransform2 = obj2.rectTransform;
				rectTransform2.sizeDelta = new Vector2(y: Mathf.Max(0.01f, num7 - num8), x: _channelThickness);
				rectTransform2.anchoredPosition = new Vector2(num3 + (float)key3 * _cellSize.x, (num8 + num7) * 0.5f);
			}
		}
		if ((bool)_horizontalChannel)
		{
			float x2 = num3 + (float)(num5 + num2) * 0.5f * _cellSize.x;
			_horizontalChannel.gameObject.SetActive(value: true);
			RectTransform rectTransform3 = _horizontalChannel.rectTransform;
			float b = (float)(num2 - num5) * _cellSize.x;
			rectTransform3.sizeDelta = new Vector2(Mathf.Max(0.01f, b), _channelThickness);
			rectTransform3.anchoredPosition = new Vector2(x2, num6);
		}
		_allowedZones.Clear();
		IReadOnlyList<Rect> collection = ShifterMath.BuildAllowedZones(dictionary, _cellSize.x, _cellSize.y, _channelThickness * 2.5f);
		_allowedZones.AddRange(collection);
		_lastAppliedHash = num;
		_layoutReady = true;
	}

	public Vector2 GetSlotAnchoredPos(int gear)
	{
		if (gear == 0)
		{
			return _neutralCenter;
		}
		if (_slotPixelPositions != null && _slotPixelPositions.TryGetValue(gear, out var value))
		{
			return value;
		}
		return Vector2.zero;
	}

	public bool IsSlotActive(int gear)
	{
		if (_slotPixelPositions != null)
		{
			return _slotPixelPositions.ContainsKey(gear);
		}
		return false;
	}

	public void SetKnobAnchoredPos(Vector2 pos)
	{
		if (!(_knob == null))
		{
			_knobTween?.Kill();
			_knob.anchoredPosition = pos;
		}
	}

	public void MoveKnobTo(Vector2 anchoredPos, float duration)
	{
		if (_knob == null)
		{
			return;
		}
		_knobTween?.Kill();
		if (duration <= 0f)
		{
			_knob.anchoredPosition = anchoredPos;
			return;
		}
		Vector2 anchoredPosition = _knob.anchoredPosition;
		Vector2 endValue = anchoredPos;
		if (Mathf.Abs(anchoredPosition.x - endValue.x) <= _cellSize.x * 0.5f)
		{
			_knobTween = _knob.DOAnchorPos(endValue, duration).SetEase(Ease.OutQuad);
			return;
		}
		float y = _neutralCenter.y;
		Vector2 endValue2 = new Vector2(anchoredPosition.x, y);
		Vector2 endValue3 = new Vector2(endValue.x, y);
		float num = Mathf.Abs(endValue2.y - anchoredPosition.y);
		float num2 = Mathf.Abs(endValue3.x - endValue2.x);
		float num3 = Mathf.Abs(endValue.y - endValue3.y);
		float num4 = Mathf.Max(0.0001f, num + num2 + num3);
		Sequence sequence = DOTween.Sequence();
		if (num > 0.01f)
		{
			sequence.Append(_knob.DOAnchorPos(endValue2, duration * num / num4).SetEase(Ease.Linear));
		}
		if (num2 > 0.01f)
		{
			sequence.Append(_knob.DOAnchorPos(endValue3, duration * num2 / num4).SetEase(Ease.Linear));
		}
		if (num3 > 0.01f)
		{
			sequence.Append(_knob.DOAnchorPos(endValue, duration * num3 / num4).SetEase(Ease.OutQuad));
		}
		_knobTween = sequence;
	}

	public void SetGearTraceAlpha(float a)
	{
		if ((bool)_gearTraceCanvasGroup)
		{
			_gearTraceCanvasGroup.alpha = Mathf.Clamp01(a);
		}
	}

	public void ScaleKnobGrabbed()
	{
		if (!(_knob == null))
		{
			_knobScaleTween?.Kill();
			_knobScaleTween = _knob.DOScale(_knobBaseScale * _knobGrabScale, _knobScaleDuration).SetEase(Ease.OutQuad);
		}
	}

	public void ScaleKnobReleased()
	{
		if (!(_knob == null))
		{
			_knobScaleTween?.Kill();
			_knobScaleTween = _knob.DOScale(_knobBaseScale, _knobScaleDuration).SetEase(Ease.OutQuad);
		}
	}

	public void RevealTrace()
	{
		if (!(_gearTraceCanvasGroup == null))
		{
			_traceFadeTween?.Kill();
			_traceFadeTween = _gearTraceCanvasGroup.DOFade(1f, _traceRevealDuration).SetEase(Ease.OutQuad);
		}
	}

	public void HideTraceWithDelay(float delay = -1f, float duration = -1f)
	{
		if (!(_gearTraceCanvasGroup == null))
		{
			_traceFadeTween?.Kill();
			float delay2 = ((delay >= 0f) ? delay : _traceHideDelay);
			float duration2 = ((duration > 0f) ? duration : _traceHideDuration);
			_traceFadeTween = _gearTraceCanvasGroup.DOFade(0.3f, duration2).SetEase(Ease.OutQuad).SetDelay(delay2);
		}
	}

	public void RevealAndHideWithin(float totalDuration)
	{
		if (!(_gearTraceCanvasGroup == null))
		{
			_traceFadeTween?.Kill();
			float num = totalDuration * 0.33f;
			float duration = totalDuration - num;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_gearTraceCanvasGroup.DOFade(1f, num).SetEase(Ease.OutQuad));
			sequence.Append(_gearTraceCanvasGroup.DOFade(0.3f, duration).SetEase(Ease.OutQuad));
			_traceFadeTween = sequence;
		}
	}

	private static int SlotIndexForGear(int gear)
	{
		switch (gear)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			return gear - 1;
		case -1:
			return 7;
		default:
			return -1;
		}
	}

	private static int HashCode(ShifterLayoutAsset l)
	{
		return ((17 * 31 + l.NumGears) * 31 + (int)l.ReversePos) * 31 + (l.DoglegFirstGear ? 1 : 0);
	}

	private void Awake()
	{
		CaptureKnobBaseScale();
		SteamUIParentChanger.OnChangesApplied += CaptureKnobBaseScale;
	}

	private void OnDestroy()
	{
		SteamUIParentChanger.OnChangesApplied -= CaptureKnobBaseScale;
	}

	private void CaptureKnobBaseScale()
	{
		if (_knob != null)
		{
			_knobBaseScale = _knob.localScale;
		}
	}

	private void OnDisable()
	{
		_knobTween?.Kill();
		_traceFadeTween?.Kill();
		_knobScaleTween?.Kill();
		if (_knob != null)
		{
			_knob.localScale = _knobBaseScale;
		}
	}
}
