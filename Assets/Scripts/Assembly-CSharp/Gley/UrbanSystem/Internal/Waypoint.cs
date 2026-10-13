using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public class Waypoint
	{
		[SerializeField]
		private int[] _neighbors;

		[SerializeField]
		private int[] _prev;

		[SerializeField]
		private Vector3 _position;

		[SerializeField]
		private bool _isConnector;

		[SerializeField]
		private int _listIndex;

		private bool _temporaryDisabled;

		public int[] Neighbors => _neighbors;

		public int[] Prevs => _prev;

		public bool IsConnector => _isConnector;

		public int ListIndex => _listIndex;

		public bool TemporaryDisabled
		{
			get
			{
				return _temporaryDisabled;
			}
			set
			{
				_temporaryDisabled = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public Waypoint(bool isConnector, int listIndex, Vector3 position, int[] neighbors, int[] prev)
		{
			_isConnector = isConnector;
			_listIndex = listIndex;
			_position = position;
			_neighbors = neighbors;
			_prev = prev;
			_temporaryDisabled = false;
		}

		public void AddNeighbor(int waypointIndex)
		{
			List<int> list = _neighbors.ToList();
			if (!list.Contains(waypointIndex))
			{
				list.Add(waypointIndex);
				_neighbors = list.ToArray();
			}
		}

		public void AddPrev(int waypointIndex)
		{
			List<int> list = _prev.ToList();
			if (!list.Contains(waypointIndex))
			{
				list.Add(waypointIndex);
				_prev = list.ToArray();
			}
		}
	}
}
