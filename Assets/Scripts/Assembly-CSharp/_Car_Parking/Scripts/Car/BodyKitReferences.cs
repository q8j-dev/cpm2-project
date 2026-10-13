using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	public class BodyKitReferences : MonoBehaviour, IPhysicalWiperSource
	{
		[field: SerializeField]
		public Transform[] Exhaust { get; private set; }

		[field: SerializeField]
		public Transform PlatePos { get; private set; }

		[field: SerializeField]
		public CarBodyReferences.WiperBlade[] WiperBlades { get; set; }

		public bool HasPhysicalWipers => CarBodyReferences.HasBlades(WiperBlades);

		[field: SerializeField]
		public bool HasCustomWindow { get; set; }

		[field: SerializeField]
		public Vector2 WindowShatterHoleSize { get; set; }

		[field: SerializeField]
		public Vector2 WindowShatterHolePosition { get; set; }

		[field: SerializeField]
		public Vector4 WindowWipersTilingOffset { get; set; } = new Vector4(0f, -1.5f, 0.2f, 0.7f);

		[field: SerializeField]
		public float WindowWipersScale { get; set; } = 1.5f;

		[field: SerializeField]
		public CarBodyReferences.WiperAnimationMode WiperMode { get; set; }

		[field: SerializeField]
		public Vector2 WiperShaderRange { get; set; } = new Vector2(0f, 1f);

		[field: SerializeField]
		public bool WindowWipersVerticalInvert { get; set; }

		[field: SerializeField]
		public bool WindowWipersHorizontalInvert { get; set; }

		public void ApplyWiperPose(float value)
		{
			CarBodyReferences.ApplyPose(WiperBlades, value);
		}
	}
}
