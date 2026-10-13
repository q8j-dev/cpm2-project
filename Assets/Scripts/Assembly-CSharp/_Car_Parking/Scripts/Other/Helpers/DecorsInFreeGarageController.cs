using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class DecorsInFreeGarageController : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] GameObjects;

		private List<int> _needToEnableIndex = new List<int>();

		private void Awake()
		{
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Combine(V.OnInGarageFreeChanged, new Action<bool>(InGarageFreeChangedCallback));
		}

		private void OnDestroy()
		{
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Remove(V.OnInGarageFreeChanged, new Action<bool>(InGarageFreeChangedCallback));
		}

		private void InGarageFreeChangedCallback(bool inGarage)
		{
			if (inGarage)
			{
				_needToEnableIndex.Clear();
				for (int i = 0; i < GameObjects.Length; i++)
				{
					if ((bool)GameObjects[i] && GameObjects[i].activeSelf)
					{
						_needToEnableIndex.Add(i);
						GameObjects[i].SetActive(value: false);
					}
				}
				return;
			}
			for (int j = 0; j < _needToEnableIndex.Count; j++)
			{
				int num = _needToEnableIndex[j];
				if (num < GameObjects.Length && (bool)GameObjects[num])
				{
					GameObjects[num].SetActive(value: true);
				}
			}
			_needToEnableIndex.Clear();
		}
	}
}
