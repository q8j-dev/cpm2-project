using System;
using CodeStage.AntiCheat.Storage;
using UniRx;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
	public GameObject[] eggArray;

	private void Start()
	{
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			CheckEgg();
		}).AddTo(this);
	}

	private void CheckEgg()
	{
		for (int i = 0; i < eggArray.Length; i++)
		{
			if (ObscuredPrefs.GetInt("egg" + i) == 1)
			{
				UnityEngine.Object.Destroy(eggArray[i]);
			}
		}
	}
}
