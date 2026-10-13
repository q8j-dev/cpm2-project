using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.SkinnedMeshCombiner
{
	[AddComponentMenu("")]
	public class MTAssetsMathematics : MonoBehaviour
	{
		public static List<T> RandomizeThisList<T>(List<T> list)
		{
			int count = list.Count;
			int num = count - 1;
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(i, count);
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
			return list;
		}
	}
}
