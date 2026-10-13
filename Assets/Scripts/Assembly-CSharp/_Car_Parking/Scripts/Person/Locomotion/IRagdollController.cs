using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public interface IRagdollController
	{
		bool Exists { get; }

		bool IsRagdoll { get; }

		bool IsStandingUp { get; }

		bool CanStandUpFromInput { get; }

		void DisableRagdoll();

		void EnableFallRagdoll(Vector3 force, Vector3 initialVelocity);
	}
}
