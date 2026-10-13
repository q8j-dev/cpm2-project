using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Levels/LevelsConfig", order = 1)]
public class LevelsConfig : ScriptableObject
{
	public LevelDataScript[] LevelsDatas;

	public LevelCategory[] LevelCategories;

	public static readonly int[] IDOrderLevels = new int[84]
	{
		84, 61, 0, 1, 10, 90, 91, 25, 21, 29,
		15, 2, 18, 19, 20, 12, 28, 30, 31, 36,
		37, 26, 5, 44, 52, 47, 92, 27, 6, 4,
		53, 50, 51, 7, 54, 48, 76, 77, 78, 79,
		80, 81, 83, 85, 84, 86, 40, 32, 62, 33,
		88, 89, 34, 63, 38, 35, 82, 39, 41, 23,
		22, 45, 46, 24, 56, 55, 64, 65, 67, 66,
		70, 69, 68, 71, 72, 73, 74, 75, 57, 58,
		59, 60, 42, 43
	};

	[Preserve]
	public LevelsConfig()
	{
	}

	[ContextMenu("Sort")]
	public void Sort()
	{
		List<LevelDataScript> list = new List<LevelDataScript>();
		int[] iDOrderLevels = IDOrderLevels;
		foreach (int num in iDOrderLevels)
		{
			int num2 = LevelsDatas.Length;
			while (num2-- > 0)
			{
				if (LevelsDatas[num2].ID == num)
				{
					LevelsDatas[num2].SceneForLoad++;
					list.Add(LevelsDatas[num2]);
				}
			}
		}
		LevelsDatas = list.ToArray();
	}
}
