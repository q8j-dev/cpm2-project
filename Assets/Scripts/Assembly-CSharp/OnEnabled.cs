using System;
using System.Collections.Generic;
using UnityEngine;

public class OnEnabled : MonoBehaviour
{
	public List<GameObject> objs;

	private void Start()
	{
	}

	private void OnEnable()
	{
		foreach (GameObject obj in objs)
		{
			obj.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		foreach (GameObject obj in objs)
		{
			try
			{
				obj.SetActive(value: false);
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
	}
}
