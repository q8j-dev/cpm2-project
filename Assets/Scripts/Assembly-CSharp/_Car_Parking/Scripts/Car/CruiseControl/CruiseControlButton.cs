using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.UIElement.HUD;

namespace _Car_Parking.Scripts.Car.CruiseControl
{
	public class CruiseControlButton : MonoBehaviour
	{
		public Button EnableButton;

		public Button PlusButton;

		public Button MinusButton;

		public TextMeshProUGUI LockedSpeedText;

		public Color EnabledColor = Color.yellow;

		public bool ForCircularMenu;

		private CruiseControl _cruiseControl;

		private HudColorChanger _hudColorChanger;

		[Inject]
		public void Construct(CruiseControl cruiseControl, HudColorChanger hudColorChanger)
		{
			_hudColorChanger = hudColorChanger;
			_cruiseControl = cruiseControl;
		}

		private void Awake()
		{
			if (!ForCircularMenu)
			{
				EnableButton.onClick.AddListener(_cruiseControl.EnableToggle);
				PlusButton.onClick.AddListener(_cruiseControl.Plus);
				MinusButton.onClick.AddListener(_cruiseControl.Minus);
			}
		}

		private void Start()
		{
			InitColorChange();
		}

		private void InitColorChange()
		{
			_cruiseControl.IsEnabled.Subscribe((bool x) =>
			{
				EnableActiveCruiseUI(x);
				ChangeColor(x);
			}).AddTo(this);
			_cruiseControl.LockedSpeed.Subscribe((int x) =>
			{
				TextMeshProUGUI lockedSpeedText = LockedSpeedText;
				int num = x;
				lockedSpeedText.text = num.ToString() ?? "";
			}).AddTo(this);
		}

		private void EnableActiveCruiseUI(bool enable)
		{
			LockedSpeedText.SetActive(enable);
		}

		private void ChangeColor(bool isEnabled)
		{
			if ((bool)EnableButton && (bool)EnableButton.image)
			{
				Color color = (isEnabled ? EnabledColor : _hudColorChanger.CurrentColor);
				color.a = EnableButton.image.color.a;
				EnableButton.image.color = color;
			}
		}
	}
}
