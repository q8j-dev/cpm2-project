using UnityEngine;

namespace HighlightPlus
{
	[RequireComponent(typeof(HighlightEffect))]
	public class HighlightManager : MonoBehaviour
	{
		public LayerMask layerMask = -1;

		public Camera raycastCamera;

		public RayCastSource raycastSource;

		[Tooltip("Max Distance for target. 0 = infinity")]
		public float maxDistance;

		private HighlightEffect baseEffect;

		private HighlightEffect currentEffect;

		private Transform currentObject;

		private void OnEnable()
		{
			currentObject = null;
			currentEffect = null;
			if (baseEffect == null)
			{
				baseEffect = GetComponent<HighlightEffect>();
				if (baseEffect == null)
				{
					baseEffect = base.gameObject.AddComponent<HighlightEffect>();
				}
			}
			raycastCamera = GetComponent<Camera>();
			if (raycastCamera == null)
			{
				raycastCamera = GetCamera();
				if (raycastCamera == null)
				{
					Debug.LogError("Highlight Manager: no camera found!");
				}
			}
		}

		private void OnDisable()
		{
			SwitchesCollider(null);
		}

		private void Update()
		{
			if (raycastCamera == null)
			{
				return;
			}
			Ray ray = ((raycastSource != RayCastSource.MousePosition) ? new Ray(raycastCamera.transform.position, raycastCamera.transform.forward) : raycastCamera.ScreenPointToRay(Input.mousePosition));
			if (Physics.Raycast(ray, out var hitInfo, (maxDistance > 0f) ? maxDistance : raycastCamera.farClipPlane, layerMask))
			{
				if (hitInfo.collider != currentObject)
				{
					SwitchesCollider(hitInfo.collider.transform);
				}
			}
			else
			{
				SwitchesCollider(null);
			}
		}

		private void SwitchesCollider(Transform newObject)
		{
			if (currentEffect != null)
			{
				currentEffect.SetHighlighted(state: false);
				currentEffect = null;
			}
			currentObject = newObject;
			if (!(newObject == null))
			{
				HighlightTrigger component = newObject.GetComponent<HighlightTrigger>();
				if (!(component != null) || !component.enabled)
				{
					HighlightEffect component2 = newObject.GetComponent<HighlightEffect>();
					currentEffect = ((component2 != null) ? component2 : baseEffect);
					currentEffect.SetTarget(currentObject.transform);
					currentEffect.SetHighlighted(state: true);
				}
			}
		}

		public static Camera GetCamera()
		{
			Camera camera = Camera.main;
			if (camera == null)
			{
				camera = Object.FindObjectOfType<Camera>();
			}
			return camera;
		}
	}
}
