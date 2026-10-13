using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using mozaAPI;

namespace _Car_Parking.Scripts.Car.Physics
{
	public static class MozaWrapper
	{
		private struct PROCESSENTRY32
		{
			public uint dwSize;

			public uint cntUsage;

			public uint th32ProcessID;

			public IntPtr th32DefaultHeapID;

			public uint th32ModuleID;

			public uint cntThreads;

			public uint th32ParentProcessID;

			public int pcPriClassBase;

			public uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szExeFile;
		}

		private static float? _lastSteerAngle;

		private static ERRORCODE _errorcode;

		private static float _wheelHalfRangeDeg = 270f;

		private static float _nextRangePollTime;

		private const float RangePollInterval = 2f;

		private static readonly string[] MozaProcessImages = new string[2] { "MOZA Pit House.exe", "MozaPithouse.exe" };

		private const int MozaVendorId = 13422;

		private const uint TH32CS_SNAPPROCESS = 2u;

		private const uint PROCESS_TERMINATE = 1u;

		private const int ERROR_INVALID_PARAMETER = 87;

		private static readonly IntPtr InvalidHandle = new IntPtr(-1);

		private const float ConnectionPollIntervalSec = 2f;

		private static int? _detectLastThrottle;

		private static int? _detectLastBrake;

		private static int? _detectLastClutch;

		public static readonly int[] AllInput = new int[44];

		private static readonly int[] HiddenInput = new int[44];

		private static readonly bool[] ButtonClicks = new bool[40];

		public static readonly Action<bool>[] ButtonCallbacks = new Action<bool>[40];

		public static bool SdkInstalled { get; private set; }

		public static bool BaseConnected { get; private set; }

		public static event Action<bool> BaseConnectionChanged;

		[RuntimeInitializeOnLoadMethod]
		public static void Install()
		{
			Debug.Log("[MozaWrapper] Install() entered");
			SdkInstalled = false;
			InputSystem.onDeviceChange -= OnDeviceChangeBeforeInstall;
			if (!AnyMozaDevicePresent())
			{
				Debug.Log("[MozaWrapper] No Moza device present — SDK install deferred until one is plugged in");
				InputSystem.onDeviceChange += OnDeviceChangeBeforeInstall;
			}
			else
			{
				InstallSdk();
			}
		}

		private static bool AnyMozaDevicePresent()
		{
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			for (int i = 0; i < devices.Count; i++)
			{
				if (IsMozaDevice(devices[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsMozaDevice(InputDevice device)
		{
			InputDeviceDescription description = device.description;
			if (ContainsMozaName(description.manufacturer) || ContainsMozaName(description.product))
			{
				return true;
			}
			if (description.interfaceName != "HID" || string.IsNullOrEmpty(description.capabilities))
			{
				return false;
			}
			try
			{
				return HID.HIDDeviceDescriptor.FromJson(description.capabilities).vendorId == 13422;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static bool ContainsMozaName(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				if (s.IndexOf("moza", StringComparison.OrdinalIgnoreCase) < 0)
				{
					return s.IndexOf("gudsen", StringComparison.OrdinalIgnoreCase) >= 0;
				}
				return true;
			}
			return false;
		}

		private static void OnDeviceChangeBeforeInstall(InputDevice device, InputDeviceChange change)
		{
			if ((change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected) && !SdkInstalled && IsMozaDevice(device))
			{
				InputSystem.onDeviceChange -= OnDeviceChangeBeforeInstall;
				Debug.Log("[MozaWrapper] Moza device plugged in (" + device.description.product + ") — installing SDK");
				InstallSdkNextFrameAsync().Forget();
			}
		}

		private static async UniTaskVoid InstallSdkNextFrameAsync()
		{
			await UniTask.Yield(PlayerLoopTiming.Update, Application.exitCancellationToken).SuppressCancellationThrow();
			if (!SdkInstalled && !Application.exitCancellationToken.IsCancellationRequested)
			{
				InstallSdk();
			}
		}

		private static void InstallSdk()
		{
			try
			{
				global::mozaAPI.mozaAPI.installMozaSDK();
				SdkInstalled = true;
				Debug.Log("[MozaWrapper] installMozaSDK() returned");
			}
			catch (Exception ex)
			{
				Debug.LogError("[MozaWrapper] installMozaSDK threw " + ex.GetType().Name + ": " + ex.Message);
				return;
			}
			Application.quitting -= Uninstall;
			Application.quitting += Uninstall;
			try
			{
				RefreshWheelRange();
				Debug.Log("[MozaWrapper] RefreshWheelRange completed");
			}
			catch (Exception ex2)
			{
				Debug.LogError("[MozaWrapper] RefreshWheelRange threw " + ex2.GetType().Name + ": " + ex2.Message);
			}
			BaseConnected = false;
			PollBaseConnectionAsync().Forget();
		}

		public static void Uninstall()
		{
			Debug.Log("[MozaWrapper] Uninstall() entered");
			InputSystem.onDeviceChange -= OnDeviceChangeBeforeInstall;
			KillMozaPitHouseProcesses();
			try
			{
				global::mozaAPI.mozaAPI.removeMozaSDK();
				Debug.Log("[MozaWrapper] removeMozaSDK() returned");
			}
			catch (Exception ex)
			{
				Debug.LogError("[MozaWrapper] removeMozaSDK threw " + ex.GetType().Name + ": " + ex.Message);
			}
			KillMozaPitHouseProcesses();
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hObject);

		private static void KillMozaPitHouseProcesses()
		{
			IntPtr intPtr = CreateToolhelp32Snapshot(2u, 0u);
			if (intPtr == IntPtr.Zero || intPtr == InvalidHandle)
			{
				Debug.LogError($"[MozaWrapper] CreateToolhelp32Snapshot failed err={Marshal.GetLastWin32Error()}");
				return;
			}
			try
			{
				List<(uint, uint, string)> list = new List<(uint, uint, string)>();
				PROCESSENTRY32 lppe = new PROCESSENTRY32
				{
					dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32))
				};
				if (Process32First(intPtr, ref lppe))
				{
					do
					{
						list.Add((lppe.th32ProcessID, lppe.th32ParentProcessID, lppe.szExeFile ?? string.Empty));
					}
					while (Process32Next(intPtr, ref lppe));
				}
				HashSet<uint> hashSet = new HashSet<uint>();
				foreach (var item in list)
				{
					for (int i = 0; i < MozaProcessImages.Length; i++)
					{
						if (string.Equals(item.Item3, MozaProcessImages[i], StringComparison.OrdinalIgnoreCase))
						{
							hashSet.Add(item.Item1);
						}
					}
				}
				foreach (var item2 in list)
				{
					if (item2.Item2 != 0 && hashSet.Contains(item2.Item2))
					{
						hashSet.Add(item2.Item1);
					}
				}
				if (hashSet.Count == 0)
				{
					Debug.Log("[MozaWrapper] Pit House not running, nothing to kill");
					return;
				}
				foreach (uint item3 in hashSet)
				{
					IntPtr intPtr2 = OpenProcess(1u, bInheritHandle: false, item3);
					if (intPtr2 == IntPtr.Zero)
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error == 87)
						{
							Debug.Log($"[MozaWrapper] pid={item3} already exited");
						}
						else
						{
							Debug.LogError($"[MozaWrapper] OpenProcess failed pid={item3} err={lastWin32Error}");
						}
					}
					else
					{
						if (!TerminateProcess(intPtr2, 0u))
						{
							Debug.LogError($"[MozaWrapper] TerminateProcess failed pid={item3} err={Marshal.GetLastWin32Error()}");
						}
						else
						{
							Debug.Log($"[MozaWrapper] Terminated pid={item3}");
						}
						CloseHandle(intPtr2);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[MozaWrapper] KillMozaPitHouseProcesses threw " + ex.GetType().Name + ": " + ex.Message);
			}
			finally
			{
				CloseHandle(intPtr);
			}
		}

		public static void RefreshWheelRange()
		{
			if (!SdkInstalled)
			{
				return;
			}
			ERRORCODE err = ERRORCODE.NORMAL;
			Tuple<int, int> motorLimitAngle = global::mozaAPI.mozaAPI.getMotorLimitAngle(ref err);
			if (err == ERRORCODE.NORMAL)
			{
				int item = motorLimitAngle.Item1;
				if (item >= 60 && item <= 2880)
				{
					_wheelHalfRangeDeg = (float)item * 0.5f;
				}
			}
		}

		public static bool TrySetWheelRange(int totalDegrees)
		{
			if (!BaseConnected)
			{
				return false;
			}
			int num = Mathf.Clamp(totalDegrees, 60, 2880);
			try
			{
				if (global::mozaAPI.mozaAPI.setMotorLimitAngle(num, num) != ERRORCODE.NORMAL)
				{
					return false;
				}
				RefreshWheelRange();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsBaseConnected()
		{
			return BaseConnected;
		}

		private static bool ProbeBase()
		{
			try
			{
				ERRORCODE err = ERRORCODE.NORMAL;
				Tuple<int, int> motorLimitAngle = global::mozaAPI.mozaAPI.getMotorLimitAngle(ref err);
				return err == ERRORCODE.NORMAL && motorLimitAngle.Item1 >= 60 && motorLimitAngle.Item1 <= 2880;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static async UniTaskVoid PollBaseConnectionAsync()
		{
			CancellationToken ct = Application.exitCancellationToken;
			while (!ct.IsCancellationRequested)
			{
				bool flag = ProbeBase();
				if (flag != BaseConnected)
				{
					BaseConnected = flag;
					if (flag)
					{
						RefreshWheelRange();
					}
					BaseConnectionChanged?.Invoke(flag);
				}
				await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: true, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}
		}

		public static float GetMozaSteerAngle()
		{
			if (!SdkInstalled)
			{
				return 0f;
			}
			if (Time.unscaledTime >= _nextRangePollTime)
			{
				_nextRangePollTime = Time.unscaledTime + 2f;
				RefreshWheelRange();
			}
			float fSteeringWheelAngle = global::mozaAPI.mozaAPI.getHIDData(ref _errorcode).fSteeringWheelAngle;
			(float?, float) tuple = CustomRemap(_lastSteerAngle, fSteeringWheelAngle, new float2(-1f, 1f));
			float? item = tuple.Item1;
			float item2 = tuple.Item2;
			_lastSteerAngle = item;
			return Snap(item2, (0f, 0.005f), (1f, 0.001f), (-1f, 0.001f));
		}

		public static bool AnyPedalEngaged(float threshold = 0.15f)
		{
			if (!SdkInstalled)
			{
				return false;
			}
			HIDData hIDData = global::mozaAPI.mozaAPI.getHIDData(ref _errorcode);
			float num = RemapPedalForDetect(ref _detectLastThrottle, hIDData.throttle);
			float num2 = RemapPedalForDetect(ref _detectLastBrake, hIDData.brake);
			float num3 = RemapPedalForDetect(ref _detectLastClutch, hIDData.clutch);
			if (!(num > threshold) && !(num2 > threshold))
			{
				return num3 > threshold;
			}
			return true;
		}

		private static float RemapPedalForDetect(ref int? last, int input)
		{
			(int?, float) tuple = CustomRemap(last, input, new Vector2(0f, 1f));
			int? item = tuple.Item1;
			float item2 = tuple.Item2;
			last = item;
			return item2;
		}

		public static bool GetMozaHandbrake()
		{
			if (!SdkInstalled)
			{
				return false;
			}
			return global::mozaAPI.mozaAPI.getHIDData(ref _errorcode).buttonHandbrake;
		}

		public static int GetMozaGear()
		{
			if (!SdkInstalled)
			{
				return 0;
			}
			return global::mozaAPI.mozaAPI.getHIDData(ref _errorcode).shift switch
			{
				GEAR.R => -1, 
				GEAR.GEAR1st => 1, 
				GEAR.GEAR2nd => 2, 
				GEAR.GEAR3rd => 3, 
				GEAR.GEAR4th => 4, 
				GEAR.GEAR5th => 5, 
				GEAR.GEAR6th => 6, 
				GEAR.GEAR7th => 7, 
				_ => 0, 
			};
		}

		private static float Remap(float v, float inMin, float inMax, float outMin, float outMax)
		{
			return (v - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
		}

		private static (float?, float) CustomRemap(float? last, float input, float2 remap)
		{
			bool flag = last.HasValue && !float.IsNaN(last.Value) && !float.IsInfinity(last.Value);
			if (float.IsNaN(input) || float.IsInfinity(input))
			{
				if (!flag)
				{
					return (null, 0f);
				}
				return (last, Remap(last.Value, 0f - _wheelHalfRangeDeg, _wheelHalfRangeDeg, remap.x, remap.y));
			}
			if (flag && Mathf.Abs(input - last.Value) > _wheelHalfRangeDeg - 0.005f)
			{
				return (last, Remap(last.Value, 0f - _wheelHalfRangeDeg, _wheelHalfRangeDeg, remap.x, remap.y));
			}
			last = input;
			float item = Remap(last.Value, 0f - _wheelHalfRangeDeg, _wheelHalfRangeDeg, remap.x, remap.y);
			return (last, item);
		}

		private static (int?, float) CustomRemap(int? last, int input, Vector2 remap)
		{
			if (last.HasValue && Mathf.Abs(input - last.Value) > 60000)
			{
				return (last, math.remap(-32768f, 32767f, remap.x, remap.y, last.Value));
			}
			last = input;
			float item = math.remap(-32768f, 32767f, remap.x, remap.y, last.Value);
			return (last, item);
		}

		private static float Snap(float remapped, params (float x, float precision)[] zones)
		{
			for (int i = 0; i < zones.Length; i++)
			{
				var (num, floatDiff) = zones[i];
				if (remapped.AlmostEquals(num, floatDiff))
				{
					remapped = num;
				}
			}
			return remapped;
		}

		public static void Poll()
		{
			if (HiddenInput[3] == 0)
			{
				Array.Copy(HiddenInput, AllInput, 44);
			}
			for (int i = 0; i < 40; i++)
			{
				if (AllInput[i + 4] == 2)
				{
					ButtonClicks[i] = !ButtonClicks[i];
					ButtonCallbacks[i]?.Invoke(ButtonClicks[i]);
				}
			}
		}
	}
}
