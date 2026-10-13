using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaSpawnGroundHold
	{
		private const float RayUpOffset = 0.5f;

		private const float RayDistance = 8f;

		private const float TimeoutSeconds = 20f;

		public static async UniTask HoldPersonUntilGroundedAsync(GameObject person, CancellationToken ct)
		{
			if (!person || !person.TryGetComponent<Rigidbody>(out var rb))
			{
				return;
			}
			int mask = ~LayerMask.GetMask("DriverPerson", "pedestrain", "Ignore Raycast");
			float deadline = Time.unscaledTime + 20f;
			bool held = false;
			while (!HasGroundBelow(person.transform.position, mask) && !(Time.unscaledTime >= deadline))
			{
				rb.isKinematic = true;
				held = true;
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
				if (!person)
				{
					return;
				}
			}
			if (held)
			{
				rb.isKinematic = false;
				rb.velocity = Vector3.zero;
			}
		}

		private static bool HasGroundBelow(Vector3 position, int mask)
		{
			return Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, 8f, mask, QueryTriggerInteraction.Ignore);
		}
	}
}
