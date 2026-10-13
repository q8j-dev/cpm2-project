using System.Linq;
using SWS;
using UnityEngine;

public class TargetControll : MonoBehaviour
{
	private Transform[][] _targets1;

	public Vector3[][] targets1V3;

	public GameObject[] d1;

	public GameObject toRoad;

	public Transform[] toRoadPos;

	public Vector3[] toRoadPosV3;

	public GameObject toRoad_high;

	public Transform[] toRoadPos_high;

	public Vector3[] toRoadPos_highV3;

	public PathManager[] pathWaypoints;

	private void Awake()
	{
		int num = 0;
		_targets1 = new Transform[d1.Length][];
		targets1V3 = new Vector3[d1.Length][];
		Transform[][] targets = _targets1;
		for (int i = 0; i < targets.Length; i++)
		{
			_ = targets[i];
			int num2 = d1[num].transform.Cast<Transform>().Count();
			int num3 = 0;
			_targets1[num] = new Transform[num2];
			targets1V3[num] = new Vector3[num2];
			foreach (Transform item in d1[num].transform)
			{
				_targets1[num][num3] = item.transform;
				targets1V3[num][num3] = item.transform.position;
				num3++;
			}
			num++;
		}
		if ((bool)toRoad)
		{
			int num4 = toRoad.transform.Cast<Transform>().Count();
			toRoadPos = new Transform[num4];
			toRoadPosV3 = new Vector3[num4];
			int num5 = 0;
			foreach (Transform item2 in toRoad.transform)
			{
				toRoadPos[num5] = item2.transform;
				toRoadPosV3[num5] = item2.transform.position;
				num5++;
			}
		}
		if (!toRoad_high)
		{
			return;
		}
		int num6 = toRoad_high.transform.Cast<Transform>().Count();
		toRoadPos_high = new Transform[num6];
		toRoadPos_highV3 = new Vector3[num6];
		int num7 = 0;
		foreach (Transform item3 in toRoad_high.transform)
		{
			toRoadPos_high[num7] = item3.transform;
			toRoadPos_highV3[num7] = item3.transform.position;
			num7++;
		}
	}
}
