using UnityEngine;

namespace Arikan
{
	public class MiniMapBounds : MonoBehaviour
	{
		public Transform topRight;

		public Transform bottomLeft;

		public Bounds GetWorldRect()
		{
			Vector3 position = bottomLeft.position;
			Vector3 vector = topRight.position - position;
			return new Bounds(position + vector / 2f, vector);
		}

		private void DetectChilds()
		{
			if (topRight == null)
			{
				if (base.transform.childCount > 0)
				{
					topRight = base.transform.GetChild(0);
				}
				else
				{
					Transform transform = new GameObject("TopRight").transform;
					transform.SetParent(base.transform);
					base.transform.localPosition = new Vector3(-1f, 0f, -1f);
					transform.SetSiblingIndex(0);
					topRight = transform;
				}
			}
			if (bottomLeft == null)
			{
				if (base.transform.childCount > 1)
				{
					bottomLeft = base.transform.GetChild(1);
					return;
				}
				Transform transform2 = new GameObject("BottomLeft").transform;
				transform2.SetParent(base.transform);
				base.transform.localPosition = new Vector3(1f, 0f, 1f);
				transform2.SetSiblingIndex(1);
				bottomLeft = transform2;
			}
		}

		private void OnValidate()
		{
			DetectChilds();
		}

		private void OnEnable()
		{
			DetectChilds();
		}

		private void OnDrawGizmosSelected()
		{
			if (topRight != null && bottomLeft != null)
			{
				Bounds worldRect = GetWorldRect();
				Gizmos.DrawWireCube(worldRect.center, worldRect.size);
			}
		}
	}
}
