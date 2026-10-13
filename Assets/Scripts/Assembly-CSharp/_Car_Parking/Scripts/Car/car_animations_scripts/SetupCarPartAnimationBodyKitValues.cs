using System.Collections.Generic;
using CarAnimationScripts;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.car_animations_scripts
{
	public class SetupCarPartAnimationBodyKitValues : MonoBehaviour
	{
		[field: SerializeField]
		public List<CarPartAnimationSequence> OpenSequence { get; private set; }

		[field: SerializeField]
		public List<CarPartAnimationSequence> CloseSequence { get; private set; }
	}
}
