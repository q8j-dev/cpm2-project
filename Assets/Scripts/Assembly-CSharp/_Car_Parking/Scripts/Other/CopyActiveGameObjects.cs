using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class CopyActiveGameObjects : MonoBehaviour
	{
		[Serializable]
		private class Data
		{
			public GameObject Source;

			public GameObject Target;
		}

		[SerializeField]
		private Data[] Datas;

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
