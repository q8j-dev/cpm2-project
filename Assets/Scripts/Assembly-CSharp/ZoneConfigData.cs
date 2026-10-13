using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ZoneConfigData
{
	public Vector3 ZonePosition;

	public Vector3 ZoneScale;

	public bool IsRandomPosition;

	public int Index;

	public float TotalPassTime;

	public float TotalPenaltyTime;

	public int ActiveCollectiblesCount;

	public int TargetCount;

	public List<Vector3> Zones;

	public List<Quaternion> Rotation;
}
