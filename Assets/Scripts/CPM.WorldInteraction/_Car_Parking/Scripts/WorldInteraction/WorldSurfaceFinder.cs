using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	public class WorldSurfaceFinder
	{
		public enum SurfaceSnapMethod
		{
			Mesh = 0,
			Bounds = 1,
			ColliderClosest = 2,
			CastContact = 3
		}

		private readonly WorldInteractionSettings _settings;

		private readonly RaycastHit[] _castHits = new RaycastHit[16];

		public WorldSurfaceFinder(WorldInteractionSettings settings)
		{
			_settings = settings;
		}

		public static bool AcceptsHeight(float y, float baseY, float min, float max)
		{
			float num = y - baseY;
			if (num >= min)
			{
				return num <= max;
			}
			return false;
		}

		public static bool AcceptsNormal(Vector3 normal, float maxAngle)
		{
			return Vector3.Angle(normal, Vector3.up) <= maxAngle;
		}

		public static bool AcceptsFlatness(float spread, float maxSpread)
		{
			return spread <= maxSpread;
		}

		public static WorldSurfaceHit BuildInPlaceHit(Transform person, WorldPoseData pose)
		{
			return new WorldSurfaceHit
			{
				Valid = true,
				SurfacePoint = person.position,
				Facing = Quaternion.LookRotation(Flat(person.forward), Vector3.up),
				ApproachPoint = person.position,
				Pose = pose,
				Collider = null
			};
		}

		public WorldSurfaceHit TryFind(Transform person, int id, WorldInteractionConfig cfg)
		{
			WorldInteractionConfig.TypeSettings settings = cfg.GetSettings(id);
			WorldPoseData worldPoseData = ((settings.DefaultPose != null) ? settings.DefaultPose.Clone() : new WorldPoseData
			{
				Id = id
			});
			worldPoseData.Id = id;
			if (settings.IsInPlace)
			{
				return BuildInPlaceHit(person, worldPoseData);
			}
			WorldSurfaceHit result = TryFindMarker(person, id, worldPoseData);
			if (result.Valid)
			{
				return result;
			}
			if (!settings.IsVertical)
			{
				return TryFindHorizontal(person, settings, worldPoseData);
			}
			return TryFindVertical(person, settings, worldPoseData);
		}

		private WorldSurfaceHit TryFindMarker(Transform person, int id, WorldPoseData pose)
		{
			return WorldSurfaceHit.None;
		}

		private WorldSurfaceHit TryFindVertical(Transform person, WorldInteractionConfig.TypeSettings st, WorldPoseData pose)
		{
			QueryTriggerInteraction queryTriggerInteraction = ((!st.IncludeTriggerSurfaces) ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
			Vector3 vector = Flat(person.forward);
			Vector3 searchBoxHalfExtents = _settings.SearchBoxHalfExtents;
			Quaternion orientation = Quaternion.LookRotation(vector, Vector3.up);
			int num = Physics.BoxCastNonAlloc(person.position + Vector3.up * _settings.LeanSearchHeight, searchBoxHalfExtents, vector, _castHits, orientation, _settings.SearchDistance, _settings.SurfaceMask, queryTriggerInteraction);
			float num2 = _settings.SearchDistance * _settings.SearchDistance;
			WorldSurfaceHit result = WorldSurfaceHit.None;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = _castHits[i];
				Collider collider = raycastHit.collider;
				if ((bool)collider && (!person || !collider.transform.IsChildOf(person)) && !(Vector3.Angle(raycastHit.normal, Vector3.up) < 70f))
				{
					float sqrMagnitude = (raycastHit.point - person.position).sqrMagnitude;
					if (!(sqrMagnitude >= num2))
					{
						_ = person.position + Vector3.up * _settings.LeanSearchHeight;
						Vector3 vector2 = ClosestSurfacePoint(collider, raycastHit.point, _settings.MeshSnapMaxDistance, out var _, out var _) + raycastHit.normal * _settings.LeanStandOff;
						vector2.y = person.position.y + _settings.LeanChestHeight - _settings.LeanHipHeight;
						Quaternion quaternion = Quaternion.LookRotation(Flat(raycastHit.normal), Vector3.up);
						Vector3 vector3 = new Vector3(vector2.x, person.position.y, vector2.z);
						num2 = sqrMagnitude;
						result = new WorldSurfaceHit
						{
							Valid = true,
							SurfacePoint = vector3 - Flat(quaternion * Vector3.forward) * pose.ApproachDistance,
							Facing = quaternion,
							ApproachPoint = vector3 - Flat(quaternion * Vector3.forward) * pose.ApproachDistance,
							Pose = pose,
							Collider = raycastHit.collider
						};
					}
				}
			}
			return result;
		}

		private WorldSurfaceHit TryFindHorizontal(Transform person, WorldInteractionConfig.TypeSettings st, WorldPoseData pose)
		{
			QueryTriggerInteraction queryTriggerInteraction = ((!st.IncludeTriggerSurfaces) ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
			Vector3 vector = Flat(person.forward);
			Vector3 searchBoxHalfExtents = _settings.SearchBoxHalfExtents;
			Quaternion orientation = Quaternion.LookRotation(vector, Vector3.up);
			int num = Physics.BoxCastNonAlloc(person.position + Vector3.up * (searchBoxHalfExtents.y + 0.1f), searchBoxHalfExtents, vector, _castHits, orientation, _settings.SearchDistance, _settings.SurfaceMask, queryTriggerInteraction);
			float num2 = float.PositiveInfinity;
			WorldSurfaceHit result = WorldSurfaceHit.None;
			for (int i = 0; i < num; i++)
			{
				Collider collider = _castHits[i].collider;
				if (!collider || ((bool)person && collider.transform.IsChildOf(person)))
				{
					continue;
				}
				Vector3 point = _castHits[i].point;
				if (!TryProbeTop(person, collider, st, point, out var top, out var normal, out var _))
				{
					continue;
				}
				Vector3 lhs = top - person.position;
				lhs.y = 0f;
				if (Vector3.Dot(lhs, vector) <= 0f || lhs.magnitude > _settings.SearchDistance || !AcceptsHeight(top.y, person.position.y, st.MinHeight, st.MaxHeight) || !AcceptsNormal(normal, st.MaxNormalAngle))
				{
					continue;
				}
				float sqrMagnitude = (top - person.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					Quaternion facing = FaceAwayFromApproach(person.position, top);
					Vector3 candidate = NearestEdgeSeat(person.position, top, point, collider, _settings.SeatEdgeInset);
					Vector3 vector2 = ClosestSurfacePoint(collider, candidate, _settings.MeshSnapMaxDistance, out var _, out var _);
					vector2.y = top.y;
					if (HasClearanceAbove(vector2))
					{
						num2 = sqrMagnitude;
						result = new WorldSurfaceHit
						{
							Valid = true,
							SurfacePoint = vector2,
							Facing = facing,
							ApproachPoint = vector2,
							Pose = pose,
							Collider = collider
						};
					}
				}
			}
			return result;
		}

		private bool TryProbeTop(Transform person, Collider col, WorldInteractionConfig.TypeSettings st, Vector3 castHit, out Vector3 top, out Vector3 normal, out float spread)
		{
			top = default;
			normal = Vector3.up;
			spread = 0f;
			float y = Mathf.Min(col.bounds.max.y + 0.2f, person.position.y + st.MaxHeight + 0.3f);
			castHit.y = y;
			Vector3 position = person.position;
			position.y = castHit.y;
			Vector3 normalized = (castHit - position).normalized;
			Vector3[] obj = new Vector3[5]
			{
				castHit + normalized * 0.01f,
				castHit + normalized * 0.02f,
				castHit + normalized * 0.05f,
				castHit + normalized * 0.1f,
				castHit + normalized * 0.2f
			};
			float num = float.PositiveInfinity;
			float num2 = float.NegativeInfinity;
			bool flag = false;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			int num3 = 0;
			Vector3[] array = obj;
			foreach (Vector3 origin in array)
			{
				if (col.Raycast(new Ray(origin, Vector3.down), out var hitInfo, 30f))
				{
					flag = true;
					zero += hitInfo.point;
					zero2 += hitInfo.normal;
					num3++;
					num = Mathf.Min(num, hitInfo.point.y);
					num2 = Mathf.Max(num2, hitInfo.point.y);
				}
			}
			if (!flag)
			{
				return false;
			}
			top = zero / num3;
			normal = (zero2 / num3).normalized;
			spread = num2 - num;
			top.y = num2;
			return true;
		}

		private bool HasClearanceAbove(Vector3 top)
		{
			return !Physics.CheckBox(top + Vector3.up * (_settings.ClearanceHalfExtents.y + 0.05f), _settings.ClearanceHalfExtents, Quaternion.identity, _settings.SurfaceMask, QueryTriggerInteraction.Ignore);
		}

		private static Quaternion FaceAwayFromApproach(Vector3 personPos, Vector3 top)
		{
			return Quaternion.LookRotation(Flat(personPos - top), Vector3.up);
		}

		private static Vector3 NearestEdgeSeat(Vector3 personPos, Vector3 top, Vector3 castHit, Collider col, float inset)
		{
			Vector3 vector = Flat(castHit - personPos);
			Vector3 vector2 = castHit + vector * inset;
			vector2.y = top.y;
			if (col is MeshCollider { convex: false })
			{
				if (!OnTop(col, vector2, top.y))
				{
					return castHit;
				}
				return vector2;
			}
			Vector3 result = (col ? col.ClosestPoint(vector2) : vector2);
			result.y = top.y;
			return result;
		}

		private static bool OnTop(Collider col, Vector3 p, float y)
		{
			Vector3 origin = new Vector3(p.x, y + 0.5f, p.z);
			RaycastHit hitInfo;
			if ((bool)col)
			{
				return col.Raycast(new Ray(origin, Vector3.down), out hitInfo, 1f);
			}
			return false;
		}

		private static Vector3 ClosestSurfacePoint(Collider col, Vector3 candidate, float maxSnap, out SurfaceSnapMethod method, out string source)
		{
			method = SurfaceSnapMethod.CastContact;
			source = (col ? col.name : "<null>");
			if (!col)
			{
				return candidate;
			}
			MeshRenderer componentInChildren = col.GetComponentInChildren<MeshRenderer>();
			Vector3 vector;
			if ((bool)componentInChildren)
			{
				Mesh mesh = (componentInChildren.TryGetComponent<MeshFilter>(out var component) ? component.sharedMesh : null);
				if ((bool)mesh && mesh.isReadable && TryClosestOnMesh(component, mesh, candidate, out var closest))
				{
					vector = closest;
					method = SurfaceSnapMethod.Mesh;
					source = componentInChildren.name;
				}
				else
				{
					vector = candidate;
					method = SurfaceSnapMethod.Bounds;
					source = componentInChildren.name;
				}
			}
			else
			{
				if (col is MeshCollider { convex: false })
				{
					return candidate;
				}
				vector = col.ClosestPoint(candidate);
				method = SurfaceSnapMethod.ColliderClosest;
			}
			if ((vector - candidate).sqrMagnitude > maxSnap * maxSnap)
			{
				method = SurfaceSnapMethod.CastContact;
				return candidate;
			}
			return vector;
		}

		private static bool TryClosestOnMesh(MeshFilter mf, Mesh mesh, Vector3 worldPoint, out Vector3 closest)
		{
			closest = default;
			if (!mf || !mesh)
			{
				return false;
			}
			Vector3[] vertices = mesh.vertices;
			int[] triangles = mesh.triangles;
			if (vertices.Length == 0 || triangles.Length == 0)
			{
				return false;
			}
			Transform transform = mf.transform;
			Vector3 vector = transform.InverseTransformPoint(worldPoint);
			float num = float.PositiveInfinity;
			Vector3 position = vector;
			for (int i = 0; i + 2 < triangles.Length; i += 3)
			{
				Vector3 vector2 = ClosestOnTriangle(vector, vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
				float sqrMagnitude = (vector2 - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					position = vector2;
				}
			}
			closest = transform.TransformPoint(position);
			return true;
		}

		private static Vector3 ClosestOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 vector = b - a;
			Vector3 vector2 = c - a;
			Vector3 rhs = p - a;
			float num = Vector3.Dot(vector, rhs);
			float num2 = Vector3.Dot(vector2, rhs);
			if (num <= 0f && num2 <= 0f)
			{
				return a;
			}
			Vector3 rhs2 = p - b;
			float num3 = Vector3.Dot(vector, rhs2);
			float num4 = Vector3.Dot(vector2, rhs2);
			if (num3 >= 0f && num4 <= num3)
			{
				return b;
			}
			float num5 = num * num4 - num3 * num2;
			if (num5 <= 0f && num >= 0f && num3 <= 0f)
			{
				return a + num / (num - num3) * vector;
			}
			Vector3 rhs3 = p - c;
			float num6 = Vector3.Dot(vector, rhs3);
			float num7 = Vector3.Dot(vector2, rhs3);
			if (num7 >= 0f && num6 <= num7)
			{
				return c;
			}
			float num8 = num6 * num2 - num * num7;
			if (num8 <= 0f && num2 >= 0f && num7 <= 0f)
			{
				return a + num2 / (num2 - num7) * vector2;
			}
			float num9 = num3 * num7 - num6 * num4;
			if (num9 <= 0f && num4 - num3 >= 0f && num6 - num7 >= 0f)
			{
				return b + (num4 - num3) / (num4 - num3 + (num6 - num7)) * (c - b);
			}
			float num10 = 1f / (num9 + num8 + num5);
			return a + vector * (num8 * num10) + vector2 * (num5 * num10);
		}

		private static Vector3 Flat(Vector3 v)
		{
			v.y = 0f;
			if (!(v.sqrMagnitude < 0.0001f))
			{
				return v.normalized;
			}
			return Vector3.forward;
		}
	}
}
