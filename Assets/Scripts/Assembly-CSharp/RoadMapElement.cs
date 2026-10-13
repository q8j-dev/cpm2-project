using System;
using UnityEngine;

[Serializable]
public class RoadMapElement
{
	public GameObject[] RoadElementsToEnable;

	public void SetObjectState(bool state)
	{
		GameObject[] roadElementsToEnable = RoadElementsToEnable;
		for (int i = 0; i < roadElementsToEnable.Length; i++)
		{
			roadElementsToEnable[i].SetActive(state);
		}
	}
}
