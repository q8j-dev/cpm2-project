using System;

namespace _Car_Parking.Scripts.Person.Locomotion.Input
{
	public sealed class PersonDigitalOnsetSettle
	{
		private const float AxisEpsilon = 0.01f;

		private float _idleTime = float.MaxValue;

		private float _heldTime;

		private bool _holding;

		private bool _committed;

		public bool Hold(bool hasMove, float moveX, float moveY, float deltaTime, float settleTime, float coldIdleTime)
		{
			if (!hasMove)
			{
				_idleTime += deltaTime;
				_holding = false;
				_heldTime = 0f;
				_committed = false;
				return false;
			}
			if (_committed)
			{
				return false;
			}
			bool flag = Math.Abs(moveX) > 0.01f && Math.Abs(moveY) > 0.01f;
			if (!_holding)
			{
				bool flag2 = _idleTime > coldIdleTime;
				if ((settleTime <= 0f || !flag2) | flag)
				{
					return Commit();
				}
				_holding = true;
				_heldTime = 0f;
				return true;
			}
			_heldTime += deltaTime;
			if (flag || _heldTime >= settleTime)
			{
				return Commit();
			}
			return true;
		}

		public void Reset()
		{
			_idleTime = float.MaxValue;
			_heldTime = 0f;
			_holding = false;
			_committed = false;
		}

		private bool Commit()
		{
			_committed = true;
			_holding = false;
			_heldTime = 0f;
			_idleTime = 0f;
			return false;
		}
	}
}
