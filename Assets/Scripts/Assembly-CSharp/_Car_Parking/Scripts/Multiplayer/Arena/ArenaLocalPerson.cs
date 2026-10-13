using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaLocalPerson
	{
		public static bool TryGetSpawnPose(SpawnLayoutProvider layout, IArenaContext ctx, out PositionsAndRotation pose)
		{
			pose = null;
			if (!layout || ctx == null)
			{
				return false;
			}
			IReadOnlyList<int> participantActorNumbers = ctx.ParticipantActorNumbers;
			int count = Mathf.Max(1, participantActorNumbers.Count);
			pose = layout.GetPoint(LocalSpawnIndex(participantActorNumbers), count);
			return pose != null;
		}

		public static void PlaceOnSpawn(SpawnLayoutProvider layout, IArenaContext ctx, bool resyncNetwork = false)
		{
			if (TryGetSpawnPose(layout, ctx, out var pose))
			{
				PlaceAt(pose.Position, pose.Rotation, updateCamera: true, resyncNetwork);
			}
		}

		public static void PlaceAt(Vector3 position, Quaternion rotation, bool updateCamera = true, bool resyncNetwork = false)
		{
			if (!V.DriverPerson)
			{
				return;
			}
			if (V.DriverPerson.TryGetComponent<WorldInteractionController>(out var component))
			{
				component.CancelForTeleport();
			}
			if (V.DriverPerson.TryGetComponent<DriverActionController>(out var component2) && (bool)component2.ragdollCpm)
			{
				component2.ragdollCpm.ReleaseForTeleport();
			}
			if (V.DriverPerson.TryGetComponent<PersonLocomotion>(out var component3))
			{
				component3.Teleport(position, rotation, resyncNetwork);
			}
			else
			{
				if (V.DriverPerson.TryGetComponent<Rigidbody>(out var component4))
				{
					component4.velocity = Vector3.zero;
				}
				V.DriverPerson.transform.SetPositionAndRotation(position, rotation);
				if (resyncNetwork && V.DriverPerson.TryGetComponent<MultiPersonControl>(out var component5))
				{
					component5.ResyncAfterTeleport();
				}
			}
			if (updateCamera && V.DriverPerson.TryGetComponent<TouchMovePerson>(out var component6))
			{
				component6.SnapCameraBehind(rotation);
			}
		}

		public static void MountCameraOn(Transform point)
		{
			if ((bool)point && (bool)V.CoreGameManager)
			{
				if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<TouchMovePerson>(out var component))
				{
					component.UpdateCamera = false;
				}
				V.CoreGameManager.transform.SetPositionAndRotation(point.position, point.rotation);
			}
		}

		public static void Freeze(bool frozen)
		{
			if ((bool)V.DriverPerson)
			{
				if (frozen && V.DriverPerson.TryGetComponent<Rigidbody>(out var component))
				{
					component.velocity = Vector3.zero;
				}
				if (V.DriverPerson.TryGetComponent<PersonLocomotion>(out var component2))
				{
					component2.Freeze(frozen);
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
