using UnityEngine;

namespace Gley.TrafficSystem
{
	public interface ITrafficParticipant
	{
		bool AlreadyCollidingWith(Collider[] allColliders);

		float GetCurrentSpeedMS();

		Vector3 GetHeading();
	}
}
