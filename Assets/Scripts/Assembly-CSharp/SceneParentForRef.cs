using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;

public class SceneParentForRef : MonoBehaviour
{
	public V.placeToSpawn environmet;

	public List<GameObject> listGO;

	public List<GameObject> listGO2;

	public List<GameObject> listGO3;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
	}

	private void Awake()
	{
		foreach (GameObject item in listGO)
		{
			if ((bool)item)
			{
				item.SetActive(value: false);
			}
		}
		foreach (GameObject item2 in listGO2)
		{
			if ((bool)item2)
			{
				item2.SetActive(value: false);
			}
		}
		foreach (GameObject item3 in listGO3)
		{
			if ((bool)item3)
			{
				item3.SetActive(value: false);
			}
		}
		SetReferenceForRacing();
	}

	private void SetReferenceForRacing()
	{
		if (!FlagsStorage.IsFreeDriveMode || (listGO.Count == 0 && listGO2.Count == 0 && listGO3.Count == 0) || !_freeDriveProvider.DragRacingControlMultiplayer)
		{
			return;
		}
		foreach (DragRacingMultiplayerTree tree in _freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.TreeList)
		{
			if (!(tree == null) && tree.PlaceToSpawn == environmet && tree.propsToEnable != null && tree.propsToEnable.Count >= 3)
			{
				tree.propsToEnable[0].props.AddRange(listGO);
				tree.propsToEnable[1].props.AddRange(listGO2);
				tree.propsToEnable[2].props.AddRange(listGO3);
				if ((bool)tree.otherDistanceTree && tree.otherDistanceTree.propsToEnable != null && tree.otherDistanceTree.propsToEnable.Count >= 3)
				{
					tree.otherDistanceTree.propsToEnable[0].props.AddRange(listGO);
					tree.otherDistanceTree.propsToEnable[1].props.AddRange(listGO2);
					tree.otherDistanceTree.propsToEnable[2].props.AddRange(listGO3);
				}
			}
		}
	}
}
