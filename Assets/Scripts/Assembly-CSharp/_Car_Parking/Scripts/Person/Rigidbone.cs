using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Person
{
	[Serializable]
	public class Rigidbone
	{
		public Rigidbody r;

		public Transform t;

		public Collider collider;

		public Joint joint;

		public Rigidbody c;

		public bool updateAnchor;

		public Vector3 lastPosition;

		public Quaternion lastRotation;

		public Rigidbone(Rigidbody r)
		{
			this.r = r;
			t = r.transform;
			joint = t.GetComponent<Joint>();
			collider = t.GetComponent<Collider>();
			collider.enabled = false;
			if (joint != null)
			{
				c = joint.connectedBody;
				updateAnchor = c != null;
			}
			lastPosition = t.position;
			lastRotation = t.rotation;
		}

		public void WakeUp(float angularVelocityWeight, Vector3 force)
		{
			if (updateAnchor)
			{
				joint.connectedAnchor = t.InverseTransformPoint(c.position);
			}
			r.isKinematic = false;
			collider.enabled = true;
			r.AddRelativeForce(force);
			r.WakeUp();
		}
	}
}
