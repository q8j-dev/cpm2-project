using System;
using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

public class RecyclableScrollerDemo : MonoBehaviour, IRecyclableScrollRectDataSource
{
	[SerializeField]
	private RecyclableScrollRect _recyclableScrollRect = default;

	[SerializeField]
	private int _dataLength = default;

	private List<ContactInfo> _contactList = new List<ContactInfo>();

	private void Awake()
	{
		InitData();
		_recyclableScrollRect.DataSource = this;
	}

	private void InitData()
	{
		if (_contactList != null)
		{
			_contactList.Clear();
		}
		string[] array = new string[2] { "Male", "Female" };
		for (int i = 0; i < _dataLength; i++)
		{
			ContactInfo item = new ContactInfo
			{
				Name = i + "_Name",
				Gender = array[UnityEngine.Random.Range(0, 2)],
				id = "item : " + i
			};
			_contactList.Add(item);
		}
	}

	public int GetItemCount()
	{
		return _contactList.Count;
	}

	public void SetCell(ICell cell, int index)
	{
		(cell as DemoCell).ConfigureCell(_contactList[index], index);
	}

	public void CleanData()
	{
	}

	public void SpawnList<T>(List<T> dataList, GameObject prefab, Action<GameObject, T> buttonSet)
	{
		throw new NotImplementedException();
	}
}
