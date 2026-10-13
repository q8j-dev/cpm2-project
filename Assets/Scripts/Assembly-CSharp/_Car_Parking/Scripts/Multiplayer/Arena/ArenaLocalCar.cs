using System.Collections.Generic;
using CPMEngine;
using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaLocalCar
	{
		public static void PlaceOnSpawn(SpawnLayoutProvider layout, IArenaContext ctx)
		{
			if ((bool)layout && (bool)V.Car)
			{
				IReadOnlyList<int> participantActorNumbers = ctx.ParticipantActorNumbers;
				int count = Mathf.Max(1, participantActorNumbers.Count);
				int index = LocalSpawnIndex(participantActorNumbers);
				PositionsAndRotation point = layout.GetPoint(index, count);
				V.Car.transform.SetPositionAndRotation(point.Position, point.Rotation);
			}
		}

		public static void Freeze(bool frozen)
		{
			if ((bool)V.Car && V.Car.TryGetComponent<Powertrain>(out var component))
			{
				component.FixVehicle(frozen);
				if (frozen)
				{
					component.ResetTransmission();
				}
			}
		}

		private static int LocalSpawnIndex(IReadOnlyList<int> participants)
		{
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			for (int i = 0; i < participants.Count; i++)
			{
				if (participants[i] == actorNumber)
				{
					return i;
				}
			}
			return 0;
		}
	}
}
