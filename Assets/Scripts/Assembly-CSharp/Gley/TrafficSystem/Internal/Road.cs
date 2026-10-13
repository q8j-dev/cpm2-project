using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class Road : RoadBase
	{
		public List<Lane> lanes;

		public int otherLaneLinkDistance;

		public void SetDefaults(int nrOfLanes, float laneWidth, float waypointDistance, int otherLaneLinkDistance)
		{
			base.nrOfLanes = nrOfLanes;
			base.laneWidth = laneWidth;
			base.waypointDistance = waypointDistance;
			this.otherLaneLinkDistance = otherLaneLinkDistance;
		}

		public void SetRoadProperties(int globalMaxSpeed, int nrOfCars, bool leftSideTraffic)
		{
			draw = true;
			lanes = new List<Lane>();
			for (int i = 0; i < nrOfLanes; i++)
			{
				if (!leftSideTraffic)
				{
					lanes.Add(new Lane(nrOfCars, i % 2 == 0, globalMaxSpeed));
				}
				else
				{
					lanes.Add(new Lane(nrOfCars, i % 2 != 0, globalMaxSpeed));
				}
			}
		}

		public override bool VerifyAssignments()
		{
			if (path == null || path.NumPoints < 4)
			{
				Debug.LogWarning(base.name + " is corrupted and will be deleted");
				return false;
			}
			if (!justCreated)
			{
				if (lanes == null)
				{
					lanes = new List<Lane>();
				}
				if (waypointDistance <= 0f)
				{
					waypointDistance = 1f;
				}
				for (int i = 0; i < lanes.Count; i++)
				{
					if (lanes[i].laneEdges.inConnector == null || lanes[i].laneEdges.outConnector == null)
					{
						Debug.LogError(base.name + " is corrupted. Go to Edit Road Window and press Generate Waypoints.", this);
						return false;
					}
				}
			}
			return true;
		}

		public void UpdateLaneNumber(int maxSpeed, int nrOfCars)
		{
			if (lanes.Count > nrOfLanes)
			{
				lanes.RemoveRange(nrOfLanes, lanes.Count - nrOfLanes);
			}
			if (lanes.Count < nrOfLanes)
			{
				for (int i = lanes.Count; i < nrOfLanes; i++)
				{
					lanes.Add(new Lane(nrOfCars, i % 2 == 0, maxSpeed));
				}
			}
		}

		public void AddLaneConnector(WaypointSettingsBase inConnector, WaypointSettingsBase outConnector, int index)
		{
			if (inConnector != null)
			{
				inConnector.name = inConnector.transform.parent.parent.parent.name + "-" + inConnector.transform.parent.name + "-In";
			}
			if (outConnector != null)
			{
				outConnector.name = outConnector.transform.parent.parent.parent.name + "-" + outConnector.transform.parent.name + "-Out";
			}
			lanes[index].laneEdges = new LaneConnectors(inConnector, outConnector);
		}

		public void SwitchDirection(int laneNumber)
		{
			AddLaneConnector(lanes[laneNumber].laneEdges.outConnector, lanes[laneNumber].laneEdges.inConnector, laneNumber);
		}

		public List<int> GetAllowedCars(int laneNumber)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < lanes[laneNumber].allowedCars.Length; i++)
			{
				if (lanes[laneNumber].allowedCars[i])
				{
					list.Add(i);
				}
			}
			return list;
		}

		public int GetNrOfLanes()
		{
			return base.transform.Find("Lanes").childCount;
		}
	}
}
