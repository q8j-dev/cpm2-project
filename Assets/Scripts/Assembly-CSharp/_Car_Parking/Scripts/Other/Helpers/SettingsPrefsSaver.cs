using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class SettingsPrefsSaver : IAllPrefsSaver
	{
		private static readonly string[] IntKeys = new string[35]
		{
			"ShowFps", "DarkButtons", "kph", "peopleTrafficDensity", "CarQuantity", "VibrateAvailablePrefs", "parkingSensor", "TCS", "ABS", "ESP",
			"Stabilization", "DriftMode", "ManualGearboxAssist", "hideSkidSmoke", "hideSkidmarks", "hideWheelDirt", "showCarMirror", "dynamicCloudiness", "steering", "gamepad",
			"AnalogClutch", "AnalogAccelerator", "AnalogBrake", "ignoreRacingRequest", "ignoreFriendRequest", "ignoreCarSellRequest", "hideAllChat", "hideAllTopLabel", "hideAllVinyls", "VoiceChat",
			"lowerVolumeOnVoiceChat", "AutoGraphicsPresetChecked", "SteamTrafficDefaultsV3", "SteamControls.CasualMode", "languageIndex"
		};

		private static readonly string[] FloatKeys = new string[8] { "SWsensitivity", "volume", "AmbienceVolume", "CarVolume", "UIVolume", "SFXVolume", "TiresVolume", "MusicVolume" };

		private static readonly string[] StringKeys = new string[4] { "InGameLanguage", "graphics_quality", "SteamControls.Bindings", "SteamControls.Bindings.Wheels" };

		private static readonly string[] ObscuredBoolKeys = new string[1] { "AutoFarClip" };

		private readonly Dictionary<string, int> _ints = new Dictionary<string, int>();

		private readonly Dictionary<string, float> _floats = new Dictionary<string, float>();

		private readonly Dictionary<string, string> _strings = new Dictionary<string, string>();

		private readonly Dictionary<string, bool> _obscuredBools = new Dictionary<string, bool>();

		private static readonly string[] DevicePrefixes = new string[3] { "SteamControls.MouseKeyboard.", "SteamControls.Gamepad.", "SteamControls.Wheel." };

		private const string CameraPrefix = "SteamControls.Camera.";

		public void Save()
		{
			_ints.Clear();
			_floats.Clear();
			_strings.Clear();
			_obscuredBools.Clear();
			string[] intKeys = IntKeys;
			foreach (string key in intKeys)
			{
				if (PlayerPrefs.HasKey(key))
				{
					_ints[key] = PlayerPrefs.GetInt(key);
				}
			}
			foreach (string item in SteamControlsIntKeys())
			{
				if (PlayerPrefs.HasKey(item))
				{
					_ints[item] = PlayerPrefs.GetInt(item);
				}
			}
			intKeys = FloatKeys;
			foreach (string key2 in intKeys)
			{
				if (PlayerPrefs.HasKey(key2))
				{
					_floats[key2] = PlayerPrefs.GetFloat(key2);
				}
			}
			foreach (string item2 in SteamControlsFloatKeys())
			{
				if (PlayerPrefs.HasKey(item2))
				{
					_floats[item2] = PlayerPrefs.GetFloat(item2);
				}
			}
			intKeys = StringKeys;
			foreach (string key3 in intKeys)
			{
				if (PlayerPrefs.HasKey(key3))
				{
					_strings[key3] = PlayerPrefs.GetString(key3);
				}
			}
			intKeys = ObscuredBoolKeys;
			foreach (string key4 in intKeys)
			{
				if (ObscuredPrefs.HasKey(key4))
				{
					_obscuredBools[key4] = ObscuredPrefs.GetBool(key4);
				}
			}
		}

		public void Load()
		{
			foreach (KeyValuePair<string, int> @int in _ints)
			{
				PlayerPrefs.SetInt(@int.Key, @int.Value);
			}
			foreach (KeyValuePair<string, float> @float in _floats)
			{
				PlayerPrefs.SetFloat(@float.Key, @float.Value);
			}
			foreach (KeyValuePair<string, string> @string in _strings)
			{
				PlayerPrefs.SetString(@string.Key, @string.Value);
			}
			foreach (KeyValuePair<string, bool> obscuredBool in _obscuredBools)
			{
				ObscuredPrefs.SetBool(obscuredBool.Key, obscuredBool.Value);
			}
			PlayerPrefs.Save();
		}

		private static IEnumerable<string> SteamControlsFloatKeys()
		{
			string[] array = new string[5] { "Car", "CarFpv", "Garage", "Drone", "Map" };
			foreach (string context in array)
			{
				yield return "SteamControls.Camera." + context + ".SensX";
				yield return "SteamControls.Camera." + context + ".SensY";
			}
			yield return "SteamControls.Camera.Person.SensX";
			yield return "SteamControls.Camera.Person.SensY";
			yield return "SteamControls.Camera.Person.Turn";
			yield return "SteamControls.Camera.Drone.Move";
			yield return "SteamControls.Camera.Drone.Vert";
			yield return "SteamControls.Camera.Drone.Tilt";
			array = DevicePrefixes;
			foreach (string context in array)
			{
				yield return context + "CamMultX";
				yield return context + "CamMultY";
				yield return context + "ZoomSpeed";
				yield return context + "Steering";
				yield return context + "ShifterKnob";
			}
			yield return "SteamControls.Gamepad.Cursor";
			yield return "SteamControls.Gamepad.DeadL";
			yield return "SteamControls.Gamepad.DeadR";
			yield return "SteamControls.Gamepad.VibInt";
			yield return "SteamControls.Wheel.Range";
			yield return "SteamControls.Wheel.FfbMult";
			yield return "SteamControls.MouseKeyboard.MouseSteerMax";
		}

		private static IEnumerable<string> SteamControlsIntKeys()
		{
			string[] devicePrefixes = DevicePrefixes;
			foreach (string prefix in devicePrefixes)
			{
				yield return prefix + "CamInvX";
				yield return prefix + "CamInvY";
				yield return prefix + "ZoomInv";
				yield return prefix + "InvSteer";
			}
			yield return "SteamControls.Gamepad.VibOn";
			yield return "SteamControls.Wheel.RangeOverride";
		}
	}
}
