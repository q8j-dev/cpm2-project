using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class LocalObstacleSimulator : MonoBehaviour
	{
		[Tooltip("Drive only the obstacles under this object. Leave empty to drive every obstacle in the scene.")]
		[SerializeField]
		private Transform Root = default;

		[Tooltip("Seconds to hold every obstacle at its start pose before the clock runs, so you can watch them set off together. Zero starts immediately.")]
		[SerializeField]
		private float StartDelaySeconds = default;

		private ObstacleDriver _driver;

		private double _startTime;

		private void OnEnable()
		{
			Obstacle[] array = (Root ? Root.GetComponentsInChildren<Obstacle>(includeInactive: true) : Object.FindObjectsByType<Obstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None));
			if (array.Length == 0)
			{
				Debug.LogWarning("[LocalObstacleSimulator] " + base.name + ": no Obstacle to drive.", this);
				return;
			}
			_startTime = Time.timeAsDouble;
			_driver = new ObstacleDriver(array, Elapsed);
		}

		private void OnDisable()
		{
			_driver?.Dispose();
			_driver = null;
		}

		[ContextMenu("Restart from start")]
		private void Restart()
		{
			_startTime = Time.timeAsDouble;
		}

		private double Elapsed()
		{
			double num = Time.timeAsDouble - _startTime;
			if (!(num > (double)StartDelaySeconds))
			{
				return 0.0;
			}
			return num - (double)StartDelaySeconds;
		}
	}
}
