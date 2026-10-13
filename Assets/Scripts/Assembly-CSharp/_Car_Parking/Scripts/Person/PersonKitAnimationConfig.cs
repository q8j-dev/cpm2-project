using UnityEngine;

namespace _Car_Parking.Scripts.Person
{
	[CreateAssetMenu(fileName = "PersonKitAnimationConfig", menuName = "Scriptable/Person/PersonKitAnimationConfig")]
	public class PersonKitAnimationConfig : ScriptableObject
	{
		[Tooltip("Animator layer owned by PersonKitAnimationLayer. Must exist in both person controllers.")]
		public string LayerName = "Marathon_Bag";

		[Tooltip("Kit slot watched on MainPerson.allKits.")]
		public PersonKitType Slot = PersonKitType.Bag;

		[Tooltip("Male kit ids that switch the layer on.")]
		public int[] MaleKitIds = new int[1] { 1 };

		[Tooltip("Female kit ids that switch the layer on.")]
		public int[] FemaleKitIds = new int[1] { 1 };

		[Range(0f, 1f)]
		[Tooltip("Layer weight held while one of the kits is worn.")]
		public float Weight = 1f;

		[Tooltip("Lerp speed toward the worn weight.")]
		public float WeightLerpPowerUp = 8f;

		[Tooltip("Lerp speed back to zero once the kit is off.")]
		public float WeightLerpPowerDown = 8f;

		public int[] KitIdsFor(int gender)
		{
			if (gender != 1)
			{
				return FemaleKitIds;
			}
			return MaleKitIds;
		}
	}
}
