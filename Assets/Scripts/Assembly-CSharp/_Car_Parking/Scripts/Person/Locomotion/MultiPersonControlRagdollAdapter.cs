using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class MultiPersonControlRagdollAdapter : IRagdollController
	{
		private readonly MultiPersonControl _control;

		private RagdollCPM Ragdoll
		{
			get
			{
				if (!_control || !_control.driverActionController)
				{
					return null;
				}
				return _control.driverActionController.ragdollCpm;
			}
		}

		public bool Exists => Ragdoll;

		public bool IsRagdoll
		{
			get
			{
				RagdollCPM ragdoll = Ragdoll;
				if ((bool)ragdoll)
				{
					return ragdoll.isRagdoll;
				}
				return false;
			}
		}

		public bool IsStandingUp
		{
			get
			{
				RagdollCPM ragdoll = Ragdoll;
				if ((bool)ragdoll)
				{
					return ragdoll.isStadningUp;
				}
				return false;
			}
		}

		public bool CanStandUpFromInput
		{
			get
			{
				RagdollCPM ragdoll = Ragdoll;
				if ((bool)ragdoll)
				{
					return ragdoll.CanStandUpFromInput;
				}
				return true;
			}
		}

		public MultiPersonControlRagdollAdapter(GameObject owner)
		{
			if ((bool)owner)
			{
				owner.TryGetComponent<MultiPersonControl>(out _control);
			}
		}

		public void DisableRagdoll()
		{
			RagdollCPM ragdoll = Ragdoll;
			if ((bool)ragdoll)
			{
				ragdoll.DisableRagdoll();
			}
		}

		public void EnableFallRagdoll(Vector3 force, Vector3 initialVelocity)
		{
			RagdollCPM ragdoll = Ragdoll;
			if ((bool)ragdoll)
			{
				ragdoll.RagdollEnabler(force, default, autoStandUp: true, initialVelocity).Forget();
			}
		}
	}
}
