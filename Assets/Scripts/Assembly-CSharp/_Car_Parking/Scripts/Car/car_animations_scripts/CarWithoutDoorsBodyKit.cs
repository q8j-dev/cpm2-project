using System.Collections.Generic;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.car_animations_scripts
{
	public class CarWithoutDoorsBodyKit : MonoBehaviour
	{
		[SerializeField]
		private List<NewSitGetOutAnimationsLengthSetup> NewAnimationsLength = new List<NewSitGetOutAnimationsLengthSetup>();

		[SerializeField]
		private List<CarDoorType> CarDoorTypesToDisable = new List<CarDoorType>();

		private CarAnimationReferences _carAnimationReferences;

		public Dictionary<CarAnimationType, NewSitGetOutAnimationsLengthSetup> DoorsAnimationsLength { get; private set; } = new Dictionary<CarAnimationType, NewSitGetOutAnimationsLengthSetup>();

		private void Awake()
		{
			foreach (NewSitGetOutAnimationsLengthSetup item in NewAnimationsLength)
			{
				DoorsAnimationsLength.Add(item.CarAnimationType, item);
			}
		}

		public void Init(CarAnimationReferences carAnimationReferences)
		{
			_carAnimationReferences = carAnimationReferences;
			foreach (CarDoorType item in CarDoorTypesToDisable)
			{
				if ((bool)_carAnimationReferences.CarDoorAnimators[item])
				{
					_carAnimationReferences.CarDoorAnimators[item].enabled = false;
				}
			}
		}

		private void OnDestroy()
		{
			if (!_carAnimationReferences)
			{
				return;
			}
			foreach (CarDoorType item in CarDoorTypesToDisable)
			{
				if ((bool)_carAnimationReferences.CarDoorAnimators[item])
				{
					_carAnimationReferences.CarDoorAnimators[item].enabled = true;
				}
			}
			_carAnimationReferences.CarWithoutDoorsBodyKit = null;
		}
	}
}
