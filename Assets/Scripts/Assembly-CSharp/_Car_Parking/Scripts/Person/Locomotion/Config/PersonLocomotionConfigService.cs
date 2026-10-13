using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Person.Locomotion.Stamina;

namespace _Car_Parking.Scripts.Person.Locomotion.Config
{
	public class PersonLocomotionConfigService : IPersonLocomotionConfigService
	{
		private const string LocomotionConfigPath = "Configs/Person/PersonLocomotionConfig";

		private const string CapsuleConfigPath = "Configs/Person/PersonCapsuleConfig";

		private const string StaminaConfigPath = "Configs/Person/StaminaConfig";

		private const string FootIKConfigPath = "Configs/Person/PersonFootIKConfig";

		public PersonLocomotionConfig Locomotion { get; }

		public PersonCapsuleConfig Capsule { get; }

		public StaminaConfig Stamina { get; }

		public PersonFootIKConfig FootIK { get; }

		[Preserve]
		public PersonLocomotionConfigService()
		{
			Locomotion = Resources.Load<PersonLocomotionConfig>("Configs/Person/PersonLocomotionConfig");
			Capsule = Resources.Load<PersonCapsuleConfig>("Configs/Person/PersonCapsuleConfig");
			Stamina = Resources.Load<StaminaConfig>("Configs/Person/StaminaConfig");
			FootIK = Resources.Load<PersonFootIKConfig>("Configs/Person/PersonFootIKConfig");
		}
	}
}
