using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class TempStop : VehicleBehaviour
	{
		private const float _maxStopTime = 10f;

		private float _stopTime;

		private float _currentTime;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			_currentTime = 0f;
			_stopTime = UnityEngine.Random.Range(5f, 10f);
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			PerformForwardMovement(ref behaviourResult, 0f, 0f, TrafficSystemConstants.DEFAULT_POSITION, 1f, 1f, 0f);
			if (base.VehicleComponent.GetCurrentSpeedMS() < 1f)
			{
				_currentTime += Time.deltaTime;
			}
			if (_currentTime > _stopTime)
			{
				Stop();
			}
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
