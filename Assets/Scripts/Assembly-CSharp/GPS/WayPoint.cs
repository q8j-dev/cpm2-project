using System.Collections.Generic;
using UnityEngine;

namespace GPS
{
	public class WayPoint : MonoBehaviour
	{
		public WayPointNode node;

		public List<WayPoint> next;

		private List<Vector3> pos = new List<Vector3>();

		private bool drawGizmos = true;

		private bool selected;

		public void Init(params GameObject[] prefabs)
		{
			OnValidate();
		}

		public void OnValidate()
		{
			drawGizmos = true;
		}

		private void OnDrawGizmos()
		{
			if (selected || node == null || next == null)
			{
				return;
			}
			foreach (WayPoint item in next)
			{
				if ((bool)item)
				{
					Vector3 direction = item.transform.position - base.transform.position;
					DrawArrow.ForGizmo(base.transform.position, direction, Color.red, doubled: false, 1f, 45f);
				}
			}
			drawGizmos = false;
		}

		private void OnDrawGizmosSelected()
		{
			selected = true;
			if (node == null || next == null)
			{
				return;
			}
			foreach (WayPoint item in next)
			{
				if ((bool)item)
				{
					Vector3 direction = item.transform.position - base.transform.position;
					DrawArrow.ForGizmo(base.transform.position, direction, Color.green, doubled: false, 1f, 45f);
				}
			}
			selected = false;
			drawGizmos = false;
		}
	}
}
