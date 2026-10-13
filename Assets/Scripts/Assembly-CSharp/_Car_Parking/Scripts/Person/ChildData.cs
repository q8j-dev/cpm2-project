using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Person
{
	[Serializable]
	public class ChildData
	{
		public Transform t;

		public string nameTemp;

		public Vector3 localPosition;

		public Quaternion localRotation;

		public ChildData(Transform transform)
		{
			t = transform;
			nameTemp = transform.gameObject.name;
			localPosition = t.localPosition;
			localRotation = t.localRotation;
		}

		public void FixTransform(float weight)
		{
			if ((bool)t && !(weight <= 0f))
			{
				if (weight >= 1f)
				{
					t.localPosition = localPosition;
					t.localRotation = localRotation;
				}
				else
				{
					t.localPosition = Vector3.Lerp(t.localPosition, localPosition, weight);
					t.localRotation = Quaternion.Lerp(t.localRotation, localRotation, weight);
				}
			}
		}

		public void StoreLocalState()
		{
			localPosition = t.localPosition;
			localRotation = t.localRotation;
		}
	}
}
