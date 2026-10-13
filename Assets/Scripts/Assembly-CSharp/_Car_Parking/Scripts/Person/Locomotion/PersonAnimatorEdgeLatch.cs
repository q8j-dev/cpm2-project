namespace _Car_Parking.Scripts.Person.Locomotion
{
	public sealed class PersonAnimatorEdgeLatch
	{
		private bool _pending;

		private float _raisedAtFixedTime;

		public bool Resolve(bool edge, float fixedTime, bool evaluatesInPhysicsStep)
		{
			if (edge)
			{
				_pending = evaluatesInPhysicsStep;
				_raisedAtFixedTime = fixedTime;
				return true;
			}
			if (_pending && fixedTime <= _raisedAtFixedTime)
			{
				return true;
			}
			_pending = false;
			return false;
		}
	}
}
