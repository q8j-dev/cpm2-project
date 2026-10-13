using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	public static class WorldPoseBuilder
	{
		public static WorldPoseTargets CreatePool()
		{
			GameObject gameObject = new GameObject("WorldPoseAnchor");
			WorldPoseTargets obj = new WorldPoseTargets
			{
				Anchor = gameObject.transform
			};
			obj.Body = Child(obj.Anchor, "Body");
			obj.LeftFoot = Child(obj.Anchor, "LFoot");
			obj.RightFoot = Child(obj.Anchor, "RFoot");
			obj.LeftHand = Child(obj.Anchor, "LHand");
			obj.RightHand = Child(obj.Anchor, "RHand");
			obj.LeftShoulder = Child(obj.Anchor, "LSh");
			obj.RightShoulder = Child(obj.Anchor, "RSh");
			obj.LeftThigh = Child(obj.Anchor, "LThigh");
			obj.RightThigh = Child(obj.Anchor, "RThigh");
			obj.LeftElbow = Child(obj.Anchor, "LElbow");
			obj.RightElbow = Child(obj.Anchor, "RElbow");
			obj.LeftKnee = Child(obj.Anchor, "LKnee");
			obj.RightKnee = Child(obj.Anchor, "RKnee");
			return obj;
		}

		public static void Apply(WorldPoseTargets t, WorldPoseData pose, Vector3 surfacePoint, Quaternion facing)
		{
			t.Anchor.SetPositionAndRotation(surfacePoint, facing);
			Set(t.Body, pose.BodyOffset, pose.BodyEulerOffset);
			Set(t.LeftFoot, pose.LeftFootOffset, pose.LeftFootEuler);
			Set(t.RightFoot, pose.RightFootOffset, pose.RightFootEuler);
			Set(t.LeftHand, pose.LeftHandOffset, pose.LeftHandEuler);
			Set(t.RightHand, pose.RightHandOffset, pose.RightHandEuler);
			Set(t.LeftShoulder, pose.LeftShoulderOffset);
			Set(t.RightShoulder, pose.RightShoulderOffset);
			Set(t.LeftThigh, pose.LeftThighOffset);
			Set(t.RightThigh, pose.RightThighOffset);
			Set(t.LeftElbow, pose.LeftElbowOffset);
			Set(t.RightElbow, pose.RightElbowOffset);
			Set(t.LeftKnee, pose.LeftKneeOffset);
			Set(t.RightKnee, pose.RightKneeOffset);
		}

		public static WorldPoseTargets Build(WorldPoseData pose, Vector3 surfacePoint, Quaternion facing)
		{
			WorldPoseTargets worldPoseTargets = CreatePool();
			Apply(worldPoseTargets, pose, surfacePoint, facing);
			return worldPoseTargets;
		}

		public static void ReadOffsets(WorldPoseTargets t, WorldPoseData pose)
		{
			ReadPR(t.Body, ref pose.BodyOffset, ref pose.BodyEulerOffset);
			ReadPR(t.LeftFoot, ref pose.LeftFootOffset, ref pose.LeftFootEuler);
			ReadPR(t.RightFoot, ref pose.RightFootOffset, ref pose.RightFootEuler);
			ReadPR(t.LeftHand, ref pose.LeftHandOffset, ref pose.LeftHandEuler);
			ReadPR(t.RightHand, ref pose.RightHandOffset, ref pose.RightHandEuler);
			ReadP(t.LeftShoulder, ref pose.LeftShoulderOffset);
			ReadP(t.RightShoulder, ref pose.RightShoulderOffset);
			ReadP(t.LeftThigh, ref pose.LeftThighOffset);
			ReadP(t.RightThigh, ref pose.RightThighOffset);
			ReadP(t.LeftElbow, ref pose.LeftElbowOffset);
			ReadP(t.RightElbow, ref pose.RightElbowOffset);
			ReadP(t.LeftKnee, ref pose.LeftKneeOffset);
			ReadP(t.RightKnee, ref pose.RightKneeOffset);
		}

		private static void ReadP(Transform t, ref Vector3 pos)
		{
			if ((bool)t)
			{
				pos = t.localPosition;
			}
		}

		private static void ReadPR(Transform t, ref Vector3 pos, ref Vector3 euler)
		{
			if ((bool)t)
			{
				pos = t.localPosition;
				euler = t.localRotation.eulerAngles;
			}
		}

		public static void ClampFeetToSurface(WorldPoseTargets t, WorldInteractionSettings s, Transform person)
		{
			Clamp(t.LeftFoot, s, person);
			Clamp(t.RightFoot, s, person);
		}

		private static void Clamp(Transform foot, WorldInteractionSettings s, Transform person)
		{
			if (!foot)
			{
				return;
			}
			Vector3 origin = foot.position + Vector3.up * s.FootProbeUp;
			RaycastHit[] array = new RaycastHit[8];
			int num = Physics.RaycastNonAlloc(origin, Vector3.down, array, s.FootProbeUp + s.FootProbeDown, s.FootProbeMask, QueryTriggerInteraction.Ignore);
			float num2 = float.NegativeInfinity;
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				if ((!person || !array[i].collider.transform.IsChildOf(person)) && array[i].point.y > num2)
				{
					num2 = array[i].point.y;
					flag = true;
				}
			}
			if (flag)
			{
				float num3 = num2 + s.FootSurfaceClearance;
				if (!(foot.position.y >= num3))
				{
					foot.position += new Vector3(0f, num3 - foot.position.y, 0f);
				}
			}
		}

		private static Transform Child(Transform parent, string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			return gameObject.transform;
		}

		private static void Set(Transform t, Vector3 localPos, Vector3 localEuler = default(Vector3))
		{
			if ((bool)t)
			{
				t.localPosition = localPos;
				t.localRotation = Quaternion.Euler(localEuler);
			}
		}
	}
}
