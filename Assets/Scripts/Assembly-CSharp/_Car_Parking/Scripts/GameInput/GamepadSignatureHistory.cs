using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	public sealed class GamepadSignatureHistory
	{
		public const int Capacity = 8;

		private readonly GamepadSignature[] _ring = new GamepadSignature[8];

		private int _head;

		private int _count;

		public bool HasAny => _count > 0;

		public GamepadSignature Latest => _ring[(_head + 8 - 1) % 8];

		public void Push(in GamepadSignature signature)
		{
			_ring[_head] = signature;
			_head = (_head + 1) % 8;
			if (_count < 8)
			{
				_count++;
			}
		}

		public bool MatchesAnyWithin(in GamepadSignature signature, float windowSeconds)
		{
			for (int i = 0; i < _count; i++)
			{
				ref GamepadSignature reference = ref _ring[i];
				if (Mathf.Abs(reference.Time - signature.Time) <= windowSeconds && reference.Matches(in signature))
				{
					return true;
				}
			}
			return false;
		}
	}
}
