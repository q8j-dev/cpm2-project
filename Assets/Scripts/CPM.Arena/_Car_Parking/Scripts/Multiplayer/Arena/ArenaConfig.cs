using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public abstract class ArenaConfig : ScriptableObject
	{
		[Tooltip("Stable unique id of THIS arena instance. Matchmaking partition + StartArena argument. Free = invalid (skipped by the registry).")]
		public ArenaId ArenaId;

		[Tooltip("Id of the rules controller (IArenaModeController.ModeId) this arena runs. Shared across maps.")]
		public ArenaModeType ModeType;

		[Header("Catalog")]
		[Tooltip("When false, this arena is hidden from the catalog / map (e.g. stub or WIP arenas).")]
		public bool Enabled = true;

		[Header("Matchmaking")]
		public int MinPlayers = 2;

		[Tooltip("Room capacity stored in the asset. Modes may override the effective value (ObstacleRace takes it from remote config).")]
		[FormerlySerializedAs("MaxPlayers")]
		[SerializeField]
		private int MaxPlayersDefault = 20;

		public Vector2Int AverageWaitTime = new Vector2Int(30, 60);

		[Tooltip("Seconds of fill window from room open before lock-in (restarts while below MinPlayers).")]
		public float WaitSeconds = 30f;

		[Tooltip("Final lock-in countdown length, in seconds, once gathering ends.")]
		public float CountdownSeconds = 5f;

		[Tooltip("Once MinPlayers is reached, clamp the remaining fill window to at most this many seconds. Never extends. 0 or less disables the clamp: the full window is waited out.")]
		public float MinPlayersWaitSeconds = 20f;

		[Tooltip("Max seconds the master waits for all players to be ready before scheduling the start anyway.")]
		public float ReadyBarrierTimeoutSeconds = 25f;

		[Tooltip("Countdown (seconds) from 'all ready' to the synchronized match start.")]
		public float MatchStartCountdownSeconds = 3f;

		[Header("Location")]
		[Tooltip("Arena spawn location used for the waiting area / arena.")]
		public ArenaLocation WaitingLocation;

		[Header("Text")]
		public string Title;

		public string Description;

		[Tooltip("Kits forced on every player for the whole arena visit. Never saved to the wardrobe.")]
		[Header("Outfit")]
		public ForcedPersonKit[] ForcedKits = Array.Empty<ForcedPersonKit>();

		public virtual int MaxPlayers => MaxPlayersDefault;

		[Preserve]
		protected ArenaConfig()
		{
		}

		public bool TryGetForcedKit(KitType slot, int gender, out int kitId)
		{
			kitId = -1;
			if (ForcedKits == null)
			{
				return false;
			}
			ForcedPersonKit[] forcedKits = ForcedKits;
			foreach (ForcedPersonKit forcedPersonKit in forcedKits)
			{
				if (forcedPersonKit != null && forcedPersonKit.Slot == slot)
				{
					int[] array = ((gender == 1) ? forcedPersonKit.MaleKitId : forcedPersonKit.FemaleKitId);
					if (array == null || array.Length == 0)
					{
						return false;
					}
					int num = array[UnityEngine.Random.Range(0, array.Length)];
					if (num < 0)
					{
						return false;
					}
					kitId = num;
					return true;
				}
			}
			return false;
		}

		private void OnValidate()
		{
			MinPlayers = Mathf.Clamp(MinPlayers, 2, 32);
			MaxPlayersDefault = Mathf.Clamp(MaxPlayersDefault, MinPlayers, 32);
		}
	}
}
