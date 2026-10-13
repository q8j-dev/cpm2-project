using UnityEngine;

namespace GPS
{
	public class WayPointsRenamer : MonoBehaviour
	{
		public int from;

		public int to = 9999;

		public bool isFinalPrefab;

		public void OnValidate()
		{
			int num = 0;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				for (int j = 0; j < child.childCount; j++)
				{
					child.GetChild(j).name = $"WayPoint {num}";
					LineRenderer component = child.GetChild(i).GetComponent<LineRenderer>();
					if ((bool)component)
					{
						Object.DestroyImmediate(component);
					}
					num++;
				}
			}
		}
	}
}
