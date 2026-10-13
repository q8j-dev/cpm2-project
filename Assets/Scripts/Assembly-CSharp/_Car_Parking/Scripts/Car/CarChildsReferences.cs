using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	public class CarChildsReferences : MonoBehaviour
	{
		[field: SerializeField]
		public Transform MCam { get; private set; }

		[field: SerializeField]
		public Transform RCam { get; private set; }

		[field: SerializeField]
		public Transform LCam { get; private set; }

		[field: SerializeField]
		public Transform Cog { get; private set; }

		[field: SerializeField]
		public Transform Wheels { get; private set; }

		[field: SerializeField]
		public Transform WheelFL { get; private set; }

		[field: SerializeField]
		public Transform WheelFR { get; private set; }

		[field: SerializeField]
		public Transform WheelRL { get; private set; }

		[field: SerializeField]
		public Transform WheelRR { get; private set; }

		[field: SerializeField]
		public Transform WheelRL1 { get; private set; }

		[field: SerializeField]
		public Transform WheelRR1 { get; private set; }

		[field: SerializeField]
		public Transform WheelRL2 { get; private set; }

		[field: SerializeField]
		public Transform WheelRR2 { get; private set; }

		[field: SerializeField]
		public Transform Collider { get; private set; }

		[field: SerializeField]
		public Transform ShadowDecalURP { get; private set; }

		[field: SerializeField]
		public Transform PointWheel { get; private set; }

		[field: SerializeField]
		public Transform[] Exhaust { get; private set; }

		[field: SerializeField]
		public Transform FuelSlot { get; set; }

		[field: SerializeField]
		public FuelTankEntity FuelTank { get; set; }

		[field: SerializeField]
		public Transform Trailer { get; set; }

		public void UpdateExhaustsReferences(Transform[] newExhausts)
		{
			if (!Exhaust.IsNullOrEmpty())
			{
				for (int i = 0; i < Exhaust.Length; i++)
				{
					if ((bool)Exhaust[i])
					{
						Object.Destroy(Exhaust[i].gameObject);
					}
				}
			}
			foreach (Transform transform in newExhausts)
			{
				if ((bool)transform)
				{
					transform.transform.SetParent(base.transform);
				}
			}
			Exhaust = newExhausts;
		}
	}
}
