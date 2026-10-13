using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class GameObjectsActiveByQualityLevel : MonoBehaviour
	{
		[Serializable]
		private class GameObjectsData
		{
			public int[] QualityLevels;

			public GameObject[] GameObjects;

			public void Update(bool isActive)
			{
				for (int i = 0; i < GameObjects.Length; i++)
				{
					if ((bool)GameObjects[i])
					{
						GameObjects[i].SetActive(isActive);
					}
				}
			}
		}

		[SerializeField]
		private GameObjectsData[] GameObjectsDatas;

		private CPMGraphicsSettingsManager _graphicSettings => CPMGraphicsSettingsManager.Instance;

		private void OnEnable()
		{
			UpdateGameObjectsActiveByQualityLevel(_graphicSettings.Quality);
			_graphicSettings.OnQualityChanged += OnQualityChanged;
		}

		private void OnDisable()
		{
			_graphicSettings.OnQualityChanged -= OnQualityChanged;
		}

		private void OnQualityChanged(int quality)
		{
			UpdateGameObjectsActiveByQualityLevel(quality);
		}

		private void UpdateGameObjectsActiveByQualityLevel(int quality)
		{
			for (int i = 0; i < GameObjectsDatas.Length; i++)
			{
				GameObjectsDatas[i].Update(GameObjectsDatas[i].QualityLevels.Contains(quality));
			}
		}
	}
}
