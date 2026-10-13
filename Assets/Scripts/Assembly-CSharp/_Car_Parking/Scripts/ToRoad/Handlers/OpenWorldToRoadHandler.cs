using GPS;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.ToRoad.Handlers
{
	public sealed class OpenWorldToRoadHandler : IToRoadHandler
	{
		private const float PersonSideOffset = 2f;

		private const float PersonHeightOffset = -0.2f;

		private const float NodeHeightOffset = 3f;

		private const float RoadProbeRayLength = 10f;

		private const float PersonProbeUpOffset = 2f;

		private const float PersonProbeRayLength = 10f;

		private const int City2TargetIndex = 630;

		private const int TargetIndexBacktrack = 15;

		private readonly FreeDriveProvider _freeDriveProvider;

		public ToRoadPriority Priority => ToRoadPriority.Fallback;

		public OpenWorldToRoadHandler(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		public bool TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			result = (request.MovesPerson ? ResolveOnFoot(in request) : ResolveInCar(in request));
			return true;
		}

		private ToRoadResult ResolveOnFoot(in ToRoadRequest request)
		{
			GameObject gameObject = (_freeDriveProvider.MultiPlayer ? _freeDriveProvider.MultiPlayer.GetEmptyCar(null) : null);
			if (!gameObject)
			{
				gameObject = _freeDriveProvider.FirstCarMultiPlayer;
			}
			if (!gameObject)
			{
				return ToRoadResult.Place(request.Person.transform.position, request.Person.transform.rotation);
			}
			VehicleData component = gameObject.GetComponent<VehicleData>();
			float num = (((bool)component && component.RightHanded) ? 2f : (-2f));
			return ToRoadResult.Place(LiftAboveGround(gameObject.transform.position + gameObject.transform.right * num + new Vector3(0f, -0.2f, 0f)), request.Person.transform.rotation);
		}

		private ToRoadResult ResolveInCar(in ToRoadRequest request)
		{
			int num = GPS.GPSTracker.FindRandomClosestPoint(V.DriverPerson.transform.localPosition);
			if (num == -1)
			{
				return ResolveBySpawnPoints(request.Car);
			}
			return ResolveByRoadGraph(request.Car, num);
		}

		private ToRoadResult ResolveByRoadGraph(GameObject car, int depID)
		{
			NodePosition position = GPS.GPSTracker.nodes[depID].position;
			Vector3 toRoadPoint = new Vector3(position.x, position.y, position.z) + V.GlobalOffset + Vector3.up * 3f;
			Vector3 vector = SnapToRoad(car, toRoadPoint);
			uint[] next = GPS.GPSTracker.nodes[depID].next;
			if (next == null || next.Length == 0)
			{
				return ToRoadResult.Place(vector, Quaternion.identity);
			}
			position = GPS.GPSTracker.nodes[(int)next[0]].position;
			Vector3 toRoadPoint2 = new Vector3(position.x, position.y, position.z) + V.GlobalOffset + Vector3.up * 3f;
			Vector3 forward = SnapToRoad(car, toRoadPoint2) - vector;
			Quaternion rotation = ((forward.sqrMagnitude > Mathf.Epsilon) ? Quaternion.LookRotation(forward) : Quaternion.identity);
			return ToRoadResult.Place(vector, rotation);
		}

		private ToRoadResult ResolveBySpawnPoints(GameObject car)
		{
			(Transform[] points, int IndexArrayPostionCar) spawnPoints = V.SceneControl.SpawnPointsContainer.GetSpawnPoints(V.PlaceToSpawn);
			Transform[] item = spawnPoints.points;
			V.CurrentIndexArrayPositionCar = spawnPoints.IndexArrayPostionCar;
			FreeDriveControl freeDriveControl = _freeDriveProvider.FreeDriveControl;
			if (V.CurrentEnvironment == V.placeToSpawn.City2 && (bool)freeDriveControl)
			{
				freeDriveControl.current_tar_City2 = 630;
			}
			V.CurrentIndexArrayPositionCar -= 15;
			Transform randomSpawnPoint = item.GetRandomSpawnPoint(out var forceUsePoint);
			Vector3 vector = randomSpawnPoint.position;
			if (!forceUsePoint)
			{
				vector = SpawnPointRandomizer.CheckPoints(vector, item);
			}
			vector.y++;
			vector = SnapToRoad(car, vector);
			car.GetComponent<VehicleData>().KeepRowComposition.CurrentTargetIndex = V.CurrentIndexArrayPositionCar;
			return ToRoadResult.Place(vector, Quaternion.Euler(randomSpawnPoint.eulerAngles));
		}

		private static Vector3 LiftAboveGround(Vector3 pos)
		{
			int num = LayerMaskOf(LayerViaName.RoadLayer) | LayerMaskOf(LayerViaName.TerrainLayer) | LayerMaskOf(LayerViaName.DefaultLayer);
			if (num == 0)
			{
				return pos;
			}
			if (Physics.Raycast(pos + Vector3.up * 2f, Vector3.down, out var hitInfo, 12f, num, QueryTriggerInteraction.Ignore) && pos.y < hitInfo.point.y)
			{
				pos.y = hitInfo.point.y;
			}
			return pos;
		}

		private static int LayerMaskOf(int layer)
		{
			if (layer < 0)
			{
				return 0;
			}
			return 1 << layer;
		}

		private static Vector3 SnapToRoad(GameObject car, Vector3 toRoadPoint)
		{
			if (Physics.Raycast(toRoadPoint, Vector3.down, out var hitInfo, 10f, 1 << LayerViaName.RoadLayer))
			{
				return VectorExtensions.GetPosByWheel(car, hitInfo.point);
			}
			return toRoadPoint;
		}

		bool IToRoadHandler.TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			return TryResolve(in request, out result);
		}
	}
}
