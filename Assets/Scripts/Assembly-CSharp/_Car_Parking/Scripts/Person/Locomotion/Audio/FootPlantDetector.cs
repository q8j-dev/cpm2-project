using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion.Audio
{
	public class FootPlantDetector
	{
		private readonly float _enterRatio;

		private readonly float _exitRatio;

		private readonly float _maxGroundedHeight;

		private readonly float _minInterval;

		private bool _initialised;

		private bool _planted;

		private float _lastPlantTime = float.NegativeInfinity;

		private Vector3 _previousPosition;

		public float LastSpeed { get; private set; }

		public float LastHeight { get; private set; }

		public bool IsPlanted => _planted;

		public FootPlantDetector(float enterRatio, float exitRatio, float maxGroundedHeight, float minInterval)
		{
			_enterRatio = enterRatio;
			_exitRatio = Mathf.Max(exitRatio, enterRatio + 0.05f);
			_maxGroundedHeight = maxGroundedHeight;
			_minInterval = minInterval;
		}

		public void Reset()
		{
			_initialised = false;
			_planted = true;
			_lastPlantTime = float.NegativeInfinity;
		}

		public bool Tick(Vector3 footPosition, Vector3 groundPoint, Vector3 groundNormal, float referenceSpeed, float deltaTime)
		{
			LastHeight = Vector3.Dot(footPosition - groundPoint, groundNormal);
			if (!_initialised)
			{
				_initialised = true;
				_planted = true;
				_previousPosition = footPosition;
				return false;
			}
			if (deltaTime <= 0f)
			{
				return false;
			}
			LastSpeed = (footPosition - _previousPosition).magnitude / deltaTime;
			_previousPosition = footPosition;
			if (_planted)
			{
				if (LastSpeed > referenceSpeed * _exitRatio)
				{
					_planted = false;
				}
				return false;
			}
			if (LastSpeed > referenceSpeed * _enterRatio || LastHeight > _maxGroundedHeight)
			{
				return false;
			}
			_planted = true;
			if (Time.time - _lastPlantTime < _minInterval)
			{
				return false;
			}
			_lastPlantTime = Time.time;
			return true;
		}
	}
}
