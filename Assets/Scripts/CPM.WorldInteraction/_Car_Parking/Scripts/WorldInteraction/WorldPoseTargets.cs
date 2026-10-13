using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	public class WorldPoseTargets
	{
		public Transform Anchor;

		public Transform Body;

		public Transform LeftFoot;

		public Transform RightFoot;

		public Transform LeftHand;

		public Transform RightHand;

		public Transform LeftShoulder;

		public Transform RightShoulder;

		public Transform LeftThigh;

		public Transform RightThigh;

		public Transform LeftElbow;

		public Transform RightElbow;

		public Transform LeftKnee;

		public Transform RightKnee;

		public void Destroy()
		{
			if (Anchor != null)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(Anchor.gameObject);
				}
				else
				{
					Object.DestroyImmediate(Anchor.gameObject);
				}
			}
			Anchor = (Body = (LeftFoot = (RightFoot = (LeftHand = (RightHand = (LeftShoulder = (RightShoulder = (LeftThigh = (RightThigh = (LeftElbow = (RightElbow = (LeftKnee = (RightKnee = null)))))))))))));
		}
	}
}
