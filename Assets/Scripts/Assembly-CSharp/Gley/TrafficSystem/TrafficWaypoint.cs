using System;
using System.Collections.Generic;
using System.Linq;
using Gley.TrafficSystem.Internal;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem
{
	[Serializable]
	public class TrafficWaypoint : Waypoint
	{
		private List<IIntersection> _associatedIntersections;

		[SerializeField]
		private VehicleTypes[] _allowedVehicles;

		[SerializeField]
		private int[] _angle;

		[SerializeField]
		private int[] _giveWayList;

		[SerializeField]
		private int[] _otherLanes;

		[SerializeField]
		private Vector3 _leftDirection;

		[SerializeField]
		private BlinkType _blinkType;

		private string _eventData;

		[SerializeField]
		private float _laneWidth;

		[SerializeField]
		private int _maxSpeed;

		[SerializeField]
		private bool _giveWay;

		[SerializeField]
		private bool _complexGiveWay;

		private bool _zipperGiveWay;

		private bool _intersectionGiveWay;

		private bool _triggerEvent;

		private bool _enter;

		private bool _exit;

		private bool _stop;

		public int[] GiveWayList => _giveWayList;

		public VehicleTypes[] AllowedVehicles => _allowedVehicles;

		public List<IIntersection> AssociatedIntersections => _associatedIntersections;

		public int[] OtherLanes => _otherLanes;

		public int[] Angle => _angle;

		public Vector3 LeftDirection => _leftDirection;

		public float LaneWidth => _laneWidth;

		public int MaxSpeed => _maxSpeed;

		public bool ComplexGiveWay => _complexGiveWay;

		public bool ZipperGiveWay => _zipperGiveWay;

		public bool IntersectionGiveWay => _intersectionGiveWay;

		public bool Enter => _enter;

		public bool Exit => _exit;

		public bool Stop => _stop;

		public bool GiveWay => _giveWay;

		public bool HasNeighbors => base.Neighbors.Length != 0;

		public bool HasPrevs => base.Prevs.Length != 0;

		public bool HasOtherLanes => OtherLanes.Length != 0;

		public BlinkType BlinkType
		{
			get
			{
				return _blinkType;
			}
			set
			{
				_blinkType = value;
			}
		}

		public bool TriggerEvent
		{
			get
			{
				return _triggerEvent;
			}
			set
			{
				_triggerEvent = value;
			}
		}

		public string EventData
		{
			get
			{
				return _eventData ?? string.Empty;
			}
			set
			{
				_eventData = value;
			}
		}

		public TrafficWaypoint(bool isConnector, int listIndex, Vector3 position, List<VehicleTypes> allowedVehicles, int[] neighbors, int[] prev, int[] otherLanes, int maxSpeed, bool giveWay, bool complexGiveWay, bool zipperGiveWay, bool triggerEvent, float laneWidth, Vector3 left, string eventData, int[] giveWayList, int[] angle)
			: base(isConnector, listIndex, position, neighbors, prev)
		{
			_maxSpeed = maxSpeed;
			_giveWay = giveWay;
			_complexGiveWay = complexGiveWay;
			_zipperGiveWay = zipperGiveWay;
			_giveWayList = giveWayList;
			_laneWidth = laneWidth;
			_leftDirection = left;
			_otherLanes = otherLanes;
			_enter = false;
			_exit = false;
			_stop = false;
			_triggerEvent = triggerEvent;
			_eventData = eventData;
			_allowedVehicles = allowedVehicles.ToArray();
			_angle = angle;
			_blinkType = BlinkType.None;
		}

		public void SetIntersection(IIntersection intersection, bool giveWay, bool stop, bool enter, bool exit, bool intersectionGiveWay)
		{
			if (_associatedIntersections == null)
			{
				_associatedIntersections = new List<IIntersection>();
			}
			if (!_associatedIntersections.Contains(intersection))
			{
				_associatedIntersections.Add(intersection);
			}
			_stop = stop;
			_giveWay = giveWay;
			_exit = exit;
			_enter = enter;
			_intersectionGiveWay = intersectionGiveWay;
			WaypointEvents.TriggerGiveWayStateChangedEvent(base.ListIndex, GetGiveWayState());
		}

		public void SetStopValue(bool newValue)
		{
			if (_stop != newValue)
			{
				_stop = newValue;
				WaypointEvents.TriggerStopStateChangedEvent(base.ListIndex, newValue);
			}
		}

		public void SetGiveWayValue(bool newValue)
		{
			if (_giveWay != newValue)
			{
				_giveWay = newValue;
				WaypointEvents.TriggerGiveWayStateChangedEvent(base.ListIndex, GetGiveWayState());
			}
		}

		public void ComputeBlinkerData(TrafficWaypointsData trafficWaypointsData)
		{
			if (base.Neighbors.Length == 0)
			{
				_blinkType = BlinkType.StartHazard;
			}
			else if (base.Neighbors.Length > 1)
			{
				_blinkType = BlinkType.Start;
				for (int i = 0; i < base.Neighbors.Length; i++)
				{
					Vector3 forwardVector = GetForwardVector(trafficWaypointsData);
					Vector3 normalized = (trafficWaypointsData.AllTrafficWaypoints[base.Neighbors[i]].Position - base.Position).normalized;
					float y = Vector3.Cross(forwardVector, normalized).y;
					if (y > 0.1f)
					{
						trafficWaypointsData.AllTrafficWaypoints[base.Neighbors[i]].BlinkType = BlinkType.Right;
					}
					else if (y < -0.1f)
					{
						trafficWaypointsData.AllTrafficWaypoints[base.Neighbors[i]].BlinkType = BlinkType.Left;
					}
				}
			}
			else if (base.Neighbors.Length == 1 && _blinkType == BlinkType.None)
			{
				Vector3 forwardVector2 = GetForwardVector(trafficWaypointsData);
				Vector3 normalized2 = (trafficWaypointsData.AllTrafficWaypoints[base.Neighbors[0]].Position - base.Position).normalized;
				float y2 = Vector3.Cross(forwardVector2, normalized2).y;
				if (y2 > 0.1f)
				{
					_blinkType = BlinkType.Right;
				}
				else if (y2 < -0.1f)
				{
					_blinkType = BlinkType.Left;
				}
				else
				{
					_blinkType = BlinkType.None;
				}
			}
		}

		public Vector3 GetForwardVector(TrafficWaypointsData trafficWaypointsData)
		{
			if (base.Prevs.Length == 0)
			{
				Debug.LogWarning("Waypoint has no previous waypoints, defaulting to (0,0,1)");
				return Vector3.forward;
			}
			Vector3 zero = Vector3.zero;
			int[] prevs = base.Prevs;
			foreach (int num in prevs)
			{
				zero += (base.Position - trafficWaypointsData.AllTrafficWaypoints[num].Position).normalized;
			}
			return zero.normalized;
		}

		public GiveWayType GetGiveWayState()
		{
			if (IntersectionGiveWay && Enter)
			{
				return GiveWayType.Intersection;
			}
			if (ComplexGiveWay)
			{
				return GiveWayType.Complex;
			}
			if (ZipperGiveWay)
			{
				return GiveWayType.Zipper;
			}
			if (GiveWay)
			{
				return GiveWayType.Standard;
			}
			return GiveWayType.None;
		}

		public void AddOtherLane(int waypointIndex)
		{
			List<int> list = _otherLanes.ToList();
			if (!list.Contains(waypointIndex))
			{
				list.Add(waypointIndex);
				_otherLanes = list.ToArray();
			}
		}
	}
}
