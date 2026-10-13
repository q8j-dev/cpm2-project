using _Car_Parking.Scripts.Person.Locomotion.Stamina;

namespace _Car_Parking.Scripts.Person.Locomotion.Config
{
	public interface IPersonLocomotionConfigService
	{
		PersonLocomotionConfig Locomotion { get; }

		PersonCapsuleConfig Capsule { get; }

		StaminaConfig Stamina { get; }

		PersonFootIKConfig FootIK { get; }
	}
}
