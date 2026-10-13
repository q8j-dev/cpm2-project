using UnityEngine;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts
{
	public class SteeringWheelHandPositions : MonoBehaviour
	{
		[SerializeField]
		private Transform TopPosition;

		[SerializeField]
		private Transform MiddlePosition;

		[SerializeField]
		private Transform BottomPosition;

		[SerializeField]
		private Transform Root;

		public void Init(HandIKTarget handIkTarget)
		{
			handIkTarget.transform.SetParent(Root ? Root : base.transform);
		}

		public void UpdatePosition(float steering)
		{
			if ((bool)Root)
			{
				if (steering < 0f)
				{
					Root.localPosition = Vector3.Lerp(MiddlePosition.localPosition, BottomPosition.localPosition, 0f - steering);
					Root.localRotation = Quaternion.Lerp(MiddlePosition.localRotation, BottomPosition.localRotation, 0f - steering);
				}
				else
				{
					Root.localPosition = Vector3.Lerp(MiddlePosition.localPosition, TopPosition.localPosition, steering);
					Root.localRotation = Quaternion.Lerp(MiddlePosition.localRotation, TopPosition.localRotation, steering);
				}
			}
		}

		private void Awake()
		{
			if ((bool)Root)
			{
				Root.localPosition = MiddlePosition.localPosition;
				Root.localRotation = MiddlePosition.localRotation;
			}
		}
	}
}
