using System.Collections.Generic;

namespace _Car_Parking.Scripts
{
	public class DictionaryDebugExtension
	{
		public static void Sort<T>(Dictionary<int, T> dict)
		{
			Dictionary<int, T> dictionary = new Dictionary<int, T>();
			foreach (KeyValuePair<int, T> item in dict)
			{
				dictionary.Add(item.Key, item.Value);
			}
			dict.Clear();
			while (dictionary.Count > 0)
			{
				int num = 0;
				bool flag = false;
				foreach (KeyValuePair<int, T> item2 in dictionary)
				{
					if (item2.Key < num || !flag)
					{
						flag = true;
						num = item2.Key;
					}
				}
				dict.Add(num, dictionary.GetValueOrDefault(num, default));
				dictionary.Remove(num);
			}
		}
	}
}
