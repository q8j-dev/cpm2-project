using UnityEngine;

namespace CPM.Obstacles
{
	public interface IRagdollImpact
	{
		bool TryGetRagdollImpact(Vector3 contactPoint, out Vector3 impulse);
	}
}
