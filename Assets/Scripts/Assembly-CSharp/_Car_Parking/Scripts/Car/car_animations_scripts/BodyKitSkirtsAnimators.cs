using CarAnimationScripts;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.car_animations_scripts
{
	public class BodyKitSkirtsAnimators : MonoBehaviour
	{
		private CarAnimationReferences _carAnimationReferences;

		[field: SerializeField]
		public Animator LeftSkirtsAnimator { get; private set; }

		[field: SerializeField]
		public Animator RightSkirtsAnimator { get; private set; }

		[field: SerializeField]
		public bool EveryDoor { get; private set; }

		[field: SerializeField]
		public Animator LeftBackSkirtsAnimator { get; private set; }

		[field: SerializeField]
		public Animator RightBackSkirtsAnimator { get; private set; }

		public void Init(CarAnimationReferences carAnimationReferences)
		{
			if ((bool)carAnimationReferences && ((bool)LeftSkirtsAnimator || (bool)RightSkirtsAnimator))
			{
				_carAnimationReferences = carAnimationReferences;
				_carAnimationReferences.SetupSkirts(LeftSkirtsAnimator, RightSkirtsAnimator, EveryDoor, LeftBackSkirtsAnimator, RightBackSkirtsAnimator);
			}
		}

		private void OnDestroy()
		{
			if ((bool)_carAnimationReferences)
			{
				_carAnimationReferences.ResetSkirts(LeftSkirtsAnimator, RightSkirtsAnimator, LeftBackSkirtsAnimator, RightBackSkirtsAnimator);
			}
		}
	}
}
