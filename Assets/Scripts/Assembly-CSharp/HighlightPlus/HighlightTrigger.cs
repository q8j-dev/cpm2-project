using System;
using System.Collections;
using UnityEngine;

namespace HighlightPlus
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(HighlightEffect))]
	public class HighlightTrigger : MonoBehaviour
	{
		[Tooltip("Used to trigger automatic highlighting including children objects.")]
		public TriggerMode triggerMode;

		public Camera raycastCamera;

		public RayCastSource raycastSource;

		public float maxDistance;

		public LayerMask volumeLayerMask;

		private const int MAX_RAYCAST_HITS = 100;

		[NonSerialized]
		public Collider[] colliders;

		private Collider currentCollider;

		private static RaycastHit[] hits;

		private void OnEnable()
		{
			Init();
		}

		private void Start()
		{
			if (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren)
			{
				if (raycastCamera == null)
				{
					raycastCamera = HighlightManager.GetCamera();
					if (raycastCamera == null)
					{
						Debug.LogError("Highlight Trigger on " + base.gameObject.name + ": no camera found!");
					}
				}
				if (colliders != null && colliders.Length != 0)
				{
					hits = new RaycastHit[100];
					StartCoroutine(DoRayCast());
				}
			}
			else if (GetComponent<Collider>() == null && GetComponent<MeshFilter>() != null)
			{
				base.gameObject.AddComponent<MeshCollider>();
			}
		}

		private IEnumerator DoRayCast()
		{
			while (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren)
			{
				if (raycastCamera != null)
				{
					Ray ray = ((raycastSource != RayCastSource.MousePosition) ? new Ray(raycastCamera.transform.position, raycastCamera.transform.forward) : raycastCamera.ScreenPointToRay(Input.mousePosition));
					int num = ((!(maxDistance > 0f)) ? Physics.RaycastNonAlloc(ray, hits) : Physics.RaycastNonAlloc(ray, hits, maxDistance));
					bool flag = false;
					for (int i = 0; i < num; i++)
					{
						Collider collider = hits[i].collider;
						for (int j = 0; j < colliders.Length; j++)
						{
							if (colliders[j] == collider)
							{
								flag = true;
								if (collider != currentCollider)
								{
									SwitchCollider(collider);
									i = num;
									break;
								}
							}
						}
					}
					if (!flag && currentCollider != null)
					{
						SwitchCollider(null);
					}
				}
				yield return null;
			}
		}

		private void SwitchCollider(Collider newCollider)
		{
			currentCollider = newCollider;
			if (currentCollider != null)
			{
				Highlight(state: true);
			}
			else
			{
				Highlight(state: false);
			}
		}

		private void OnMouseDown()
		{
			if (base.isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject)
			{
				Highlight(state: true);
			}
		}

		private void OnMouseEnter()
		{
			if (base.isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject)
			{
				Highlight(state: true);
			}
		}

		private void OnMouseExit()
		{
			if (base.isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject)
			{
				Highlight(state: false);
			}
		}

		private void Highlight(bool state)
		{
			HighlightEffect highlightEffect = base.transform.GetComponent<HighlightEffect>();
			if ((highlightEffect == null) & state)
			{
				highlightEffect = base.gameObject.AddComponent<HighlightEffect>();
			}
			if (highlightEffect != null)
			{
				highlightEffect.SetHighlighted(state);
			}
		}

		public void Init()
		{
			if (raycastCamera == null)
			{
				raycastCamera = HighlightManager.GetCamera();
			}
			if (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren)
			{
				colliders = GetComponentsInChildren<Collider>();
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (triggerMode == TriggerMode.Volume && ((int)volumeLayerMask & (1 << other.gameObject.layer)) != 0)
			{
				Highlight(state: true);
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (triggerMode == TriggerMode.Volume && ((int)volumeLayerMask & (1 << other.gameObject.layer)) != 0)
			{
				Highlight(state: false);
			}
		}
	}
}
