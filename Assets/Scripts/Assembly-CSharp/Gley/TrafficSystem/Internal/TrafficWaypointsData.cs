using System;
using System.Collections.Generic;
using System.Linq;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class TrafficWaypointsData : MonoBehaviour, ISerializationCallbackReceiver
	{
		private TrafficWaypoint[] _allTrafficWaypoints;

		[SerializeField]
		private Vector3[] _wp_positions;

		[SerializeField]
		private int[] _wp_maxSpeeds;

		[SerializeField]
		private float[] _wp_laneWidths;

		[SerializeField]
		private int[] _wp_blinkTypes;

		[SerializeField]
		private Vector3[] _wp_leftDirections;

		[SerializeField]
		private int[] _wp_allowedVehiclesMask;

		[SerializeField]
		private byte[] _wp_flags;

		[SerializeField]
		private int[] _wp_neighborsFlat;

		[SerializeField]
		private int[] _wp_neighborCounts;

		[SerializeField]
		private int[] _wp_prevsFlat;

		[SerializeField]
		private int[] _wp_prevCounts;

		[SerializeField]
		private int[] _wp_otherLanesFlat;

		[SerializeField]
		private int[] _wp_otherLaneCounts;

		[SerializeField]
		private int[] _wp_anglesFlat;

		[SerializeField]
		private int[] _wp_giveWayListFlat;

		[SerializeField]
		private int[] _wp_giveWayListCounts;

		public TrafficWaypoint[] AllTrafficWaypoints => _allTrafficWaypoints;

		public TrafficWaypointsData Initialize()
		{
			WaypointEvents.OnTrafficLightChanged += TrafficLightChangedHandler;
			return this;
		}

		public void SetTrafficWaypoints(TrafficWaypoint[] waypoints)
		{
			_allTrafficWaypoints = waypoints;
		}

		public void OnBeforeSerialize()
		{
			if (_allTrafficWaypoints == null || _allTrafficWaypoints.Length == 0)
			{
				_wp_positions = null;
				_wp_maxSpeeds = null;
				_wp_laneWidths = null;
				_wp_blinkTypes = null;
				_wp_leftDirections = null;
				_wp_allowedVehiclesMask = null;
				_wp_flags = null;
				_wp_neighborsFlat = null;
				_wp_neighborCounts = null;
				_wp_prevsFlat = null;
				_wp_prevCounts = null;
				_wp_otherLanesFlat = null;
				_wp_otherLaneCounts = null;
				_wp_anglesFlat = null;
				_wp_giveWayListFlat = null;
				_wp_giveWayListCounts = null;
				return;
			}
			int num = _allTrafficWaypoints.Length;
			_wp_positions = new Vector3[num];
			_wp_maxSpeeds = new int[num];
			_wp_laneWidths = new float[num];
			_wp_blinkTypes = new int[num];
			_wp_leftDirections = new Vector3[num];
			_wp_allowedVehiclesMask = new int[num];
			_wp_flags = new byte[num];
			_wp_neighborCounts = new int[num];
			_wp_prevCounts = new int[num];
			_wp_otherLaneCounts = new int[num];
			_wp_giveWayListCounts = new int[num];
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < num; i++)
			{
				TrafficWaypoint trafficWaypoint = _allTrafficWaypoints[i];
				num2 += ((trafficWaypoint.Neighbors != null) ? trafficWaypoint.Neighbors.Length : 0);
				num3 += ((trafficWaypoint.Prevs != null) ? trafficWaypoint.Prevs.Length : 0);
				num4 += ((trafficWaypoint.OtherLanes != null) ? trafficWaypoint.OtherLanes.Length : 0);
				num5 += ((trafficWaypoint.GiveWayList != null) ? trafficWaypoint.GiveWayList.Length : 0);
			}
			_wp_neighborsFlat = new int[num2];
			_wp_anglesFlat = new int[num2];
			_wp_prevsFlat = new int[num3];
			_wp_otherLanesFlat = new int[num4];
			_wp_giveWayListFlat = new int[num5];
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			for (int j = 0; j < num; j++)
			{
				TrafficWaypoint trafficWaypoint2 = _allTrafficWaypoints[j];
				_wp_positions[j] = trafficWaypoint2.Position;
				_wp_maxSpeeds[j] = trafficWaypoint2.MaxSpeed;
				_wp_laneWidths[j] = trafficWaypoint2.LaneWidth;
				_wp_blinkTypes[j] = (int)trafficWaypoint2.BlinkType;
				_wp_leftDirections[j] = trafficWaypoint2.LeftDirection;
				int num10 = 0;
				if (trafficWaypoint2.AllowedVehicles != null)
				{
					for (int k = 0; k < trafficWaypoint2.AllowedVehicles.Length; k++)
					{
						num10 |= 1 << (int)trafficWaypoint2.AllowedVehicles[k];
					}
				}
				_wp_allowedVehiclesMask[j] = num10;
				byte b = 0;
				if (trafficWaypoint2.IsConnector)
				{
					b |= 1;
				}
				if (trafficWaypoint2.GiveWay)
				{
					b |= 2;
				}
				if (trafficWaypoint2.ComplexGiveWay)
				{
					b |= 4;
				}
				_wp_flags[j] = b;
				int num11 = ((trafficWaypoint2.Neighbors != null) ? trafficWaypoint2.Neighbors.Length : 0);
				_wp_neighborCounts[j] = num11;
				for (int l = 0; l < num11; l++)
				{
					_wp_neighborsFlat[num6] = trafficWaypoint2.Neighbors[l];
					_wp_anglesFlat[num6] = ((trafficWaypoint2.Angle != null && l < trafficWaypoint2.Angle.Length) ? trafficWaypoint2.Angle[l] : 0);
					num6++;
				}
				int num12 = ((trafficWaypoint2.Prevs != null) ? trafficWaypoint2.Prevs.Length : 0);
				_wp_prevCounts[j] = num12;
				for (int m = 0; m < num12; m++)
				{
					_wp_prevsFlat[num7++] = trafficWaypoint2.Prevs[m];
				}
				int num13 = ((trafficWaypoint2.OtherLanes != null) ? trafficWaypoint2.OtherLanes.Length : 0);
				_wp_otherLaneCounts[j] = num13;
				for (int n = 0; n < num13; n++)
				{
					_wp_otherLanesFlat[num8++] = trafficWaypoint2.OtherLanes[n];
				}
				int num14 = ((trafficWaypoint2.GiveWayList != null) ? trafficWaypoint2.GiveWayList.Length : 0);
				_wp_giveWayListCounts[j] = num14;
				for (int num15 = 0; num15 < num14; num15++)
				{
					_wp_giveWayListFlat[num9++] = trafficWaypoint2.GiveWayList[num15];
				}
			}
		}

		public void OnAfterDeserialize()
		{
			if (_wp_positions == null || _wp_positions.Length == 0)
			{
				_allTrafficWaypoints = Array.Empty<TrafficWaypoint>();
				return;
			}
			int num = _wp_positions.Length;
			_allTrafficWaypoints = new TrafficWaypoint[num];
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < num; i++)
			{
				int num6 = ((_wp_flags != null && i < _wp_flags.Length) ? _wp_flags[i] : 0);
				bool isConnector = (num6 & 1) != 0;
				bool giveWay = (num6 & 2) != 0;
				bool complexGiveWay = (num6 & 4) != 0;
				int num7 = ((_wp_allowedVehiclesMask != null && i < _wp_allowedVehiclesMask.Length) ? _wp_allowedVehiclesMask[i] : 0);
				List<VehicleTypes> list = new List<VehicleTypes>();
				for (int j = 0; j < 32; j++)
				{
					if (num7 == 0)
					{
						break;
					}
					if ((num7 & (1 << j)) != 0)
					{
						list.Add((VehicleTypes)j);
						num7 &= ~(1 << j);
					}
				}
				int num8 = ((_wp_neighborCounts != null && i < _wp_neighborCounts.Length) ? _wp_neighborCounts[i] : 0);
				int[] array = new int[num8];
				int[] array2 = new int[num8];
				for (int k = 0; k < num8; k++)
				{
					array[k] = _wp_neighborsFlat[num2];
					array2[k] = ((_wp_anglesFlat != null && num2 < _wp_anglesFlat.Length) ? _wp_anglesFlat[num2] : 0);
					num2++;
				}
				int num9 = ((_wp_prevCounts != null && i < _wp_prevCounts.Length) ? _wp_prevCounts[i] : 0);
				int[] array3 = new int[num9];
				for (int l = 0; l < num9; l++)
				{
					array3[l] = _wp_prevsFlat[num3++];
				}
				int num10 = ((_wp_otherLaneCounts != null && i < _wp_otherLaneCounts.Length) ? _wp_otherLaneCounts[i] : 0);
				int[] array4 = new int[num10];
				for (int m = 0; m < num10; m++)
				{
					array4[m] = _wp_otherLanesFlat[num4++];
				}
				int num11 = ((_wp_giveWayListCounts != null && i < _wp_giveWayListCounts.Length) ? _wp_giveWayListCounts[i] : 0);
				int[] array5 = new int[num11];
				for (int n = 0; n < num11; n++)
				{
					array5[n] = _wp_giveWayListFlat[num5++];
				}
				_allTrafficWaypoints[i] = new TrafficWaypoint(isConnector, i, _wp_positions[i], list, array, array3, array4, (_wp_maxSpeeds != null && i < _wp_maxSpeeds.Length) ? _wp_maxSpeeds[i] : 0, giveWay, complexGiveWay, zipperGiveWay: false, triggerEvent: false, (_wp_laneWidths != null && i < _wp_laneWidths.Length) ? _wp_laneWidths[i] : 0f, (_wp_leftDirections != null && i < _wp_leftDirections.Length) ? _wp_leftDirections[i] : Vector3.zero, null, array5, array2);
				_allTrafficWaypoints[i].BlinkType = ((_wp_blinkTypes != null && i < _wp_blinkTypes.Length) ? ((BlinkType)_wp_blinkTypes[i]) : BlinkType.None);
			}
			_wp_positions = null;
			_wp_maxSpeeds = null;
			_wp_laneWidths = null;
			_wp_blinkTypes = null;
			_wp_leftDirections = null;
			_wp_allowedVehiclesMask = null;
			_wp_flags = null;
			_wp_neighborsFlat = null;
			_wp_neighborCounts = null;
			_wp_prevsFlat = null;
			_wp_prevCounts = null;
			_wp_otherLanesFlat = null;
			_wp_otherLaneCounts = null;
			_wp_anglesFlat = null;
			_wp_giveWayListFlat = null;
			_wp_giveWayListCounts = null;
		}

		public void AssignZipperGiveWay()
		{
			for (int i = 0; i < _allTrafficWaypoints.Length; i++)
			{
				if (_allTrafficWaypoints[i].ZipperGiveWay)
				{
					int[] prevs = _allTrafficWaypoints[i].Prevs;
					for (int j = 0; j < prevs.Length; j++)
					{
						_allTrafficWaypoints[prevs[j]].SetGiveWayValue(newValue: true);
					}
				}
			}
		}

		public bool IsValid(out string error)
		{
			error = string.Empty;
			if (_allTrafficWaypoints == null)
			{
				error = TrafficSystemErrors.NullWaypointData;
				return false;
			}
			if (_allTrafficWaypoints.Length == 0)
			{
				error = TrafficSystemErrors.NoWaypointsFound;
				return false;
			}
			return true;
		}

		public TrafficWaypoint GetWaypointFromIndex(int waypointIndex)
		{
			if (IsWaypointIndexValid(waypointIndex))
			{
				return _allTrafficWaypoints[waypointIndex];
			}
			return null;
		}

		public List<NeighborStruct> GetNeighborsWithConditions(int waypointIndex, VehicleTypes vehicleType)
		{
			List<NeighborStruct> list = new List<NeighborStruct>();
			int[] neighbors = _allTrafficWaypoints[waypointIndex].Neighbors;
			int[] angle = _allTrafficWaypoints[waypointIndex].Angle;
			for (int i = 0; i < neighbors.Length; i++)
			{
				if (Enumerable.Contains(_allTrafficWaypoints[neighbors[i]].AllowedVehicles, vehicleType) && !_allTrafficWaypoints[neighbors[i]].TemporaryDisabled)
				{
					list.Add(new NeighborStruct(neighbors[i], angle[i]));
				}
			}
			return list;
		}

		public List<int> GetOtherLanesWithConditions(int waypointIndex, VehicleTypes vehicleType)
		{
			List<int> list = new List<int>();
			int[] otherLanes = _allTrafficWaypoints[waypointIndex].OtherLanes;
			for (int i = 0; i < otherLanes.Length; i++)
			{
				if (Enumerable.Contains(_allTrafficWaypoints[otherLanes[i]].AllowedVehicles, vehicleType) && !_allTrafficWaypoints[otherLanes[i]].TemporaryDisabled)
				{
					list.Add(otherLanes[i]);
				}
			}
			return list;
		}

		public int GetOtherLaneWaypointIndex(int currentWaypointIndex, VehicleTypes vehicleType, RoadSide side, Vector3 forwardVector)
		{
			if (_allTrafficWaypoints[currentWaypointIndex].HasOtherLanes)
			{
				List<int> otherLanesWithConditions = GetOtherLanesWithConditions(currentWaypointIndex, vehicleType);
				if (otherLanesWithConditions.Count > 0)
				{
					return GetSideWaypoint(otherLanesWithConditions, currentWaypointIndex, side, forwardVector);
				}
			}
			return -1;
		}

		public void SetEventData(int waypointIndex, string data)
		{
			if (IsWaypointIndexValid(waypointIndex))
			{
				if (data != null)
				{
					_allTrafficWaypoints[waypointIndex].TriggerEvent = true;
				}
				else
				{
					_allTrafficWaypoints[waypointIndex].TriggerEvent = false;
				}
				_allTrafficWaypoints[waypointIndex].EventData = data;
			}
		}

		public Quaternion GetNextOrientation(TrafficWaypoint waypoint)
		{
			if (!waypoint.HasNeighbors)
			{
				return Quaternion.identity;
			}
			return Quaternion.LookRotation(_allTrafficWaypoints[waypoint.Neighbors[0]].Position - waypoint.Position);
		}

		public Quaternion GetPrevOrientation(TrafficWaypoint waypoint)
		{
			if (!waypoint.HasPrevs)
			{
				return Quaternion.identity;
			}
			return Quaternion.LookRotation(waypoint.Position - _allTrafficWaypoints[waypoint.Prevs[0]].Position);
		}

		private int GetSideWaypoint(List<int> waypointIndexes, int currentWaypointIndex, RoadSide side, Vector3 forwardVector)
		{
			switch (side)
			{
			case RoadSide.Any:
				return waypointIndexes[UnityEngine.Random.Range(0, waypointIndexes.Count)];
			case RoadSide.Left:
			{
				for (int j = 0; j < waypointIndexes.Count; j++)
				{
					if (Vector3.SignedAngle(_allTrafficWaypoints[waypointIndexes[j]].Position - _allTrafficWaypoints[currentWaypointIndex].Position, forwardVector, Vector3.up) > 5f)
					{
						return waypointIndexes[j];
					}
				}
				break;
			}
			case RoadSide.Right:
			{
				for (int i = 0; i < waypointIndexes.Count; i++)
				{
					if (Vector3.SignedAngle(_allTrafficWaypoints[waypointIndexes[i]].Position - _allTrafficWaypoints[currentWaypointIndex].Position, _allTrafficWaypoints[_allTrafficWaypoints[currentWaypointIndex].Neighbors[0]].Position - _allTrafficWaypoints[currentWaypointIndex].Position, Vector3.up) < -5f)
					{
						return waypointIndexes[i];
					}
				}
				break;
			}
			}
			return -1;
		}

		public void AddNeighbor(int waypointIndex, int waypointIndexToAdd)
		{
			if (IsWaypointIndexValid(waypointIndex) && IsWaypointIndexValid(waypointIndexToAdd))
			{
				AllTrafficWaypoints[waypointIndex].AddNeighbor(waypointIndexToAdd);
			}
		}

		public void AddPrevs(int waypointIndex, int waypointIndexToAdd)
		{
			if (IsWaypointIndexValid(waypointIndex) && IsWaypointIndexValid(waypointIndexToAdd))
			{
				AllTrafficWaypoints[waypointIndex].AddPrev(waypointIndexToAdd);
			}
		}

		public void AddOtherLane(int waypointIndex, int waypointIndexToAdd)
		{
			if (IsWaypointIndexValid(waypointIndex) && IsWaypointIndexValid(waypointIndexToAdd))
			{
				AllTrafficWaypoints[waypointIndex].AddOtherLane(waypointIndexToAdd);
			}
		}

		private bool IsWaypointIndexValid(int waypointIndex)
		{
			if (waypointIndex < 0)
			{
				Debug.LogError($"Waypoint index {waypointIndex} should be >= 0");
				return false;
			}
			if (waypointIndex >= _allTrafficWaypoints.Length)
			{
				Debug.LogError($"Waypoint index {waypointIndex} should be < {_allTrafficWaypoints.Length}");
				return false;
			}
			if (_allTrafficWaypoints[waypointIndex] == null)
			{
				Debug.LogError($"Waypoint at {waypointIndex} is null, Verify the setup");
				return false;
			}
			return true;
		}

		private void TrafficLightChangedHandler(int waypointIndex, bool newValue)
		{
			_allTrafficWaypoints[waypointIndex].SetStopValue(newValue);
		}

		private void OnDestroy()
		{
			WaypointEvents.OnTrafficLightChanged -= TrafficLightChangedHandler;
		}
	}
}
