using System;
using CPMEngine;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.PhotoMode.UI;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.PhotoMode
{
	public class PhotoModeScroll : MonoBehaviour
	{
		public const float HapticDuration = 0.05f;

		public const float HapticIntensity = 0.1f;

		[SerializeField]
		private SimpleScrollSnap ScrollSnap;

		[SerializeField]
		private Transform Content;

		[SerializeField]
		private PhotoModeScrollItemWithText FirstItem;

		[SerializeField]
		private PhotoModeScrollItemWithText LastItem;

		[SerializeField]
		private TextMeshProUGUI CurrentValueText;

		[SerializeField]
		private string[] Texts;

		private bool _isInitialized;

		private bool _isNormalizedPosSet;

		private float _maxValue;

		private float _minValue;

		private int _startPanel;

		private int _itemsCount;

		private int _lastCenteredPanel = -1;

		private float _previousScrollValue = -1f;

		private float _prevCenter;

		[field: SerializeField]
		public ClickDetector ClickDetector { get; private set; }

		public float CurrentValue { get; private set; }

		public string CurrentText { get; private set; }

		public int LastCenteredIndex => _lastCenteredPanel;

		public event Action<float> OnValueChanged;

		public event Action<int> OnValueCentering;

		public void GoToPanel(int panel)
		{
			int panelNumber = Mathf.Clamp(panel, 0, _itemsCount - 1);
			ScrollSnap.GoToPanel(panelNumber);
		}

		private void Start()
		{
			ScrollSnap.ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
			ScrollSnap.OnPanelCentering.AddListener(OnCentering);
			ScrollSnap.OnPanelCentered.AddListener(OnCentered);
		}

		private void OnDestroy()
		{
			if ((bool)ScrollSnap.ScrollRect)
			{
				ScrollSnap.ScrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
			}
		}

		private void OnEnable()
		{
			if (_isInitialized && !_isNormalizedPosSet)
			{
				SetScrollNormalizedPos(_startPanel);
			}
		}

		public void Init(PhotoModeScrollDefaultValues values, int startPanel)
		{
			if (!_isInitialized)
			{
				_startPanel = startPanel;
			}
			_isInitialized = true;
			_minValue = values.Min;
			_maxValue = values.Max;
			_itemsCount = values.ItemsCount;
			if (_isNormalizedPosSet)
			{
				ScrollSnap.GoToPanel(startPanel);
			}
			SetText(values.DefaultItemIndex);
			_lastCenteredPanel = startPanel;
			_previousScrollValue = (float)startPanel / (float)(_itemsCount - 1);
			_prevCenter = startPanel;
		}

		public void ResetText()
		{
			SetText(_lastCenteredPanel);
		}

		public float GetScrollValue(int panel)
		{
			float t = 1f / (float)(_itemsCount - 1) * (float)panel;
			CurrentValue = Mathf.Lerp(_minValue, _maxValue, t);
			return CurrentValue;
		}

		private void SetScrollNormalizedPos(int startPanel)
		{
			_isNormalizedPosSet = true;
			ScrollSnap.AutoSnap = false;
			Observable.TimerFrame(1).Subscribe((long _) =>
			{
				ScrollSnap.ScrollRect.HorizontalNormalizedPosByElementsCount(startPanel, _itemsCount);
			}).AddTo(this);
			Observable.TimerFrame(2).Subscribe((long _) =>
			{
				ScrollSnap.AutoSnap = true;
				ScrollSnap.GoToPanel(startPanel);
			}).AddTo(this);
		}

		private void OnScrollValueChanged(Vector2 scrollValue)
		{
			int closestPanelToCenter = GetClosestPanelToCenter(scrollValue);
			if (closestPanelToCenter != _lastCenteredPanel)
			{
				SetText(closestPanelToCenter);
				_lastCenteredPanel = closestPanelToCenter;
			}
			OnValueCentering?.Invoke(_lastCenteredPanel);
			SetFirstLastItemsTextAlpha(scrollValue.x);
			CheckCenterCrossing(scrollValue.x);
			_previousScrollValue = scrollValue.x;
		}

		private void SetFirstLastItemsTextAlpha(float scrollPos)
		{
			if (scrollPos < 0.5f)
			{
				float textColorAlpha = Mathf.InverseLerp(0.2f, 0.4f, scrollPos);
				FirstItem.SetTextColorAlpha(textColorAlpha);
			}
			else
			{
				float textColorAlpha2 = Mathf.InverseLerp(0.8f, 0.6f, scrollPos);
				LastItem.SetTextColorAlpha(textColorAlpha2);
			}
		}

		private int GetClosestPanelToCenter(Vector2 scrollValue)
		{
			if (_itemsCount <= 1)
			{
				return 0;
			}
			float x = scrollValue.x;
			CurrentValue = Mathf.Lerp(_minValue, _maxValue, x);
			OnValueChanged?.Invoke(CurrentValue);
			return Mathf.Clamp(Mathf.RoundToInt(x * (float)(_itemsCount - 1)), 0, _itemsCount - 1);
		}

		private void OnCentering(int centeringPanel, int selectedPanel)
		{
			SetText(centeringPanel);
			_lastCenteredPanel = centeringPanel;
		}

		private void OnCentered(int centeredPanel, int selectedPanel)
		{
			SetText(centeredPanel);
			_lastCenteredPanel = centeredPanel;
		}

		private void SetText(int currentPanel)
		{
			if (Texts != null && currentPanel >= 0 && currentPanel < Texts.Length)
			{
				CurrentText = Texts[currentPanel];
				CurrentValueText.text = CurrentText;
			}
			else
			{
				CurrentText = CurrentValue.ToString("F2");
				CurrentValueText.text = CurrentText;
			}
		}

		private void CheckCenterCrossing(float currentScrollValue)
		{
			if (_previousScrollValue < 0f)
			{
				return;
			}
			float num = currentScrollValue * (float)(_itemsCount - 1);
			float num2 = _previousScrollValue * (float)(_itemsCount - 1);
			for (int i = 0; i < _itemsCount; i++)
			{
				float num3 = i;
				bool num4 = (num2 < num3 && num >= num3) || (num2 > num3 && num <= num3);
				bool flag = (_prevCenter == 0f && Mathf.Approximately(_prevCenter, num3)) || (Mathf.Approximately(_prevCenter, _itemsCount - 1) && Mathf.Approximately(_prevCenter, num3));
				if (num4 && !flag)
				{
					Utils.Vibrate(0.05f, 0.1f, unscaledTime: true);
					PlaySound();
					_prevCenter = num3;
					break;
				}
			}
		}

		private void PlaySound()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingValueSwitch, base.gameObject);
		}
	}
}
