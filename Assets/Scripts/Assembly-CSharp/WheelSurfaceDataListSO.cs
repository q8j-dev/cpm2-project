using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelSurfaceDataConfig", menuName = "Scriptable/WheelSurfaceData", order = 10)]
public class WheelSurfaceDataListSO : ScriptableObject
{
	public List<SurfaceDirtColor> surfaceDirtColors;

	public List<WheelSurfaceData> wheelSurfaceDataList;
}
