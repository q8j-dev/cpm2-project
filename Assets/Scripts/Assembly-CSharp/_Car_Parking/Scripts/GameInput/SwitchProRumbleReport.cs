using System;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	public static class SwitchProRumbleReport
	{
		public const int BluetoothReportLength = 49;

		public const int UsbReportLength = 64;

		public const byte RumbleReportId = 16;

		public const byte SubcommandReportId = 1;

		public const byte EnableVibrationSubcommand = 72;

		public const int CounterOffset = 1;

		public const int LeftRumbleOffset = 2;

		public const int RightRumbleOffset = 6;

		public const int SubcommandOffset = 10;

		public const int SubcommandArgOffset = 11;

		public const ushort HighBandFrequency = 256;

		public const byte LowBandFrequency = 64;

		private const byte LowBandAmplitudeBase = 64;

		private const int MaxEncodedAmplitude = 100;

		public static byte EncodeAmplitude(float amplitude01)
		{
			double num = Mathf.Clamp01(amplitude01);
			if (num <= 0.0)
			{
				return 0;
			}
			double a = ((num > 0.23) ? (Math.Log(num * 8.7, 2.0) * 32.0) : ((!(num > 0.12)) ? (num / 0.12 * Math.Log(2.04, 2.0) * 16.0) : (Math.Log(num * 17.0, 2.0) * 16.0)));
			return (byte)Mathf.Clamp((int)Math.Round(a), 0, 100);
		}

		public static void EncodeRumble(byte[] buffer, int offset, float lowFrequency, float highFrequency)
		{
			int num = EncodeAmplitude(highFrequency) * 2;
			int num2 = EncodeAmplitude(lowFrequency) / 2 + 64;
			buffer[offset] = 0;
			buffer[offset + 1] = (byte)(num | 1);
			buffer[offset + 2] = 64;
			buffer[offset + 3] = (byte)num2;
		}

		public static void WriteRumble(byte[] report, byte counter, float lowFrequency, float highFrequency)
		{
			WriteHeader(report, 16, counter, lowFrequency, highFrequency);
		}

		public static void WriteRumbleWithEnable(byte[] report, byte counter, float lowFrequency, float highFrequency)
		{
			WriteHeader(report, 1, counter, lowFrequency, highFrequency);
			report[10] = 72;
			report[11] = 1;
		}

		private static void WriteHeader(byte[] report, byte reportId, byte counter, float lowFrequency, float highFrequency)
		{
			Array.Clear(report, 0, report.Length);
			report[0] = reportId;
			report[1] = (byte)(counter & 0xF);
			EncodeRumble(report, 2, lowFrequency, highFrequency);
			EncodeRumble(report, 6, lowFrequency, highFrequency);
		}
	}
}
