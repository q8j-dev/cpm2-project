using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	public class CarRetractableDisplay : MonoBehaviour
	{
		private static readonly int IsDisplayActive = Animator.StringToHash("isDisplayActive");

		public Animator DisplayAnimator { get; set; }

		public void ShowHideDisplay(bool isActive)
		{
			if ((bool)base.gameObject)
			{
				base.gameObject.SetActive(isActive);
			}
		}

		public void SwitchOnOffCarDisplayAnimation(bool isActive)
		{
			if ((bool)DisplayAnimator)
			{
				DisplayAnimator.SetBool(IsDisplayActive, isActive);
			}
		}

		public void PlaySlideCarDisplaySound()
		{
		}

		public void PlayFoldCarDisplaySound()
		{
		}
	}
}
