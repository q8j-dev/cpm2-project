using System.Collections.Generic;
using System.Linq;
using CPM.Common;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "AllRacingCars", menuName = "CPMInstruments/AllRacingCarsConfig", order = 50)]
public class AllRacingCarClassList : ScriptableObject
{
	private class OneCarMainValuesToSort
	{
		public float ValueToSort;

		public MainCarValues values;
	}

	public MainCarValues[] class14;

	public MainCarValues[] class13;

	public MainCarValues[] class12;

	public MainCarValues[] class11;

	public MainCarValues[] class10;

	public MainCarValues[] class9;

	public MainCarValues[] classNoLimit;

	private bool NamesWasRandomized { get; set; }

	public MainCarValues[] GetClassList(int input)
	{
		TryRandomNames();
		if (input > 8)
		{
			return input switch
			{
				9 => class9, 
				10 => class10, 
				11 => class11, 
				12 => class12, 
				13 => class13, 
				14 => class14, 
				_ => class13, 
			};
		}
		return classNoLimit;
	}

	private void TryRandomNames()
	{
		if (!NamesWasRandomized)
		{
			Debug.Log("Randomize racing cars names");
			NamesWasRandomized = true;
			classNoLimit.ForEach((MainCarValues x) =>
			{
				SetRandomNameAndGender(x);
			});
			class9.ForEach((MainCarValues x) =>
			{
				SetRandomNameAndGender(x);
			});
			class10.ForEach((MainCarValues x) =>
			{
				SetRandomNameAndGender(x);
			});
			class11.ForEach((MainCarValues x) =>
			{
				SetRandomNameAndGender(x);
			});
			class12.ForEach((MainCarValues x) =>
			{
				SetRandomNameAndGender(x);
			});
			class13.ForEach((MainCarValues x) =>
			{
				SetRandomNameAndGender(x);
			});
			class14.ForEach((MainCarValues x) =>
			{
				SetRandomNameAndGender(x);
			});
		}
	}

	public MainCarValues[] GetClassList(int input, int carCount)
	{
		List<MainCarValues> list = GetClassList(input).ToList();
		MainCarValues[] array = new MainCarValues[16];
		int num = Mathf.Min(16, list.Count);
		for (int i = 0; i < num; i++)
		{
			array[i] = list[i];
		}
		for (int j = 0; j < carCount; j++)
		{
			if (array[j] == null && list.Count > 0)
			{
				array[j] = Object.Instantiate(list[Random.Range(0, list.Count)]);
				SetRandomNameAndGender(array[j]);
			}
		}
		List<string> list2 = new List<string>();
		MainCarValues[] array2 = array;
		foreach (MainCarValues mainCarValues in array2)
		{
			if (mainCarValues == null)
			{
				continue;
			}
			int num2 = 10;
			while (num2-- > 0)
			{
				SetRandomNameAndGender(mainCarValues);
				if (!list2.Contains(mainCarValues.NickName))
				{
					break;
				}
			}
			list2.Add(mainCarValues.NickName);
		}
		return array;
	}

	private static void SetRandomNameAndGender(MainCarValues mainCarValues)
	{
		if ((bool)mainCarValues)
		{
			mainCarValues.gender = Random.Range(0, 2);
			if (mainCarValues.gender == 0)
			{
				mainCarValues.NickName = ChatFilter.maleRandomName[Random.Range(0, ChatFilter.maleRandomName.Length)];
			}
			else
			{
				mainCarValues.NickName = ChatFilter.femaleRandomName[Random.Range(0, ChatFilter.femaleRandomName.Length)];
			}
		}
	}

	public MainCarValues[] EliminateRandom(MainCarValues[] input, int maxCarCount)
	{
		int num = input.Length - maxCarCount;
		if (num <= 0)
		{
			return input;
		}
		List<MainCarValues> list = new List<MainCarValues>();
		List<OneCarMainValuesToSort> list2 = new List<OneCarMainValuesToSort>();
		foreach (MainCarValues mainCarValues in input)
		{
			OneCarMainValuesToSort item = new OneCarMainValuesToSort
			{
				values = mainCarValues,
				ValueToSort = mainCarValues.torque
			};
			list2.Add(item);
		}
		list2 = list2.OrderByDescending((OneCarMainValuesToSort o) => o.ValueToSort).ToList();
		int num2 = -1;
		foreach (OneCarMainValuesToSort item2 in list2)
		{
			num2++;
			if (num > 0 && Random.Range(0, maxCarCount - num2) == 0)
			{
				Debug.Log($"Removed: {num2}");
				num--;
			}
			else if (list.Count < maxCarCount)
			{
				list.Add(item2.values);
			}
		}
		return list.ToArray();
	}

	public MainCarValues[] EliminateRandom(MainCarValues[] input, int maxCarCount, GameObject ownCar)
	{
		maxCarCount = Mathf.Max(maxCarCount, 4);
		int num = input.Length - maxCarCount;
		if (num <= 0)
		{
			return input;
		}
		List<MainCarValues> list = new List<MainCarValues>();
		List<OneCarMainValuesToSort> list2 = new List<OneCarMainValuesToSort>();
		foreach (MainCarValues mainCarValues in input)
		{
			OneCarMainValuesToSort item = new OneCarMainValuesToSort
			{
				values = mainCarValues,
				ValueToSort = mainCarValues.torque
			};
			list2.Add(item);
		}
		list2 = list2.OrderByDescending((OneCarMainValuesToSort o) => o.ValueToSort).ToList();
		int num2 = -1;
		list.Add(ownCar.GetComponent<VehicleData>().CarValues);
		foreach (OneCarMainValuesToSort item2 in list2)
		{
			num2++;
			if (num > 0 && Random.Range(0, maxCarCount - num2) == 0)
			{
				num--;
			}
			else if (list.Count < maxCarCount)
			{
				list.Add(item2.values);
			}
		}
		Debug.Log($"{list.Count} length");
		return list.ToArray();
	}
}
