using CPMEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.NewUI
{
	public class PneumaticCarControlUIPart : PhoneUISubPanel
	{
		private const float ChangeStepMeters = 0.001f;

		private const float RoomEpsilonMeters = 1E-05f;

		private const float HoldButtonDuration = 0.5f;

		private const float VibrationHoldDuration = 0.3f;

		private const float VibrateHoldIntensity = 0.03f;

		private const float VibrateClickIntensity = 0.2f;

		private const float VibrationClickDuration = 0.05f;

		private CarControlUIPanel _carControllerUIPanel;

		private uint _soundEvent;

		private uint _uiSwitch;

		private uint _suspensionAppLockSwitch;

		private uint _suspensionAppUnlockSwitch;

		[SerializeField]
		[Header("Texts")]
		private TextMeshProUGUI FrontPneumaticValueText;

		[SerializeField]
		private TextMeshProUGUI RearPneumaticValueText;

		[SerializeField]
		private TextMeshProUGUI BothPneumaticValueText;

		[SerializeField]
		[Header("Buttons")]
		private ButtonUpDown FrontUpButton;

		[SerializeField]
		private ButtonUpDown FrontDownButton;

		[SerializeField]
		private ButtonUpDown RearUpButton;

		[SerializeField]
		private ButtonUpDown RearDownButton;

		[SerializeField]
		private ButtonUpDown BothUpButton;

		[SerializeField]
		private ButtonUpDown BothDownButton;

		[SerializeField]
		private Toggle BothAxlesToggle;

		[SerializeField]
		private Button ResetButton;

		[SerializeField]
		[Header("Outlines")]
		private FillableImage[] BorderImages;

		[SerializeField]
		private Color ActiveColor;

		[SerializeField]
		private Color DeactiveColor;

		private float _commonDelta;

		private bool _bothAxlesMode;

		private bool? _frontUpOn;

		private bool? _frontDownOn;

		private bool? _rearUpOn;

		private bool? _rearDownOn;

		private bool? _bothUpOn;

		private bool? _bothDownOn;

		public void PublishDatas(CarControlUIPanel carControlUIPanel)
		{
			_carControllerUIPanel = carControlUIPanel;
			_commonDelta = 0f;
			RefreshFromCar();
		}

		private void RefreshFromCar()
		{
			SetTexts();
		}

		private float2 FrontLimits()
		{
			return AirSuspensionMath.WholeMillimetreLimits(_carControllerUIPanel.GetChassis.GetFrontAirBagLimit());
		}

		private float2 RearLimits()
		{
			return AirSuspensionMath.WholeMillimetreLimits(_carControllerUIPanel.GetChassis.GetRearAirBagLimit());
		}

		private static int ToMm(float meters)
		{
			return Mathf.FloorToInt(meters * 1000f + 0.5f);
		}

		private static string Mm(float meters)
		{
			return string.Format("{0}\u00a0{1}", ToMm(meters), new LocalizedString("Utils.mm"));
		}

		private static string SignedMm(float meters)
		{
			int num = ToMm(meters);
			return ((num > 0) ? "+" : ((num < 0) ? "−" : "")) + Mm(Mathf.Abs(meters));
		}

		private void SetTexts()
		{
			FrontPneumaticValueText.text = Mm(_carControllerUIPanel.GetChassis.GetFrontTargetHeight());
			RearPneumaticValueText.text = Mm(_carControllerUIPanel.GetChassis.GetRearTargetHeight());
			BothPneumaticValueText.text = SignedMm(_commonDelta);
			UpdateButtonsInteractable();
		}

		private void UpdateButtonsInteractable()
		{
			float2 limitsFront = FrontLimits();
			float2 limitsRear = RearLimits();
			float frontTargetHeight = _carControllerUIPanel.GetChassis.GetFrontTargetHeight();
			float rearTargetHeight = _carControllerUIPanel.GetChassis.GetRearTargetHeight();
			float2 float5 = AirSuspensionMath.CommonRoom(frontTargetHeight, limitsFront, rearTargetHeight, limitsRear);
			Apply(FrontUpButton, !_bothAxlesMode && limitsFront.y - frontTargetHeight > 1E-05f, ref _frontUpOn);
			Apply(FrontDownButton, !_bothAxlesMode && frontTargetHeight - limitsFront.x > 1E-05f, ref _frontDownOn);
			Apply(RearUpButton, !_bothAxlesMode && limitsRear.y - rearTargetHeight > 1E-05f, ref _rearUpOn);
			Apply(RearDownButton, !_bothAxlesMode && rearTargetHeight - limitsRear.x > 1E-05f, ref _rearDownOn);
			Apply(BothUpButton, _bothAxlesMode && float5.y > 1E-05f, ref _bothUpOn);
			Apply(BothDownButton, _bothAxlesMode && float5.x > 1E-05f, ref _bothDownOn);
		}

		private static void Apply(ButtonUpDown button, bool state, ref bool? applied)
		{
			if (applied != state)
			{
				applied = state;
				button.SetInteractable(state);
			}
		}

		private void Start()
		{
			_soundEvent = AudioWrapper.GetId("Ui_Event");
			_uiSwitch = AudioWrapper.GetId("UiSwitch");
			_suspensionAppLockSwitch = AudioWrapper.GetId("UI_Phone_AirSuspensionApp_Lock");
			_suspensionAppUnlockSwitch = AudioWrapper.GetId("UI_Phone_AirSuspensionApp_UnLock");
			BothAxlesToggle.onValueChanged.AddListener(SelectBothAxles);
		}

		public override void OnEnableEvent()
		{
			base.OnEnableEvent();
			SetListeners();
		}

		public override void Show()
		{
			base.Show();
			if (!_carControllerUIPanel.GetVehicleData.carInfo.AdditionalData.AirSuspension.Bought || !_carControllerUIPanel.GetVehicleData.carInfo.AdditionalData.AirSuspension.Installed)
			{
				_carControllerUIPanel.ShowNotAvaliblePanel(new LocalizedString("Phone.PneumaticNotAvalible"));
				CanvasGroup.interactable = false;
				ResetButton.interactable = false;
				return;
			}
			if (_carControllerUIPanel.GetVehicleData.GetPowertrain.SpeedKmh > 5f)
			{
				_carControllerUIPanel.ShowNotAvaliblePanel(new LocalizedString("Phone.PneumaticNotAvalibleSlowDown"));
				CanvasGroup.interactable = false;
				ResetButton.interactable = false;
				return;
			}
			_bothAxlesMode = false;
			BothAxlesToggle.SetIsOnWithoutNotify(value: false);
			CanvasGroup.interactable = true;
			ResetButton.interactable = true;
			ActivateOutline(state: false);
			SetModeColors();
			RefreshFromCar();
		}

		private void SetListeners()
		{
			FrontUpButton.AddClickListener(() =>
			{
				ChangeFrontValueSingle(isUp: true);
			});
			FrontUpButton.AddHoldButtonListener(() =>
			{
				ChangeFrontDownUpValue(isUp: true);
			});
			FrontDownButton.AddClickListener(() =>
			{
				ChangeFrontValueSingle(isUp: false);
			});
			FrontDownButton.AddHoldButtonListener(() =>
			{
				ChangeFrontDownUpValue(isUp: false);
			});
			RearUpButton.AddClickListener(() =>
			{
				ChangeRearValueSingle(isUp: true);
			});
			RearUpButton.AddHoldButtonListener(() =>
			{
				ChangeRearUpDownValue(isUp: true);
			});
			RearDownButton.AddClickListener(() =>
			{
				ChangeRearValueSingle(isUp: false);
			});
			RearDownButton.AddHoldButtonListener(() =>
			{
				ChangeRearUpDownValue(isUp: false);
			});
			BothUpButton.AddClickListener(() =>
			{
				ChangeBothValueSingle(isUp: true);
			});
			BothUpButton.AddHoldButtonListener(() =>
			{
				ChangeBothUpDownValue(isUp: true);
			});
			BothDownButton.AddClickListener(() =>
			{
				ChangeBothValueSingle(isUp: false);
			});
			BothDownButton.AddHoldButtonListener(() =>
			{
				ChangeBothUpDownValue(isUp: false);
			});
			ResetButton.transform.parent.gameObject.SetActive(value: true);
			ResetButton.onClick.AddListener(ResetValues);
		}

		private void SetModeColors()
		{
			Color color = (_bothAxlesMode ? Color.gray : Color.white);
			FrontPneumaticValueText.color = color;
			RearPneumaticValueText.color = color;
			BothPneumaticValueText.color = (_bothAxlesMode ? Color.white : Color.gray);
		}

		private void ResetValues()
		{
			_carControllerUIPanel.GetChassis.SetFrontTargetHeight(_carControllerUIPanel.GetChassis.GetFrontStockClearance());
			_carControllerUIPanel.GetChassis.SetRearTargetHeight(_carControllerUIPanel.GetChassis.GetRearStockClearance());
			_commonDelta = 0f;
			SetTexts();
		}

		private void Vibrate(ButtonUpDown buttonUpDown)
		{
			if (!buttonUpDown.IsVibrated)
			{
				Utils.Vibrate(0.3f, 0.03f);
				buttonUpDown.IsVibrated = true;
			}
		}

		private void ChangeFrontValueSingle(bool isUp)
		{
			if (ChangeFront(isUp, 0.001f))
			{
				Utils.Vibrate(0.05f, 0.2f);
			}
		}

		private void ChangeRearValueSingle(bool isUp)
		{
			if (ChangeRear(isUp, 0.001f))
			{
				Utils.Vibrate(0.05f, 0.2f);
			}
		}

		private void ChangeBothValueSingle(bool isUp)
		{
			if (ChangeBoth(isUp, 0.001f))
			{
				Utils.Vibrate(0.05f, 0.2f);
			}
		}

		private bool ChangeFront(bool isUp, float step)
		{
			float2 float5 = FrontLimits();
			float frontTargetHeight = _carControllerUIPanel.GetChassis.GetFrontTargetHeight();
			float num = Mathf.Clamp(frontTargetHeight + (isUp ? step : (0f - step)), float5.x, float5.y);
			bool num2 = !Mathf.Approximately(num, frontTargetHeight);
			if (num2)
			{
				_carControllerUIPanel.GetChassis.SetFrontTargetHeight(num);
			}
			SetTexts();
			return num2;
		}

		private bool ChangeRear(bool isUp, float step)
		{
			float2 float5 = RearLimits();
			float rearTargetHeight = _carControllerUIPanel.GetChassis.GetRearTargetHeight();
			float num = Mathf.Clamp(rearTargetHeight + (isUp ? step : (0f - step)), float5.x, float5.y);
			bool num2 = !Mathf.Approximately(num, rearTargetHeight);
			if (num2)
			{
				_carControllerUIPanel.GetChassis.SetRearTargetHeight(num);
			}
			SetTexts();
			return num2;
		}

		private bool ChangeBoth(bool isUp, float step)
		{
			float2 limitsFront = FrontLimits();
			float2 limitsRear = RearLimits();
			float frontTargetHeight = _carControllerUIPanel.GetChassis.GetFrontTargetHeight();
			float rearTargetHeight = _carControllerUIPanel.GetChassis.GetRearTargetHeight();
			float num = AirSuspensionMath.ClampCommonDelta(isUp ? step : (0f - step), frontTargetHeight, limitsFront, rearTargetHeight, limitsRear);
			if (Mathf.Approximately(num, 0f))
			{
				return false;
			}
			_carControllerUIPanel.GetChassis.SetFrontTargetHeight(frontTargetHeight + num);
			_carControllerUIPanel.GetChassis.SetRearTargetHeight(rearTargetHeight + num);
			_commonDelta += num;
			SetTexts();
			return true;
		}

		private void ChangeFrontDownUpValue(bool isUp)
		{
			bool flag = FrontUpButton.PressTime <= 0.5f;
			bool flag2 = FrontDownButton.PressTime <= 0.5f;
			if (!(isUp & flag) && !(!isUp & flag2))
			{
				ButtonUpDown buttonUpDown = (isUp ? FrontUpButton : FrontDownButton);
				if (ChangeFront(isUp, _carControllerUIPanel.GetChassis.GetHeightAdjustSpeed() * Time.deltaTime))
				{
					Vibrate(buttonUpDown);
				}
			}
		}

		private void ChangeRearUpDownValue(bool isUp)
		{
			bool flag = RearUpButton.PressTime <= 0.5f;
			bool flag2 = RearDownButton.PressTime <= 0.5f;
			if (!(isUp & flag) && !(!isUp & flag2))
			{
				bool num = ChangeRear(isUp, _carControllerUIPanel.GetChassis.GetHeightAdjustSpeed() * Time.deltaTime);
				ButtonUpDown buttonUpDown = (isUp ? RearUpButton : RearDownButton);
				if (num)
				{
					Vibrate(buttonUpDown);
				}
			}
		}

		private void ChangeBothUpDownValue(bool isUp)
		{
			if (!((isUp ? BothUpButton.PressTime : BothDownButton.PressTime) <= 0.5f) && ChangeBoth(isUp, _carControllerUIPanel.GetChassis.GetHeightAdjustSpeed() * Time.deltaTime))
			{
				Vibrate(isUp ? BothUpButton : BothDownButton);
			}
		}

		private void ActivateOutline(bool state)
		{
			for (int i = 0; i < BorderImages.Length; i++)
			{
				if (state)
				{
					BorderImages[i].StartFilling();
				}
				else
				{
					BorderImages[i].EndFilling();
				}
			}
		}

		private void SelectBothAxles(bool state)
		{
			_bothAxlesMode = state;
			if (state)
			{
				_commonDelta = 0f;
			}
			SetModeColors();
			ActivateOutline(state);
			SetTexts();
			AudioWrapper.SetSwitch(_uiSwitch, state ? _suspensionAppLockSwitch : _suspensionAppUnlockSwitch, base.gameObject);
			AudioWrapper.PostEvent(_soundEvent, base.gameObject);
			Utils.Vibrate(0.05f, 0.2f);
		}

		private void OnDisable()
		{
			FrontUpButton.RemoveAllListeners();
			RearUpButton.RemoveAllListeners();
			BothUpButton.RemoveAllListeners();
			FrontDownButton.RemoveAllListeners();
			RearDownButton.RemoveAllListeners();
			BothDownButton.RemoveAllListeners();
			ResetButton.onClick.RemoveAllListeners();
			ResetButton.transform.parent.gameObject.SetActive(value: false);
		}
	}
}
