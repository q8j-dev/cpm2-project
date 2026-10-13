using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class ProgressionRotation : ProgressionBase
	{
		public Vector3 EndRotation;

		public override float Progression
		{
			get
			{
				return Vector3.Angle(Vector3.zero, base.transform.localEulerAngles) / (Vector3.Angle(Vector3.zero, Utils.FixedVector(EndRotation)) + float.Epsilon);
			}
			set
			{
				base.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Utils.FixedVector(EndRotation), value);
			}
		}
	}
}
