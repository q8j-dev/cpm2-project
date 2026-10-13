#nullable enable annotations
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace Kit
{
	internal class Value
	{
		private int[] spoilerIndexes;

		private int index { get; set; }

		private int carIndex { get; set; }

		private int type { get; set; }

		private Dictionary<string, Transform?> paths { get; }

		private List<ObscuredInt> prices { get; }

		public Value(int index)
		{
			this.index = index;
			paths = new Dictionary<string, Transform>();
			prices = new List<ObscuredInt>();
		}

		public Value(int index, int type, int carIndex)
		{
			this.index = index;
			this.carIndex = carIndex;
			this.type = type;
			paths = new Dictionary<string, Transform>();
			prices = new List<ObscuredInt>();
		}

		public Value(int index, int type, int carIndex, int[] spoilerIndexes)
		{
			this.spoilerIndexes = spoilerIndexes;
			this.index = index;
			this.carIndex = carIndex;
			this.type = type;
			paths = new Dictionary<string, Transform>();
			prices = new List<ObscuredInt>();
		}

		public void Add(KeyValuePair<string, Transform?> pair, ObscuredInt price)
		{
			paths.Add(pair.Key, pair.Value);
			prices.Add(price);
		}

		public int GetIndex()
		{
			if (type != 0)
			{
				return index;
			}
			return spoilerIndexes[index];
		}

		public (string, Transform) Next(ref int cash, ref int coin)
		{
			int num = int.Parse(prices[index].ToString());
			bool flag = ObscuredPrefs.GetInt("type" + type + "_has_" + ((type == 0) ? spoilerIndexes[index] : index) + "_" + carIndex) == 1;
			if (num == 0)
			{
				cash -= ((!((index == 0) | flag)) ? 5000 : 0);
			}
			else
			{
				coin -= ((!((index == 0) | flag)) ? num : 0);
			}
			index = (paths.Count + ++index) % paths.Count;
			num = int.Parse(prices[index].ToString());
			flag = ObscuredPrefs.GetInt("type" + type + "_has_" + ((type == 0) ? spoilerIndexes[index] : index) + "_" + carIndex) == 1;
			if (num == 0)
			{
				cash += ((!((index == 0) | flag)) ? 5000 : 0);
			}
			else
			{
				coin += ((!((index == 0) | flag)) ? num : 0);
			}
			return (paths.ElementAt(index).Key, paths.ElementAt(index).Value);
		}

		public (string, Transform) Prev(ref int cash, ref int coin)
		{
			int num = int.Parse(prices[index].ToString());
			bool flag = ObscuredPrefs.GetInt("type" + type + "_has_" + ((type == 0) ? spoilerIndexes[index] : index) + "_" + carIndex) == 1;
			if (num == 0)
			{
				cash -= ((!((index == 0) | flag)) ? 5000 : 0);
			}
			else
			{
				coin -= ((!((index == 0) | flag)) ? num : 0);
			}
			index = (paths.Count + --index) % paths.Count;
			flag = ObscuredPrefs.GetInt("type" + type + "_has_" + ((type == 0) ? spoilerIndexes[index] : index) + "_" + carIndex) == 1;
			num = int.Parse(prices[index].ToString());
			if (num == 0)
			{
				cash += ((!((index == 0) | flag)) ? 5000 : 0);
			}
			else
			{
				coin += ((!((index == 0) | flag)) ? num : 0);
			}
			return (paths.ElementAt(index).Key, paths.ElementAt(index).Value);
		}

		public override string ToString()
		{
			string text = "[";
			for (int i = 0; i < paths.Count; i++)
			{
				text = text + "{" + paths.ElementAt(i).Key + ": " + prices[i].ToString() + "}";
				if (i != paths.Count - 1)
				{
					text += ", \n";
				}
			}
			return text + "]";
		}
	}
}
