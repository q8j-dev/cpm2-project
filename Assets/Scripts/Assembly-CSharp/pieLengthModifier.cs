using System;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;

public class pieLengthModifier : MonoBehaviour
{
	private static Dictionary<string, float> LengthDictionary;

	static pieLengthModifier()
	{
		LengthDictionary = new Dictionary<string, float>();
		LengthDictionary["Category 1"] = 50f;
		LengthDictionary["Category 2"] = 100f;
		LengthDictionary["Category 3"] = 150f;
	}

	private void Start()
	{
	}

	private void Update()
	{
		PieInfo component = GetComponent<PieInfo>();
		try
		{
			component.pieObject.ItemLabel.Direction.Length = LengthDictionary[component.Category];
		}
		catch (Exception)
		{
		}
	}
}
