using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class DebugBounds : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(component.bounds.center, component.bounds.size);
			}
		}
	}
}
