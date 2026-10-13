using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PolyAndCode.UI;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MenuUI.VisualTuning;

public class VisualTuningElementSpawner : MonoBehaviour
{
	private IPrefabFactory _prefabFactory;

	private List<VisualTuningCellData> _visualTuningCellDatas = new List<VisualTuningCellData>();

	private VisualTuningTypes _currentTuningType;

	private VisualItemElement[] _cahchedVisualTuningElements;

	[SerializeField]
	private VisualTuningWindow VisualTuningWindow;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory)
	{
		_prefabFactory = prefabFactory;
	}

	public int GetItemCount()
	{
		return _visualTuningCellDatas.Count();
	}

	public void SetCell(ICell cell, int index)
	{
		(cell as VisualItemElement).Init(_visualTuningCellDatas[index]);
	}

	public void SpawnList<T>(IList<T> dataList, VisualItemElement prefab, Action<GameObject, object> buttonSet, bool cleanPanel = false)
	{
		if (cleanPanel)
		{
			DestroyElements();
		}
		_visualTuningCellDatas = new List<VisualTuningCellData>();
		foreach (T data in dataList)
		{
			VisualTuningCellData visualTuningCellData = new VisualTuningCellData();
			visualTuningCellData.data = data;
			visualTuningCellData.buttonSetAction = buttonSet;
			_prefabFactory.Instantiate(prefab, VisualTuningWindow.content).Init(visualTuningCellData);
		}
		InitScrollRectOcclusion();
	}

	private void DestroyElements()
	{
		foreach (Transform item in VisualTuningWindow.content)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	private async UniTaskVoid InitScrollRectOcclusion()
	{
		await UniTask.Yield();
		VisualTuningWindow.UiScrollRectOcclusiion.Init();
	}

	public void ResetVisualTuningElementsCollection()
	{
		_cahchedVisualTuningElements = null;
	}

	public VisualItemElement[] GetCachedVisualTuningElements()
	{
		if (_cahchedVisualTuningElements == null)
		{
			_cahchedVisualTuningElements = VisualTuningWindow.content.GetComponentsInChildren<VisualItemElement>(includeInactive: true);
		}
		return _cahchedVisualTuningElements;
	}
}
