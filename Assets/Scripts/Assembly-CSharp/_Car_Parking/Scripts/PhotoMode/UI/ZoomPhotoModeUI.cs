using CPMEngine;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class ZoomPhotoModeUI : MonoBehaviour
	{
		[SerializeField]
		private RectTransform ScrollParentRect;

		[SerializeField]
		private RectTransform TouchDetectorRect;

		[SerializeField]
		private RectMask2D ScrollMask;

		[SerializeField]
		private ScrollRect ZoomScroll;

		[SerializeField]
		private SimpleScrollSnap ScrollSnap;

		[SerializeField]
		private TextMeshProUGUI ZoomValue;

		[SerializeField]
		private PhotoModePrototype PhotoModePrototype;

		[SerializeField]
		private Button ResetButton;

		[SerializeField]
		private TouchDetector TouchDetector;

		[SerializeField]
		private float Tolerance = 0.01f;

		[SerializeField]
		private string[] Texts;

		private readonly int _startPanel = 18;

		private readonly int _itemsCount = 23;

		private int _currentSnapIndex = -1;

		private bool _wasInCenter;

		private bool _isNormalizedPosSet;

		private float _previousScrollValue = -1f;

		private IZoom _zoom;

		private void Start()
		{
			ScrollSnap.OnPanelCentered.AddListener(OnPanelCentered);
			SetScrollNormalizedPos();
			PhotoModePrototype.CameraZoom = 60f;
			PhotoModePrototype.OnZoomByTouch += SetScrollPosition;
			TouchDetector.OnZoomStateChanged += OnTouchZoomStateChanged;
			_zoom = new ZoomPhotoMode();
			_currentSnapIndex = _startPanel;
			_wasInCenter = true;
			_previousScrollValue = (float)_startPanel / (float)(_itemsCount - 1);
			UpdateZoomForIndex(_currentSnapIndex);
			ZoomScroll.onValueChanged.AddListener(OnScroll);
			ResetButton.onClick.AddListener(ResetScrollToDefault);
			CalculateTouchDetectorSize();
			CalculateMask();
		}

		private void OnDestroy()
		{
			PhotoModePrototype.OnZoomByTouch -= SetScrollPosition;
			TouchDetector.OnZoomStateChanged -= OnTouchZoomStateChanged;
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnPanelCentered.RemoveListener(OnPanelCentered);
			}
			if ((bool)ZoomScroll)
			{
				ZoomScroll.onValueChanged.RemoveListener(OnScroll);
			}
		}

		private void SetScrollNormalizedPos()
		{
			ScrollSnap.AutoSnap = false;
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				ZoomScroll.VerticalNormalizedPosByElementsCount(_startPanel, _itemsCount, reverse: true);
			}).AddTo(this);
			Observable.TimerFrame(2).Subscribe((long _) =>
			{
				_isNormalizedPosSet = true;
				ScrollSnap.AutoSnap = true;
				ResetScrollToDefault();
			}).AddTo(this);
		}

		private void OnTouchZoomStateChanged(bool isZooming)
		{
			ScrollSnap.AutoSnap = !isZooming;
		}

		private void CalculateMask()
		{
			float num = ScrollParentRect.rect.height / 2f;
			ScrollMask.padding = new Vector4(0f, 0f - num, 0f, 0f - num);
			ScrollMask.softness = new Vector2Int(0, (int)num);
		}

		private void CalculateTouchDetectorSize()
		{
			float x = TouchDetectorRect.sizeDelta.x;
			float height = ScrollParentRect.rect.height;
			TouchDetectorRect.sizeDelta = new Vector2(x, height);
		}

		private void ResetScrollToDefault()
		{
			ScrollSnap.GoToPanel(_startPanel);
		}

		private void PlaySound()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeZoomSwitch, base.gameObject);
		}

		private void OnPanelCentered(int centered, int selected)
		{
			CheckForResetInteractable(centered);
			if (_currentSnapIndex != centered)
			{
				_currentSnapIndex = centered;
				UpdateZoomForIndex(centered);
			}
		}

		private void CheckForResetInteractable(int centered)
		{
			ResetButton.interactable = centered != _startPanel;
		}

		private void UpdateZoomForIndex(int index)
		{
			float num = (float)index / (float)(_itemsCount - 1);
			_zoom.CalculateDistanceFree(num);
			PhotoModePrototype.CameraZoom = _zoom.ZoomDistance;
			SetText(num);
		}

		private void OnScroll(Vector2 value)
		{
			_zoom.CalculateDistanceFree(value.y);
			PhotoModePrototype.CameraZoom = _zoom.ZoomDistance;
			SetText(value.y);
			if (_isNormalizedPosSet)
			{
				CheckPreciseCentering(value.y);
				CheckCenterCrossing(value.y);
			}
			_previousScrollValue = value.y;
		}

		private void CheckPreciseCentering(float scrollValue)
		{
			float num = scrollValue * (float)(_itemsCount - 1);
			int num2 = Mathf.RoundToInt(num);
			bool flag = Mathf.Abs(num - (float)num2) <= Tolerance;
			if (flag && !_wasInCenter)
			{
				_currentSnapIndex = num2;
				Utils.Vibrate(0.05f, 0.1f, unscaledTime: true);
				PlaySound();
			}
			_wasInCenter = flag;
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
				if ((num2 < num3 && num >= num3) || (num2 > num3 && num <= num3))
				{
					Utils.Vibrate(0.05f, 0.1f, unscaledTime: true);
					PlaySound();
					break;
				}
			}
		}

		private int GetCurrentCenterIndex(float scrollValue)
		{
			return Mathf.RoundToInt(scrollValue * (float)(_itemsCount - 1));
		}

		private void SetText(float value)
		{
			string text = Texts[GetCurrentCenterIndex(value)];
			ZoomValue.text = text;
		}

		public void SetScrollPosition(float zoomDistance)
		{
			float num = Mathf.InverseLerp(6f, 100f, zoomDistance);
			float verticalNormalizedPosition = 1f - num;
			ZoomScroll.verticalNormalizedPosition = verticalNormalizedPosition;
			ScrollSnap.GoToPanel(GetCurrentCenterIndex(num));
		}
	}
}
