using System;
using CPM.RacingRecords;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	public sealed class GhostRaceState
	{
		public GhostSelectionKind Kind { get; private set; }

		public RacingRecord Opponent { get; private set; }

		public bool PickArmed { get; private set; }

		public bool RaceEnabled
		{
			get
			{
				return GhostTogglePrefs.Enabled;
			}
			private set
			{
				GhostTogglePrefs.Enabled = value;
			}
		}

		public GhostSelectionKind EffectiveKind
		{
			get
			{
				if (!RaceEnabled)
				{
					return GhostSelectionKind.None;
				}
				return Kind;
			}
		}

		public RacingRecord EffectiveOpponent
		{
			get
			{
				if (!RaceEnabled)
				{
					return null;
				}
				return Opponent;
			}
		}

		public event Action Changed;

		public void ArmPick()
		{
			RaceEnabled = true;
			PickArmed = true;
			Changed?.Invoke();
		}

		public void PickOpponent(RacingRecord record)
		{
			if (record != null)
			{
				Kind = GhostSelectionKind.Opponent;
				Opponent = record;
				PickArmed = false;
				Changed?.Invoke();
			}
		}

		public void PickLocal()
		{
			Kind = GhostSelectionKind.Local;
			Opponent = null;
			PickArmed = false;
			Changed?.Invoke();
		}

		public void SetRaceEnabled(bool on)
		{
			RaceEnabled = on;
			if (!on)
			{
				PickArmed = false;
			}
			Changed?.Invoke();
		}
	}
}
