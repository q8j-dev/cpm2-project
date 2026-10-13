using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public class CellData
	{
		[SerializeField]
		private CellProperties _cellProperties;

		[SerializeField]
		private CellWaypointsData _trafficWaypointsData;

		[SerializeField]
		private CellWaypointsData _pedestrianWaypointsData;

		[SerializeField]
		private List<int> _intersectionsInCell;

		private bool _inView;

		public CellProperties CellProperties => _cellProperties;

		public CellWaypointsData TrafficWaypointsData => _trafficWaypointsData;

		public CellWaypointsData PedestrianWaypointsData => _pedestrianWaypointsData;

		public List<int> IntersectionsInCell => _intersectionsInCell;

		public bool InView
		{
			get
			{
				return _inView;
			}
			set
			{
				_inView = value;
			}
		}

		public CellData(CellProperties cellProperties, CellWaypointsData trafficWaypointsData, CellWaypointsData pedestrianWaypointsData, List<int> intersectionsInCell)
		{
			_cellProperties = cellProperties;
			_trafficWaypointsData = trafficWaypointsData;
			_pedestrianWaypointsData = pedestrianWaypointsData;
			_intersectionsInCell = intersectionsInCell;
		}
	}
}
