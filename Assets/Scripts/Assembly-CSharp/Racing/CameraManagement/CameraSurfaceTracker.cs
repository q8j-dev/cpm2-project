using UnityEngine;

namespace Racing.CameraManagement
{
	public class CameraSurfaceTracker
	{
		private const float MaxRayDistance = 300f;

		private readonly LayerMask _layers;

		public CameraSurfaceTracker(LayerMask layers)
		{
			_layers = layers;
		}

		public bool TryGetSurfaceHit(Vector3 origin, Vector3 direction, float maxDistance, out RaycastHit hit)
		{
			hit = default;
			if (direction.sqrMagnitude < Mathf.Epsilon)
			{
				return false;
			}
			return Physics.Raycast(origin, direction.normalized, out hit, maxDistance, _layers, QueryTriggerInteraction.Ignore);
		}

		public bool TryGetSurfacePoint(Vector3 cameraPosition, Collider target, out Vector3 surfacePoint)
		{
			surfacePoint = Vector3.zero;
			if (!target)
			{
				return false;
			}
			if (!TryGetSurfaceHit(cameraPosition, target.bounds.center - cameraPosition, 300f, out var hit))
			{
				return false;
			}
			surfacePoint = hit.point;
			return true;
		}
	}
}
