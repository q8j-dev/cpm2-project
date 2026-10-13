using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[CreateAssetMenu(menuName = "CPM/Steam/Desktop Controls Settings", fileName = "_DesktopControlsSettings")]
	public class DesktopControlsSettingsSO : ScriptableObject
	{
		private const string SWsensitivityKey = "SWsensitivity";

		private const string CameraPrefix = "SteamControls.Camera.";

		[SerializeField]
		private CameraSettingsBlock Cameras = new CameraSettingsBlock();

		[SerializeField]
		private DeviceControls MouseKeyboard = new DeviceControls
		{
			CameraMultiplier = new ControlSettingVector2
			{
				Default = new Vector2(0.025f, 0.05f),
				Max = new Vector2(0.2f, 0.2f)
			},
			ZoomSpeed = new ControlSetting
			{
				Min = 1f,
				Max = 100f,
				Default = 25f
			}
		};

		[SerializeField]
		private DeviceControls Gamepad = new DeviceControls
		{
			CameraMultiplier = new ControlSettingVector2
			{
				Default = new Vector2(100f, 200f),
				Max = new Vector2(400f, 400f)
			},
			ZoomSpeed = new ControlSetting
			{
				Min = 1f,
				Max = 100f,
				Default = 25f
			}
		};

		[SerializeField]
		private DeviceControls Wheel = new DeviceControls
		{
			CameraMultiplier = new ControlSettingVector2
			{
				Default = new Vector2(100f, 200f),
				Max = new Vector2(400f, 400f)
			},
			ZoomSpeed = new ControlSetting
			{
				Min = 1f,
				Max = 100f,
				Default = 25f
			}
		};

		[SerializeField]
		private GamepadControlsBlock GamepadOnly = new GamepadControlsBlock();

		[SerializeField]
		private WheelControlsBlock WheelOnly = new WheelControlsBlock();

		[SerializeField]
		private MouseKeyboardControlsBlock MouseKeyboardOnly = new MouseKeyboardControlsBlock();

		[SerializeField]
		private ControlToggleSetting CasualMode = new ControlToggleSetting();

		private List<(ControlSetting setting, string key)> _floatIndex;

		private List<(ControlToggleSetting toggle, string key)> _boolIndex;

		private List<(ControlSettingVector2 look, string keyX, string keyY)> _vec2Index;

		public float WheelRotationRange => WheelOnly.RotationRange.Current;

		public bool WheelRangeOverrideEnabled => WheelOnly.RangeOverrideEnabled.Current;

		public float WheelFfbMultiplier => WheelOnly.FfbMultiplier.Current;

		public float MouseSteerMaxDisplacement => MouseKeyboardOnly.MouseSteerMaxDisplacement.Current;

		public float MouseSteerReturnRate => MouseKeyboardOnly.MouseSteerReturnRate;

		public float UiScrollDeltaPerTick => MouseKeyboardOnly.UiScrollDeltaPerTick;

		public float CursorSensitivity => GamepadOnly.CursorSensitivity.Current;

		public bool VibrationEnabled => GamepadOnly.VibrationEnabled.Current;

		public float VibrationIntensity => GamepadOnly.VibrationIntensity.Current;

		public event Action<float> WheelRangeChanged;

		public event Action<bool> WheelRangeOverrideChanged;

		public event Action<float> WheelFfbMultiplierChanged;

		public event Action<bool> CasualModeChanged;

		private DeviceControls Device(ControlDevice device)
		{
			return device switch
			{
				ControlDevice.Gamepad => Gamepad, 
				ControlDevice.Wheel => Wheel, 
				_ => MouseKeyboard, 
			};
		}

		public void Load()
		{
			BuildIndex();
			foreach (var item8 in _floatIndex)
			{
				ControlSetting item = item8.setting;
				string item2 = item8.key;
				item.Current = PlayerPrefs.GetFloat(item2, item.Default);
			}
			foreach (var item9 in _boolIndex)
			{
				ControlToggleSetting item3 = item9.toggle;
				string item4 = item9.key;
				item3.Current = PrefsGetBool(item4, item3.Default);
			}
			foreach (var item10 in _vec2Index)
			{
				ControlSettingVector2 item5 = item10.look;
				string item6 = item10.keyX;
				string item7 = item10.keyY;
				item5.Current = new Vector2(PlayerPrefs.GetFloat(item6, item5.Default.x), PlayerPrefs.GetFloat(item7, item5.Default.y));
			}
			PushSteeringToV(ControlDevice.MouseKeyboard);
			CasualMode.Current = FlagsStorage.IsCasualControl;
		}

		public Vector2 GetLookScale(CameraSettingsContext ctx)
		{
			if (ctx == CameraSettingsContext.Person)
			{
				return Cameras.Person.Look.Current;
			}
			return Cameras.Camera(ctx)?.Look.Current ?? Vector2.one;
		}

		public ControlSetting GetPersonTurnSlider()
		{
			return Cameras.Person.TurnSpeed;
		}

		public float GetPersonTurnSpeed()
		{
			return Cameras.Person.TurnSpeed.Current;
		}

		public void SetPersonTurnSpeed(float value)
		{
			Cameras.Person.TurnSpeed.Current = value;
			Save(Cameras.Person.TurnSpeed);
		}

		public bool GetPersonInvertTurn()
		{
			return Cameras.Person.InvertTurn.Current;
		}

		public (bool x, bool y, bool zoom) GetInvert(bool gamepad)
		{
			ControlSettingVector2 cameraMultiplier = Device(ToDevice(gamepad)).CameraMultiplier;
			return (x: cameraMultiplier.InvertX.Current, y: cameraMultiplier.InvertY.Current, zoom: false);
		}

		public Vector2 GetCameraMultiplier(bool gamepad)
		{
			return Device(ToDevice(gamepad)).CameraMultiplier.Current;
		}

		public float GetZoomSpeedScalar(bool gamepad)
		{
			return Device(ToDevice(gamepad)).ZoomSpeed.Current;
		}

		public bool GetZoomInvert(bool gamepad)
		{
			return Device(ToDevice(gamepad)).ZoomInvert.Current;
		}

		public bool InvertSteering(bool gamepad)
		{
			return Device(ToDevice(gamepad)).CarControls.InvertSteering.Current;
		}

		public (float moment, float multiplier) GetWheelFfbBase(bool moza)
		{
			if (!moza)
			{
				return (moment: WheelOnly.LogitechBaseMoment, multiplier: WheelOnly.LogitechBaseMultiplier);
			}
			return (moment: WheelOnly.MozaBaseMoment, multiplier: WheelOnly.MozaBaseMultiplier);
		}

		public float StickDeadzone(bool left)
		{
			return (left ? GamepadOnly.StickDeadzoneLeft : GamepadOnly.StickDeadzoneRight).Current;
		}

		public float ShifterKnobScale(bool gamepad)
		{
			return Device(ToDevice(gamepad)).CarControls.ShifterKnobScale.Current;
		}

		public ControlToggleSetting GetCasualToggle()
		{
			return CasualMode;
		}

		public void SetCasual(bool value)
		{
			CasualMode.Current = value;
			FlagsStorage.IsCasualControl = value;
			CasualModeChanged?.Invoke(value);
		}

		public ControlSetting GetCameraSlider(CameraSettingsContext ctx, CameraAxis axis)
		{
			CameraControls cameraControls = Cameras.Camera(ctx);
			if (cameraControls == null)
			{
				return null;
			}
			return axis switch
			{
				CameraAxis.SensX => cameraControls.Look.Axis(x: true), 
				CameraAxis.SensY => cameraControls.Look.Axis(x: false), 
				_ => null, 
			};
		}

		public ControlSetting GetDeviceMultiplierAxis(ControlDevice device, bool x)
		{
			return Device(device).CameraMultiplier.Axis(x);
		}

		public ControlToggleSetting GetDeviceMultiplierInvert(ControlDevice device, bool x)
		{
			if (!x)
			{
				return Device(device).CameraMultiplier.InvertY;
			}
			return Device(device).CameraMultiplier.InvertX;
		}

		public ControlSetting GetCarSlider(ControlDevice device, Sliders slider)
		{
			DeviceControls deviceControls = Device(device);
			return slider switch
			{
				Sliders.Steering => deviceControls.CarControls.SteeringSensitivity, 
				Sliders.ShifterKnob => deviceControls.CarControls.ShifterKnobScale, 
				Sliders.ZoomSpeed => deviceControls.ZoomSpeed, 
				_ => null, 
			};
		}

		public ControlSetting GetUniqueSlider(UniqueSliders slider)
		{
			return slider switch
			{
				UniqueSliders.Cursor => GamepadOnly.CursorSensitivity, 
				UniqueSliders.DeadzoneLeft => GamepadOnly.StickDeadzoneLeft, 
				UniqueSliders.DeadzoneRight => GamepadOnly.StickDeadzoneRight, 
				UniqueSliders.VibrationIntensity => GamepadOnly.VibrationIntensity, 
				UniqueSliders.DroneMove => Cameras.Drone.MoveSpeed, 
				UniqueSliders.DroneVertical => Cameras.Drone.VerticalSpeed, 
				UniqueSliders.DroneTilt => Cameras.Drone.TiltSpeed, 
				UniqueSliders.WheelRange => WheelOnly.RotationRange, 
				UniqueSliders.WheelFfbMultiplier => WheelOnly.FfbMultiplier, 
				UniqueSliders.MouseSteerMaxDisplacement => MouseKeyboardOnly.MouseSteerMaxDisplacement, 
				_ => null, 
			};
		}

		public ControlToggleSetting GetCarToggle(ControlDevice device, Toggles toggle)
		{
			DeviceControls deviceControls = Device(device);
			return toggle switch
			{
				Toggles.InvertSteering => deviceControls.CarControls.InvertSteering, 
				Toggles.ZoomInvert => deviceControls.ZoomInvert, 
				_ => null, 
			};
		}

		public ControlToggleSetting GetUniqueToggle(UniqueToggles toggle)
		{
			return toggle switch
			{
				UniqueToggles.VibrationEnabled => GamepadOnly.VibrationEnabled, 
				UniqueToggles.WheelRangeOverride => WheelOnly.RangeOverrideEnabled, 
				_ => null, 
			};
		}

		public void SetCameraSlider(CameraSettingsContext ctx, CameraAxis axis, float value)
		{
			if (axis == CameraAxis.SensX || axis == CameraAxis.SensY)
			{
				ControlSettingVector2 controlSettingVector = LookOf(ctx);
				if (controlSettingVector != null)
				{
					WriteAxis(controlSettingVector, axis == CameraAxis.SensX, value);
				}
				return;
			}
			ControlSetting cameraSlider = GetCameraSlider(ctx, axis);
			if (cameraSlider != null)
			{
				cameraSlider.Current = value;
				Save(cameraSlider);
			}
		}

		private ControlSettingVector2 LookOf(CameraSettingsContext ctx)
		{
			if (ctx != CameraSettingsContext.Person)
			{
				return Cameras.Camera(ctx)?.Look;
			}
			return Cameras.Person.Look;
		}

		public void SetDeviceMultiplier(ControlDevice device, bool x, float value)
		{
			WriteAxis(Device(device).CameraMultiplier, x, value);
		}

		public void SetDeviceMultiplierInvert(ControlDevice device, bool x, bool value)
		{
			ControlToggleSetting deviceMultiplierInvert = GetDeviceMultiplierInvert(device, x);
			deviceMultiplierInvert.Current = value;
			Save(deviceMultiplierInvert);
		}

		public void SetSlider(ControlDevice device, Sliders slider, float value)
		{
			ControlSetting carSlider = GetCarSlider(device, slider);
			if (carSlider != null)
			{
				carSlider.Current = value;
				Save(carSlider);
				if (slider == Sliders.Steering)
				{
					PushSteeringToV(device);
				}
			}
		}

		public void SetUniqueSlider(UniqueSliders slider, float value)
		{
			ControlSetting uniqueSlider = GetUniqueSlider(slider);
			if (uniqueSlider != null)
			{
				uniqueSlider.Current = value;
				Save(uniqueSlider);
				switch (slider)
				{
				case UniqueSliders.WheelRange:
					WheelRangeChanged?.Invoke(value);
					break;
				case UniqueSliders.WheelFfbMultiplier:
					WheelFfbMultiplierChanged?.Invoke(value);
					break;
				}
			}
		}

		public void SetToggle(ControlDevice device, Toggles toggle, bool value)
		{
			ControlToggleSetting carToggle = GetCarToggle(device, toggle);
			if (carToggle != null)
			{
				carToggle.Current = value;
				Save(carToggle);
			}
		}

		public void SetUniqueToggle(UniqueToggles toggle, bool value)
		{
			ControlToggleSetting uniqueToggle = GetUniqueToggle(toggle);
			if (uniqueToggle != null)
			{
				uniqueToggle.Current = value;
				Save(uniqueToggle);
				if (toggle == UniqueToggles.WheelRangeOverride)
				{
					WheelRangeOverrideChanged?.Invoke(value);
				}
			}
		}

		private void WriteAxis(ControlSettingVector2 vec, bool x, float value)
		{
			vec.Current = (x ? new Vector2(value, vec.Current.y) : new Vector2(vec.Current.x, value));
			Save(vec);
		}

		public void ResetDevice(ControlDevice device)
		{
			BuildIndex();
			foreach (var (controlSetting, text) in _floatIndex)
			{
				if (text.StartsWith(Prefix(device)))
				{
					controlSetting.ResetToDefault();
					PlayerPrefs.SetFloat(text, controlSetting.Current);
				}
			}
			foreach (var (controlToggleSetting, text2) in _boolIndex)
			{
				if (text2.StartsWith(Prefix(device)))
				{
					controlToggleSetting.ResetToDefault();
					PrefsSetBool(text2, controlToggleSetting.Current);
				}
			}
			foreach (var (controlSettingVector, text3, key) in _vec2Index)
			{
				if (text3.StartsWith(Prefix(device)))
				{
					controlSettingVector.ResetToDefault();
					PlayerPrefs.SetFloat(text3, controlSettingVector.Current.x);
					PlayerPrefs.SetFloat(key, controlSettingVector.Current.y);
				}
			}
			PushSteeringToV(device);
			if (device == ControlDevice.Wheel)
			{
				WheelRangeOverrideChanged?.Invoke(WheelOnly.RangeOverrideEnabled.Current);
				WheelRangeChanged?.Invoke(WheelOnly.RotationRange.Current);
				WheelFfbMultiplierChanged?.Invoke(WheelOnly.FfbMultiplier.Current);
			}
			CasualMode.ResetToDefault();
			FlagsStorage.IsCasualControl = CasualMode.Current;
			CasualModeChanged?.Invoke(CasualMode.Current);
		}

		public void ResetCameras()
		{
			BuildIndex();
			foreach (var (controlSetting, text) in _floatIndex)
			{
				if (text.StartsWith("SteamControls.Camera."))
				{
					controlSetting.ResetToDefault();
					PlayerPrefs.SetFloat(text, controlSetting.Current);
				}
			}
			foreach (var (controlToggleSetting, text2) in _boolIndex)
			{
				if (text2.StartsWith("SteamControls.Camera."))
				{
					controlToggleSetting.ResetToDefault();
					PrefsSetBool(text2, controlToggleSetting.Current);
				}
			}
			foreach (var (controlSettingVector, text3, key) in _vec2Index)
			{
				if (text3.StartsWith("SteamControls.Camera."))
				{
					controlSettingVector.ResetToDefault();
					PlayerPrefs.SetFloat(text3, controlSettingVector.Current.x);
					PlayerPrefs.SetFloat(key, controlSettingVector.Current.y);
				}
			}
		}

		private void PushSteeringToV(ControlDevice device)
		{
			float value = (V.SWsensitivity = Device(device).CarControls.SteeringSensitivity.Current);
			PlayerPrefs.SetFloat("SWsensitivity", value);
		}

		private void Save(ControlSetting setting)
		{
			BuildIndex();
			foreach (var (controlSetting, key) in _floatIndex)
			{
				if (controlSetting == setting)
				{
					PlayerPrefs.SetFloat(key, setting.Current);
					break;
				}
			}
		}

		private void Save(ControlToggleSetting toggle)
		{
			BuildIndex();
			foreach (var (controlToggleSetting, key) in _boolIndex)
			{
				if (controlToggleSetting == toggle)
				{
					PrefsSetBool(key, toggle.Current);
					break;
				}
			}
		}

		private void Save(ControlSettingVector2 look)
		{
			BuildIndex();
			foreach (var (controlSettingVector, key, key2) in _vec2Index)
			{
				if (controlSettingVector == look)
				{
					PlayerPrefs.SetFloat(key, look.Current.x);
					PlayerPrefs.SetFloat(key2, look.Current.y);
					break;
				}
			}
		}

		private static bool PrefsGetBool(string key, bool def)
		{
			return PlayerPrefs.GetInt(key, def ? 1 : 0) == 1;
		}

		private static void PrefsSetBool(string key, bool val)
		{
			PlayerPrefs.SetInt(key, val ? 1 : 0);
		}

		private static string Prefix(ControlDevice device)
		{
			return $"SteamControls.{device}.";
		}

		private void BuildIndex()
		{
			if (_floatIndex == null)
			{
				_floatIndex = new List<(ControlSetting, string)>();
				_boolIndex = new List<(ControlToggleSetting, string)>();
				_vec2Index = new List<(ControlSettingVector2, string, string)>();
				IndexCameras();
				IndexDevice(ControlDevice.MouseKeyboard);
				IndexDevice(ControlDevice.Gamepad);
				IndexDevice(ControlDevice.Wheel);
				IndexGamepadOnly();
				IndexWheel();
				IndexMouseKeyboardOnly();
			}
		}

		private void IndexMouseKeyboardOnly()
		{
			string text = Prefix(ControlDevice.MouseKeyboard);
			_floatIndex.Add((MouseKeyboardOnly.MouseSteerMaxDisplacement, text + "MouseSteerMax"));
		}

		private void IndexWheel()
		{
			string text = Prefix(ControlDevice.Wheel);
			_floatIndex.Add((WheelOnly.RotationRange, text + "Range"));
			_floatIndex.Add((WheelOnly.FfbMultiplier, text + "FfbMult"));
			_boolIndex.Add((WheelOnly.RangeOverrideEnabled, text + "RangeOverride"));
		}

		private void IndexGamepadOnly()
		{
			string text = Prefix(ControlDevice.Gamepad);
			_floatIndex.Add((GamepadOnly.CursorSensitivity, text + "Cursor"));
			_floatIndex.Add((GamepadOnly.StickDeadzoneLeft, text + "DeadL"));
			_floatIndex.Add((GamepadOnly.StickDeadzoneRight, text + "DeadR"));
			_floatIndex.Add((GamepadOnly.VibrationIntensity, text + "VibInt"));
			_boolIndex.Add((GamepadOnly.VibrationEnabled, text + "VibOn"));
		}

		private void IndexCameras()
		{
			foreach (CameraSettingsContext value in Enum.GetValues(typeof(CameraSettingsContext)))
			{
				CameraControls cameraControls = Cameras.Camera(value);
				if (cameraControls != null)
				{
					string text = string.Format("{0}{1}.", "SteamControls.Camera.", value);
					_vec2Index.Add((cameraControls.Look, text + "SensX", text + "SensY"));
				}
			}
			PersonCameraControls person = Cameras.Person;
			string text2 = "SteamControls.Camera.Person.";
			_vec2Index.Add((person.Look, text2 + "SensX", text2 + "SensY"));
			_floatIndex.Add((person.TurnSpeed, text2 + "Turn"));
			_floatIndex.Add((Cameras.Drone.MoveSpeed, "SteamControls.Camera.Drone.Move"));
			_floatIndex.Add((Cameras.Drone.VerticalSpeed, "SteamControls.Camera.Drone.Vert"));
			_floatIndex.Add((Cameras.Drone.TiltSpeed, "SteamControls.Camera.Drone.Tilt"));
		}

		private void IndexDevice(ControlDevice device)
		{
			DeviceControls deviceControls = Device(device);
			string text = Prefix(device);
			_vec2Index.Add((deviceControls.CameraMultiplier, text + "CamMultX", text + "CamMultY"));
			_boolIndex.Add((deviceControls.CameraMultiplier.InvertX, text + "CamInvX"));
			_boolIndex.Add((deviceControls.CameraMultiplier.InvertY, text + "CamInvY"));
			_floatIndex.Add((deviceControls.ZoomSpeed, text + "ZoomSpeed"));
			_boolIndex.Add((deviceControls.ZoomInvert, text + "ZoomInv"));
			_floatIndex.Add((deviceControls.CarControls.SteeringSensitivity, text + "Steering"));
			_floatIndex.Add((deviceControls.CarControls.ShifterKnobScale, text + "ShifterKnob"));
			_boolIndex.Add((deviceControls.CarControls.InvertSteering, text + "InvSteer"));
		}

		private static ControlDevice ToDevice(bool gamepad)
		{
			if (!gamepad)
			{
				return ControlDevice.MouseKeyboard;
			}
			return ControlDevice.Gamepad;
		}
	}
}
