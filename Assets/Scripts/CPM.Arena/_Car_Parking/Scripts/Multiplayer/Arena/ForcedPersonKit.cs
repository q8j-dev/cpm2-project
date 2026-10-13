using System;
using UnityEngine;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Serializable]
	public class ForcedPersonKit
	{
		[Tooltip("Outfit slot to override.")]
		public KitType Slot;

		[Tooltip("Kit ids from the male catalog, one is picked at random. Empty or -1 = keep the player's own.")]
		public int[] MaleKitId;

		[Tooltip("Kit ids from the female catalog, one is picked at random. Empty or -1 = keep the player's own.")]
		public int[] FemaleKitId;
	}
}
