using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Rally.Data;

[CreateAssetMenu(fileName = "RacingMapTrajectorySO", menuName = "Configs/RacingMapTrajectorySO")]
public class RallyMapTrajectorySO : ScriptableObject
{
	public List<RallySequenceData> RallySequenceDataList = new List<RallySequenceData>();

	public void AddMapElement(RallySequenceData _RallySequenceData)
	{
		RallySequenceDataList.Add(_RallySequenceData);
		SortElements();
	}

	public void SortElements()
	{
		for (int i = 0; i < RallySequenceDataList.Count; i++)
		{
			RallySequenceDataList[i].Index = i;
		}
	}
}
