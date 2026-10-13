using System;
using UnityEngine;

[Serializable]
public class LevelDatas
{
	public Transform AngleMeasurePoint;

	public Transform[] ObjectsForEnable;

	public Transform[] ObjectsForDisable;

	public void LoadLevelDataForLevel(LevelScript levelScript)
	{
		levelScript.AngleMeasurePoint = AngleMeasurePoint;
		if (ObjectsForEnable != null)
		{
			EnableDisableObjects(enable: true, ObjectsForEnable);
		}
		if (ObjectsForDisable != null)
		{
			EnableDisableObjects(enable: false, ObjectsForDisable);
		}
	}

	public void EnableDisableObjects(bool enable, Transform[] objects)
	{
		foreach (Transform transform in objects)
		{
			if ((bool)transform)
			{
				transform.gameObject.SetActive(enable);
			}
		}
	}

	public void ForLevelParticularMethods()
	{
	}
}
