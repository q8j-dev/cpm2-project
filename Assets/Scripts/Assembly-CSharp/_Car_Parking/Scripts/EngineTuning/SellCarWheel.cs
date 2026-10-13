using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.EngineTuning
{
	public class SellCarWheel : MonoBehaviour
	{
		public WheelModel WheelModel;

		public GameObject Model;

		private const float SellCarOffset = 0.015f;

		private const float RimRadiusToMaxDiameter = 1.9f;

		private float _currentDiameter;

		private float _rimRadius = -1f;

		private float _brakeDiskDiameter;

		public Material RimMaterial1;

		public Material RimMaterial2;

		public bool IsLeft => base.transform.parent.InverseTransformPoint(base.transform.position).x < 0f;

		private bool IsFront => base.transform.parent.InverseTransformPoint(base.transform.position).z > 0f;

		private float MaxDiameter => _rimRadius * 1.9f;

		public void SetSize(TyreWidth tyreWidth, TyreProfile tyreProfile, WheelDiameter rimDiameter, RimWidth RimWidth, SellCarValues sellCar)
		{
			float num = (float)tyreWidth * 0.001f;
			float num2 = (float)tyreProfile;
			float num3 = (float)rimDiameter * 0.0127f + num * num2 * 0.01f;
			float rimRadius = (float)rimDiameter * 0.0127f;
			float rimWidth = (float)RimWidth * 0.00254f;
			WheelModel.SetSize(num3, num, rimRadius, rimWidth, isOnline: false);
			if (IsFront)
			{
				if (sellCar.IsFrontFloorChecked)
				{
					return;
				}
			}
			else if (sellCar.IsRearFloorChecked)
			{
				return;
			}
			int mask = LayerMask.GetMask("Default", "Road");
			RaycastHit[] array = new RaycastHit[5];
			int num4 = Physics.RaycastNonAlloc(base.transform.position, Vector3.down, array, num3 + 0.5f, mask);
			if (num4 <= 0)
			{
				return;
			}
			if (IsFront)
			{
				sellCar.IsFrontFloorChecked = true;
			}
			else
			{
				sellCar.IsRearFloorChecked = true;
			}
			Vector3 point = array[0].point;
			for (int i = 1; i < num4; i++)
			{
				if (array[i].point.y > point.y)
				{
					point = array[i].point;
				}
			}
			Vector3 vector = base.transform.position + Vector3.down * (num3 - 0.015f);
			Vector3 vector2 = point - vector;
			if (IsFront)
			{
				sellCar.FrontHeight = vector2.y;
			}
			else
			{
				sellCar.RearHeight = vector2.y;
			}
		}

		private void OnDestroy()
		{
			CommonExtensions.DestroyMat(ref RimMaterial1);
			CommonExtensions.DestroyMat(ref RimMaterial2);
		}
	}
}
