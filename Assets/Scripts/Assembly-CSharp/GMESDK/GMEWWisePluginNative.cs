using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GMESDK
{
	public class GMEWWisePluginNative
	{
		public const string MyLibName = "TencentGME";

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr GMEWWisePlugin_GetVersion();

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_SetUserID(string userID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_SetRoomID(string roomID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_ReceivePlugin_SetReceiveOpenIDWithGameObjectID(ulong gameObjectID, string userID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_ReceivePlugin_GetReceiveOpenIDWithGameObjectID(ulong gameObjectID, StringBuilder userID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SendPlugin_EnableLoopbackWithGameObjectID(ulong gameObjectID, bool enableLoopback);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GMEWWisePlugin_SendPlugin_GetEnableLoopbackWithGameObjectID(ulong gameObjectID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_GetMessage(ref int localUTCTime, ref int messageType, ref int code, byte[] message1, int len1, byte[] message2, int len2);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetLogLevel(int logLevelWrite, int logLevelPrint);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetAudioStreamProfile(int profile);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_Pause();

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_Resume();

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_GetAudioSendStreamLevel();

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_GetAudioRecvStreamLevel(string targetID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_AddAudioBlockList(string targetID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_RemoveAudioBlockList(string targetID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_EnableBluetoothMic(bool enabled);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetRangeAudioTeamID(int teamID);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetRangeAudioTeamMode(int teamMode);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetRangeAudioRecvRange(int range);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetSelfPosition(int positionX, int positionY, int positionZ);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_StartRecording(string speechLanguage);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_StopRecording();

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_PlayRecordFile(string fileid);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_StopPlayFile();

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_SpeechToText(string fileid, string speechLanguage, string translateLanguage);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GMEWWisePlugin_GetVoiceFileDuration(string fileid);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetRegion(int region);

		[DllImport("TencentGME", CallingConvention = CallingConvention.Cdecl)]
		public static extern void GMEWWisePlugin_SetGainDB(int GainDB);
	}
}
