using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class CopyActiveGameObjects : MonoBehaviour
	{
		[Serializable]
		private class Data
		{
			public GameObject Source = default;

			public GameObject Target = default;
		}

		[SerializeField]
		private Data[] Datas = default;

		private void OnEnable()
		{
			for (int i = 0; i < Datas.Length; i++)
			{
				if ((bool)Datas[i].Source && (bool)Datas[i].Target)
				{
					Datas[i].Target.SetActive(Datas[i].Source.activeSelf);
				}
			}
		}
	}
}
