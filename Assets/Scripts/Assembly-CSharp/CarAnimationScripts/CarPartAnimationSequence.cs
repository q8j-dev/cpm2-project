using System;
using CarInteractionAnimationsScripts;
using UnityEngine;

namespace CarAnimationScripts
{
	[Serializable]
	public class CarPartAnimationSequence
	{
		public CarDoorType AnimType;

		[SerializeReference]
		public CarSoundCondition CarSoundCondition;

		public string SoundEvent;

		public float DelayPlaySound;

		public float DelayAfterPlay;

		public bool IsOpen;

		public bool CanPlaySound(VehicleData vehicleData)
		{
			if (CarSoundCondition == null)
			{
				return true;
			}
			return CarSoundCondition.CanPlaySound(vehicleData);
		}

		private void SetSoundCondition(CarSoundCondition.Type conditionType)
		{
			switch (conditionType)
			{
			case CarSoundCondition.Type.None:
				CarSoundCondition = null;
				break;
			case CarSoundCondition.Type.HaveBodyKit:
				CarSoundCondition = new CarSoundConditionHaveBodyKit();
				break;
			}
		}
	}
}
