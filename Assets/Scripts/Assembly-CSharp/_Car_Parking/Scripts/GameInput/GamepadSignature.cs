using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	public readonly struct GamepadSignature
	{
		public const float StickActiveThreshold = 0.4f;

		public const float StickMatchTolerance = 0.3f;

		public readonly ushort Buttons;

		public readonly Vector2 LeftStick;

		public readonly Vector2 RightStick;

		public readonly float Time;

		public bool IsActive
		{
			get
			{
				if (Buttons == 0 && !(LeftStick.sqrMagnitude > 0.16000001f))
				{
					return RightStick.sqrMagnitude > 0.16000001f;
				}
				return true;
			}
		}

		public GamepadSignature(ushort buttons, Vector2 leftStick, Vector2 rightStick, float time)
		{
			Buttons = buttons;
			LeftStick = leftStick;
			RightStick = rightStick;
			Time = time;
		}

		public bool Matches(in GamepadSignature other)
		{
			if (Buttons == other.Buttons && (LeftStick - other.LeftStick).sqrMagnitude < 0.09f)
			{
				return (RightStick - other.RightStick).sqrMagnitude < 0.09f;
			}
			return false;
		}
	}
}
