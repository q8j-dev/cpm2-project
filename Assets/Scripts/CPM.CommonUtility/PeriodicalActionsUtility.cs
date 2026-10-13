using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicalActionsUtility
{
	public class RefreshData
	{
		public readonly string id;

		public float refreshTime;

		private readonly float minRefreshTime;

		private readonly float maxRefreshPow;

		private float currentPow;

		public readonly Action refreshAction;

		public RefreshData(string id, float refreshTime, Action refreshAction)
		{
			this.refreshTime = refreshTime;
			this.refreshAction = refreshAction;
			this.id = id;
		}

		public RefreshData(string id, float minRefreshTime, float maxRefreshPow, Action refreshAction, int startingPow = 0)
		{
			this.minRefreshTime = minRefreshTime;
			this.maxRefreshPow = maxRefreshPow;
			this.refreshAction = refreshAction;
			currentPow = startingPow;
			this.id = id;
			SetRefreshTimeWithPow();
		}

		private void SetRefreshTimeWithPow()
		{
			refreshTime = minRefreshTime * Mathf.Pow(2f, currentPow);
		}

		public void ChangePow(int deltaValue)
		{
			if (maxRefreshPow != 0f)
			{
				currentPow = Mathf.Clamp(currentPow + (float)deltaValue, 0f, maxRefreshPow);
				SetRefreshTimeWithPow();
			}
		}
	}

	private class CoroutineDataPair
	{
		public CustomCour cour;

		public RefreshData data;

		public CoroutineDataPair(CustomCour cour, RefreshData data)
		{
			this.cour = cour;
			this.data = data;
		}
	}

	private readonly Dictionary<string, CoroutineDataPair> courDataPairsDict;

	public PeriodicalActionsUtility(MonoBehaviour mono, params RefreshData[] datas)
	{
		courDataPairsDict = new Dictionary<string, CoroutineDataPair>();
		foreach (RefreshData data in datas)
		{
			CustomCour cour = new CustomCour(mono, () => Refreshing(data));
			courDataPairsDict.Add(data.id, new CoroutineDataPair(cour, data));
		}
	}

	public void StopAll()
	{
		foreach (KeyValuePair<string, CoroutineDataPair> item in courDataPairsDict)
		{
			item.Value.cour.Stop();
		}
	}

	public void StartAll()
	{
		foreach (KeyValuePair<string, CoroutineDataPair> item in courDataPairsDict)
		{
			item.Value.cour.Run();
		}
	}

	public void AdjustRefreshDataPow(string id, int value)
	{
		if (courDataPairsDict.TryGetValue(id, out var value2))
		{
			value2.data.ChangePow(value);
		}
	}

	public void StartRefresh(string id)
	{
		if (courDataPairsDict.TryGetValue(id, out var value))
		{
			value.cour.Run();
		}
	}

	public void StopRefresh(string id)
	{
		if (courDataPairsDict.TryGetValue(id, out var value))
		{
			value.cour.Stop();
			value.data.ChangePow(-5);
		}
	}

	private IEnumerator Refreshing(RefreshData refreshData)
	{
		while (refreshData.refreshAction != null)
		{
			yield return new WaitForSeconds(refreshData.refreshTime);
			refreshData.refreshAction?.Invoke();
		}
	}
}
