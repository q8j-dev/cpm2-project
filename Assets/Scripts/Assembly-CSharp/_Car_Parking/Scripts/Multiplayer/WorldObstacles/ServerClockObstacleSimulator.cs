using System.Collections.Generic;
using CPM.Obstacles;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.WorldObstacles
{
	public sealed class ServerClockObstacleSimulator : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Drive the obstacles under this object. Leave empty to drive the ones under the object the component sits on.")]
		private Transform Root;

		private static readonly List<Obstacle> Driven = new List<Obstacle>();

		private static ObstacleDriver _driver;

		private Obstacle[] _mine;

		private void OnEnable()
		{
			Transform transform = (Root ? Root : base.transform);
			_mine = transform.GetComponentsInChildren<Obstacle>();
			if (_mine.Length == 0)
			{
				Debug.LogWarning("[ServerClockObstacleSimulator] " + base.name + ": no Obstacle to drive.", this);
				return;
			}
			Driven.AddRange(_mine);
			if (_driver == null)
			{
				_driver = new ObstacleDriver(Driven, ServerObstacleClock.Elapsed);
			}
		}

		private void OnDisable()
		{
			if (_mine != null)
			{
				for (int i = 0; i < _mine.Length; i++)
				{
					Driven.Remove(_mine[i]);
				}
				_mine = null;
				if (Driven.Count <= 0)
				{
					_driver?.Dispose();
					_driver = null;
				}
			}
		}
	}
}
