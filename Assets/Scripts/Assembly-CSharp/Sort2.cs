using System.Collections.Generic;
using SWS;
using UnityEngine;
using _Car_Parking.Scripts.Other.ScenesLogic;

public class Sort2 : MonoBehaviour
{
	public PathManager waypointRow1;

	public PathManager waypointRow2;

	public PathManager waypointRow3;

	public PathManager waypointRow4;

	public PathManager waypointRow5;

	public PathManager waypointRow6;

	public PathManager waypointRow7;

	public PathManager waypointRow8;

	public PathManager waypointMountain402_1;

	public PathManager waypointMountain402_2;

	public PathManager waypointMountainMain1;

	public PathManager waypointMountainMain2;

	public PathManager waypointMountainMain3;

	public PathManager waypointMountainMain4;

	public PathManager desertRoad1;

	public PathManager desertRoad2;

	public Transform[] desertRoad1T;

	public Transform[] desertRoad2T;

	public PathManager waypointRowCity7;

	public PathManager waypointRowCity6;

	public PathManager waypointRowCity5;

	public PathManager waypointRowCity8;

	public PathManager waypointRacing402_1;

	public PathManager waypointRacing402_2;

	public List<Vector3>[] AllRowLists;

	public List<Vector3> rowList1;

	public List<Vector3> rowList2;

	public List<Vector3> rowList3;

	public List<Vector3> rowList4;

	public Transform ParentRow1;

	public Transform ParentRow2;

	public Transform ParentRow3;

	public Transform ParentRow4;

	public List<Vector3> part1;

	public List<Vector3> part2;

	public List<Vector3> part3;

	public List<Vector3> part4;

	public List<Vector3> part5;

	public Transform[] row1TrafficArray;

	public Transform[] row2TrafficArray;

	public Transform[] row3TrafficArray;

	public Transform[] row4TrafficArray;

	public Transform[] row5TrafficArray;

	public Transform[] row6TrafficArray;

	public Transform[] row7TrafficArray;

	public Transform[] row8TrafficArray;

	public Vector3[][] rowTrafficArrays;

	public Vector3[] row1TrafficArrayV;

	public Vector3[] row2TrafficArrayV;

	public Vector3[] row3TrafficArrayV;

	public Vector3[] row4TrafficArrayV;

	public Vector3[] row5TrafficArrayV;

	public Vector3[] row6TrafficArrayV;

	public Vector3[] row7TrafficArrayV;

	public Vector3[] row8TrafficArrayV;

	public Vector3[] MountainRoad402_1V;

	public Vector3[] MountainRoad402_2V;

	public Vector3[] desertRoad1V;

	public Vector3[] desertRoad2V;

	public GameObject allParentDestroy;

	public Vector3[] row1City2;

	public Vector3[] row2City2;

	public Vector3[] row3City2;

	public Vector3[] row4City2;

	public Vector3[] row5City2;

	public Vector3[] row6City2;

	[SerializeField]
	private ScenePointsContainer ScenePointsContainer;

	public Vector3[] MountainRoadRoundBigV => ScenePointsContainer.MountainRoadRoundBigV.Collection;

	public Vector3[] MountainRoadRoundBigV2 => ScenePointsContainer.MountainRoadRoundBigV2.Collection;

	public Vector3[] MountainRoadRoundBigV3 => ScenePointsContainer.MountainRoadRoundBigV3.Collection;

	public Vector3[] MountainRoadRoundBigV4 => ScenePointsContainer.MountainRoadRoundBigV4.Collection;

	public Vector3[] RacingPlaceRoad4022 => waypointRacing402_2.GetPathPoints();

	public Vector3[] RacingPlaceRoad4021 => waypointRacing402_1.GetPathPoints();

	private void Awake()
	{
		rowTrafficArrays = new Vector3[18][];
		rowTrafficArrays[0] = row1TrafficArrayV;
		rowTrafficArrays[1] = row2TrafficArrayV;
		rowTrafficArrays[2] = row3TrafficArrayV;
		rowTrafficArrays[3] = row4TrafficArrayV;
		rowTrafficArrays[4] = row5TrafficArrayV;
		rowTrafficArrays[5] = row6TrafficArrayV;
		rowTrafficArrays[6] = row7TrafficArrayV;
		rowTrafficArrays[7] = row8TrafficArrayV;
		rowTrafficArrays[8] = MountainRoadRoundBigV;
		rowTrafficArrays[9] = MountainRoadRoundBigV2;
		rowTrafficArrays[10] = MountainRoadRoundBigV3;
		rowTrafficArrays[11] = MountainRoadRoundBigV4;
		rowTrafficArrays[12] = MountainRoad402_1V;
		rowTrafficArrays[13] = MountainRoad402_2V;
		if ((bool)allParentDestroy)
		{
			Object.Destroy(allParentDestroy);
		}
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	public void runOnEditor()
	{
		row1TrafficArray = waypointRow1.waypoints;
		row2TrafficArray = waypointRow2.waypoints;
		row3TrafficArray = waypointRow3.waypoints;
		row4TrafficArray = waypointRow4.waypoints;
		row5TrafficArray = waypointRow5.waypoints;
		row6TrafficArray = waypointRow6.waypoints;
		row7TrafficArray = waypointRow7.waypoints;
		row8TrafficArray = waypointRow8.waypoints;
		desertRoad1T = desertRoad1.waypoints;
		desertRoad2T = desertRoad2.waypoints;
		row1TrafficArrayV = PlayerDatas.FromTransformToVector3(row1TrafficArray);
		row2TrafficArrayV = PlayerDatas.FromTransformToVector3(row2TrafficArray);
		row3TrafficArrayV = PlayerDatas.FromTransformToVector3(row3TrafficArray);
		row4TrafficArrayV = PlayerDatas.FromTransformToVector3(row4TrafficArray);
		row5TrafficArrayV = PlayerDatas.FromTransformToVector3(row5TrafficArray);
		row6TrafficArrayV = PlayerDatas.FromTransformToVector3(row6TrafficArray);
		row7TrafficArrayV = PlayerDatas.FromTransformToVector3(row7TrafficArray);
		row8TrafficArrayV = PlayerDatas.FromTransformToVector3(row8TrafficArray);
		desertRoad1V = PlayerDatas.FromTransformToVector3(desertRoad1T);
		desertRoad2V = PlayerDatas.FromTransformToVector3(desertRoad2T);
		Debug.Log("runOnEditor done");
	}
}
