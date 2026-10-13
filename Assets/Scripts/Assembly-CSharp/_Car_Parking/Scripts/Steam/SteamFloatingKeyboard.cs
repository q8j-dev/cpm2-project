using System;
using System.Runtime.InteropServices;
using Steamworks;

namespace _Car_Parking.Scripts.Steam
{
	internal static class SteamFloatingKeyboard
	{
		private const string SteamApiDll = "steam_api64";

		private static IntPtr _steamUtilsPtr = IntPtr.Zero;

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SteamAPI_SteamUtils_v010();

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamAPI_ISteamUtils_ShowFloatingGamepadTextInput(IntPtr self, int eKeyboardMode, int nTextFieldXPosition, int nTextFieldYPosition, int nTextFieldWidth, int nTextFieldHeight);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamAPI_ISteamUtils_DismissFloatingGamepadTextInput(IntPtr self);

		public static bool TryShow(TextInputMode mode, int x, int y, int width, int height)
		{
			if (!SteamClient.IsValid)
			{
				return false;
			}
			try
			{
				IntPtr steamUtils = GetSteamUtils();
				if (steamUtils == IntPtr.Zero)
				{
					return false;
				}
				return SteamAPI_ISteamUtils_ShowFloatingGamepadTextInput(steamUtils, (int)mode, x, y, width, height);
			}
			catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
			{
				return false;
			}
		}

		public static bool TryDismiss()
		{
			if (!SteamClient.IsValid)
			{
				return false;
			}
			try
			{
				IntPtr steamUtils = GetSteamUtils();
				if (steamUtils == IntPtr.Zero)
				{
					return false;
				}
				return SteamAPI_ISteamUtils_DismissFloatingGamepadTextInput(steamUtils);
			}
			catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
			{
				return false;
			}
		}

		private static IntPtr GetSteamUtils()
		{
			if (_steamUtilsPtr == IntPtr.Zero)
			{
				_steamUtilsPtr = SteamAPI_SteamUtils_v010();
			}
			return _steamUtilsPtr;
		}
	}
}
