using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnaliticsGoogleOwn
{
	public static void OnAllTaskDone()
	{
		if (Application.isEditor)
		{
			Debug.Log("done all battle pass tasks");
		}
		int day = DateTime.Today.Day;
		int month = DateTime.Today.Month;
		int year = DateTime.Today.Year;
		string text = day + "_" + month + "_" + year;
		if (PlayerPrefs.GetInt(text + "taskDone", 0) == 0)
		{
			PlayerPrefs.SetInt(text + "taskDone", 1);
			Analytics.CustomEvent("Task", new Dictionary<string, object> { { "TaskDone ", 1 } });
		}
	}
}
