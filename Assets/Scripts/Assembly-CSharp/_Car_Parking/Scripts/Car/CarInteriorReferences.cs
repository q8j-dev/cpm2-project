using UnityEngine;
using _Car_Parking.Scripts.Car.InteriorSpeedometer;
using _Car_Parking.Scripts.Car.TachometerCustom;

namespace _Car_Parking.Scripts.Car
{
	public class CarInteriorReferences : MonoBehaviour
	{
		[field: SerializeField]
		public Transform Mmirror { get; set; }

		[field: SerializeField]
		public Transform SteeringWheel { get; set; }

		[field: SerializeField]
		public Transform Tacho { get; set; }

		[field: SerializeField]
		public Transform Speedo { get; set; }

		[field: SerializeField]
		public Transform FrontLeftDoorInteriorPart { get; set; }

		[field: SerializeField]
		public Transform FrontRightDoorInteriorPart { get; set; }

		[field: SerializeField]
		public Transform BackLeftDoorInteriorPart { get; set; }

		[field: SerializeField]
		public Transform BackRightDoorInteriorPart { get; set; }

		[field: SerializeField]
		public Transform TrunkFirstDoorInteriorPart { get; set; }

		[field: SerializeField]
		public Transform TrunkSecondDoorInteriorPart { get; set; }

		[field: SerializeField]
		public Transform DigitalGEAR { get; private set; }

		[field: SerializeField]
		public Transform DigitalSPEEDO { get; private set; }

		[field: SerializeField]
		public Transform DigitalTACHO { get; private set; }

		[field: SerializeField]
		public Transform GraphicTACHO { get; private set; }

		[field: SerializeField]
		public MeshRenderer[] MeshRenderers { get; private set; }

		[field: SerializeField]
		public Transform Gimbal { get; private set; }

		[field: SerializeField]
		public Transform RetractableDisplay { get; private set; }

		[field: SerializeField]
		public Transform MiniMap { get; private set; }

		[field: SerializeField]
		public Transform Display { get; private set; }

		[field: SerializeField]
		public Transform Tachometer { get; private set; }

		[field: SerializeField]
		public TachometerHolder TachometerHolder { get; private set; }

		[field: SerializeField]
		public SpeedometerController SpeedometerController { get; private set; }

		[field: SerializeField]
		public TachometerHolderGradient TachometerHolderGradient { get; private set; }
	}
}
