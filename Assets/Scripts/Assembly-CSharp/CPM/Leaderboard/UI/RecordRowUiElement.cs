using System;
using CPM.RacingRecords;
using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Racing_Scripts;

namespace CPM.Leaderboard.UI
{
	public sealed class RecordRowUiElement : MonoBehaviour
	{
		[SerializeField]
		private Toggle _toggle = default;

		[SerializeField]
		private TextMeshProUGUI _placeText = default;

		[SerializeField]
		private Text _nameText = default;

		[SerializeField]
		private TextMeshProUGUI _timeText = default;

		[SerializeField]
		private TextMeshProUGUI _carClassText = default;

		[SerializeField]
		private TextMeshProUGUI _weatherText = default;

		[SerializeField]
		private GameObject _ownHighlight = default;

		[SerializeField]
		private GameObject _selectedHighlight = default;

		[SerializeField]
		private Image _weatherIcon = default;

		[SerializeField]
		private Sprite _sunSprite = default;

		[SerializeField]
		private Sprite _cloudSprite = default;

		[SerializeField]
		private Sprite _snowSprite = default;

		private RacingRecord _record;

		private Action<RacingRecord> _onSelected;

		private Action _onSelectedLocal;

		private bool _isSelected;

		public RacingRecord Record => _record;

		public void Bind(RacingRecord record, int displayRank, bool isOwn, Action<RacingRecord> onSelected)
		{
			_record = record;
			if (record != null)
			{
				_onSelected = onSelected;
				_onSelectedLocal = null;
				if ((bool)_placeText)
				{
					_placeText.text = "#" + displayRank;
				}
				if ((bool)_nameText)
				{
					_nameText.text = (isOwn ? "You" : record.Nickname);
				}
				if ((bool)_carClassText)
				{
					TextMeshProUGUI carClassText = _carClassText;
					VehicleClass carClass = (VehicleClass)record.CarClass;
					carClassText.text = carClass.ToString();
				}
				if ((bool)_timeText)
				{
					_timeText.text = FormatTime(record.Score);
				}
				if ((bool)_ownHighlight)
				{
					_ownHighlight.SetActive(isOwn);
				}
				SetWeather(record.Rain);
				WireToggle();
			}
		}

		public void BindLocalBest(long timeMs, Action onSelected, int provisionalRank = 0)
		{
			_record = null;
			_onSelected = null;
			_onSelectedLocal = onSelected;
			if ((bool)_placeText)
			{
				_placeText.text = ((provisionalRank > 0) ? ("#" + provisionalRank) : ">250");
			}
			if ((bool)_carClassText)
			{
				_carClassText.text = string.Empty;
			}
			if ((bool)_nameText)
			{
				_nameText.text = "You";
			}
			if ((bool)_timeText)
			{
				_timeText.text = FormatTime(timeMs);
			}
			if ((bool)_ownHighlight)
			{
				_ownHighlight.SetActive(value: true);
			}
			HideWeather();
			WireToggle();
		}

		private void WireToggle()
		{
			if ((bool)_toggle)
			{
				_toggle.group = null;
				_toggle.SetIsOnWithoutNotify(value: false);
				_toggle.onValueChanged.RemoveListener(OnToggleChanged);
				_toggle.onValueChanged.AddListener(OnToggleChanged);
			}
			_isSelected = false;
			if ((bool)_selectedHighlight)
			{
				_selectedHighlight.SetActive(value: false);
			}
		}

		public void SetSelected(bool selected)
		{
			_isSelected = selected;
			if ((bool)_toggle)
			{
				_toggle.SetIsOnWithoutNotify(selected);
			}
			if ((bool)_selectedHighlight)
			{
				_selectedHighlight.SetActive(selected);
			}
		}

		public void SetSelectable(bool on)
		{
			if ((bool)_toggle)
			{
				_toggle.interactable = on;
			}
		}

		public void HideWeather()
		{
			if ((bool)_weatherIcon)
			{
				_weatherIcon.enabled = false;
			}
		}

		private void SetWeather(bool rain)
		{
			if ((bool)_weatherIcon)
			{
				_weatherIcon.enabled = true;
				if ((bool)_weatherText)
				{
					_weatherText.text = $"{(int)RallyController.DayTimeType}:00 |";
				}
				Sprite sprite = _sunSprite;
				switch (RallyController.WeatherType)
				{
				case WeatherType.Rain:
					sprite = _cloudSprite;
					break;
				case WeatherType.Snow:
					sprite = _snowSprite;
					break;
				}
				_weatherIcon.sprite = sprite;
			}
		}

		private void OnToggleChanged(bool isOn)
		{
			if (isOn)
			{
				if (_record != null)
				{
					_onSelected?.Invoke(_record);
				}
				else
				{
					_onSelectedLocal?.Invoke();
				}
			}
			else if (_isSelected && (bool)_toggle)
			{
				_toggle.SetIsOnWithoutNotify(value: true);
			}
		}

		private void OnDestroy()
		{
			if ((bool)_toggle)
			{
				_toggle.onValueChanged.RemoveListener(OnToggleChanged);
			}
		}

		private static string FormatTime(long ms)
		{
			int num = (int)(ms / 1000);
			int num2 = num / 60;
			int num3 = num % 60;
			int num4 = (int)(ms % 1000 / 100);
			return $"{num2}:{num3:00}.{num4}";
		}
	}
}
