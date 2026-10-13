using Cinemachine;
using UnityEngine;
using _Car_Parking.Scripts.Racing_Scripts;

public class CinemachineDataParser : MonoBehaviour
{
	public RaceModeDataSO RaceModeDataSo;

	public CinemachinePath CinemachinePath;

	[ContextMenu("Parse")]
	public void Parse()
	{
		CineMachinePathData cineMachinePathData = new CineMachinePathData();
		cineMachinePathData.Resolution = CinemachinePath.m_Resolution;
		cineMachinePathData.Loopped = CinemachinePath.m_Looped;
		cineMachinePathData.Waypoints = CinemachinePath.m_Waypoints;
		RaceModeDataSo.RaceModeData.CineMachinePathData = cineMachinePathData;
	}
}
