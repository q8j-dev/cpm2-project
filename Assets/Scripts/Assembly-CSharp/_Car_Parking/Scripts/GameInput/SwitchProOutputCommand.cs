using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput
{
	[StructLayout((LayoutKind)2, Size = 72)]
	public struct SwitchProOutputCommand : IInputDeviceCommandInfo
	{
		public const int kSize = 72;

		private const int B = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public ulong q0;

		[FieldOffset(16)]
		public ulong q1;

		[FieldOffset(24)]
		public ulong q2;

		[FieldOffset(32)]
		public ulong q3;

		[FieldOffset(40)]
		public ulong q4;

		[FieldOffset(48)]
		public ulong q5;

		[FieldOffset(56)]
		public ulong q6;

		[FieldOffset(64)]
		public ulong q7;

		public static FourCC Type => new FourCC('H', 'I', 'D', 'O');

		public FourCC typeStatic => Type;

		public static SwitchProOutputCommand Create(byte[] report, int reportSize)
		{
			int num = Mathf.Clamp(reportSize, 49, 64);
			return new SwitchProOutputCommand
			{
				baseCommand = new InputDeviceCommand(Type, 8 + num),
				q0 = BitConverter.ToUInt64(report, 0),
				q1 = BitConverter.ToUInt64(report, 8),
				q2 = BitConverter.ToUInt64(report, 16),
				q3 = BitConverter.ToUInt64(report, 24),
				q4 = BitConverter.ToUInt64(report, 32),
				q5 = BitConverter.ToUInt64(report, 40),
				q6 = BitConverter.ToUInt64(report, 48),
				q7 = BitConverter.ToUInt64(report, 56)
			};
		}
	}
}
