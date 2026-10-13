using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	public sealed class GamepadMirrorEvidence
	{
		public const int MaxScore = 10;

		public const int Threshold = 5;

		public int Score { get; private set; }

		public bool IsMirror { get; private set; } = true;

		public bool Record(bool echoed)
		{
			Score = Mathf.Clamp(Score + (echoed ? 1 : (-1)), -10, 10);
			bool isMirror = IsMirror;
			if (Score >= 5)
			{
				IsMirror = true;
			}
			else if (Score <= -5)
			{
				IsMirror = false;
			}
			return isMirror != IsMirror;
		}
	}
}
