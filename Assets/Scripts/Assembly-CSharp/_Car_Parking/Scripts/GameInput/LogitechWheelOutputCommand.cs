using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput
{
	[StructLayout((LayoutKind)2, Size = 16)]
	internal struct LogitechWheelOutputCommand : IInputDeviceCommandInfo
	{
		internal const int kReportSize = 8;

		internal const int kSize = 16;

		internal const int MinRangeDegrees = 40;

		internal const int MaxRangeDegrees = 900;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public byte reportId;

		[FieldOffset(9)]
		public byte b0;

		[FieldOffset(10)]
		public byte b1;

		[FieldOffset(11)]
		public byte b2;

		[FieldOffset(12)]
		public byte b3;

		[FieldOffset(13)]
		public byte b4;

		[FieldOffset(14)]
		public byte b5;

		[FieldOffset(15)]
		public byte b6;

		public static FourCC Type => new FourCC('H', 'I', 'D', 'O');

		public FourCC typeStatic => Type;

		public static LogitechWheelOutputCommand NativeModeG25()
		{
			return Create(248, 16, 0, 0, 0, 0, 0);
		}

		public static LogitechWheelOutputCommand Range(int degrees)
		{
			int num = Mathf.Clamp(degrees, 40, 900);
			return Create(248, 129, (byte)(num & 0xFF), (byte)((num >> 8) & 0xFF), 0, 0, 0);
		}

		public static LogitechWheelOutputCommand Create(byte c0, byte c1, byte c2, byte c3, byte c4, byte c5, byte c6)
		{
			return new LogitechWheelOutputCommand
			{
				baseCommand = new InputDeviceCommand(Type, 16),
				reportId = 0,
				b0 = c0,
				b1 = c1,
				b2 = c2,
				b3 = c3,
				b4 = c4,
				b5 = c5,
				b6 = c6
			};
		}
	}
}
