using System;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	public static class PlayStationBluetoothReport
	{
		public const int ReportLength = 78;

		public const int BufferLength = 80;

		public const int CrcOffset = 74;

		public const byte DualSenseReportId = 49;

		public const int DualSenseSeqTagOffset = 1;

		public const int DualSenseTagOffset = 2;

		public const int DualSenseValidFlag0Offset = 3;

		public const int DualSenseMotorRightOffset = 5;

		public const int DualSenseMotorLeftOffset = 6;

		public const int DualSenseValidFlag2Offset = 41;

		public const byte DualShock4ReportId = 17;

		public const int DualShock4HwControlOffset = 1;

		public const int DualShock4ValidFlag0Offset = 3;

		public const int DualShock4MotorRightOffset = 6;

		public const int DualShock4MotorLeftOffset = 7;

		private const byte DualSenseTag = 16;

		private const byte DualSenseValidFlag0Rumble = 3;

		private const byte DualSenseValidFlag2ImprovedRumble = 4;

		private const byte DualShock4HwControl = 192;

		private const byte DualShock4ValidFlag0Motor = 1;

		private const byte CrcSeed = 162;

		private static readonly uint[] CrcTable = BuildCrcTable();

		public static void WriteDualSense(byte[] report, byte sequence, byte lowFrequency, byte highFrequency)
		{
			Array.Clear(report, 0, report.Length);
			report[0] = 49;
			report[1] = (byte)((sequence & 0xF) << 4);
			report[2] = 16;
			report[3] = 3;
			report[5] = highFrequency;
			report[6] = lowFrequency;
			report[41] = 4;
			WriteCrc(report);
		}

		public static void WriteDualShock4(byte[] report, byte lowFrequency, byte highFrequency)
		{
			Array.Clear(report, 0, report.Length);
			report[0] = 17;
			report[1] = 192;
			report[3] = 1;
			report[6] = highFrequency;
			report[7] = lowFrequency;
			WriteCrc(report);
		}

		public static uint Crc32(byte[] data, int count)
		{
			uint num = uint.MaxValue;
			num = CrcTable[(num ^ 0xA2) & 0xFF] ^ (num >> 8);
			for (int i = 0; i < count; i++)
			{
				num = CrcTable[(num ^ data[i]) & 0xFF] ^ (num >> 8);
			}
			return ~num;
		}

		public static byte ToMotorByte(float speed01)
		{
			return (byte)Mathf.RoundToInt(Mathf.Clamp01(speed01) * 255f);
		}

		private static void WriteCrc(byte[] report)
		{
			uint num = Crc32(report, 74);
			report[74] = (byte)num;
			report[75] = (byte)(num >> 8);
			report[76] = (byte)(num >> 16);
			report[77] = (byte)(num >> 24);
		}

		private static uint[] BuildCrcTable()
		{
			uint[] array = new uint[256];
			for (uint num = 0u; num < 256; num++)
			{
				uint num2 = num;
				for (int i = 0; i < 8; i++)
				{
					num2 = (((num2 & 1) != 0) ? (0xEDB88320u ^ (num2 >> 1)) : (num2 >> 1));
				}
				array[num] = num2;
			}
			return array;
		}
	}
}
