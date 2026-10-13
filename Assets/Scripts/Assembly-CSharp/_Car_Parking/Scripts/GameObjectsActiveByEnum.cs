using System;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class GameObjectsActiveByEnum<T> : MonoBehaviour where T : Enum
	{
		[Serializable]
		private class GameObjectWithEnum
		{
			public T Type = default;

			public GameObject[] GameObjects = default;

			public void UpdateState(T type)
			{
				for (int i = 0; i < GameObjects.Length; i++)
				{
					if ((bool)GameObjects[i])
					{
						GameObjects[i].SetActive(type.Equals(Type));
					}
				}
			}
		}

		[SerializeField]
		private GameObjectWithEnum[] GameObjectWithEnums = default;

		public void UpdateState(T type)
		{
			for (int i = 0; i < GameObjectWithEnums.Length; i++)
			{
				GameObjectWithEnums[i].UpdateState(type);
			}
		}
	}
}
