using System;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaPhaseModel
	{
		private ArenaPhase _phase;

		public ArenaPhase Phase
		{
			get
			{
				return _phase;
			}
			set
			{
				if (_phase != value)
				{
					_phase = value;
					OnPhaseChanged?.Invoke(value);
				}
			}
		}

		public event Action<ArenaPhase> OnPhaseChanged;
	}
}
