using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RainSoundZoneColliderSync : MonoBehaviour
{
	private void Awake()
	{
		Transform parent = base.transform.parent;
		if (!(parent == null) && parent.TryGetComponent<BoxCollider>(out var component) && TryGetComponent<BoxCollider>(out var component2))
		{
			component2.size = component.size;
			component2.center = component.center;
		}
	}
}
