using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analitcs : MonoBehaviour
{
	public void OnGameOver()
	{
		string loadedLevelName = Application.loadedLevelName;
		int num = 100;
		Analytics.CustomEvent("gameOver", new Dictionary<string, object>
		{
			{ "potions", loadedLevelName },
			{ "coins", num }
		});
	}

	private void Start()
	{
		OnGameOver();
	}
}
