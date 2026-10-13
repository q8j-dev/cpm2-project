using System;
using System.Collections.Generic;
using DataEntities.Vinyl;
using UnityEngine;

[Serializable]
public class AllVinylPreset : ScriptableObject
{
	public List<OneVinylsPreset> allPresetList;

	public int CarID;

	[ContextMenu("Fix sorting layers")]
	private void FixSortingLayers()
	{
		for (int i = 0; i < allPresetList.Count; i++)
		{
			List<VinylData> allVynils = allPresetList[i].vynilOneCarList.allVynils;
			for (int j = 0; j < allVynils.Count; j++)
			{
				Vector3 position = allVynils[j].position;
				allVynils[j].position = new Vector3(position.x, position.y, j);
			}
			List<VinylData> list = allPresetList[i].windowVynilOneCarList?.allVynils;
			if (list != null)
			{
				for (int k = 0; k < list.Count; k++)
				{
					Vector3 position2 = list[k].position;
					list[k].position = new Vector3(position2.x, position2.y, k);
				}
			}
		}
	}

	public List<OneVinylsPreset> GetAllPreset()
	{
		return null;
	}
}
