using System;
using System.Collections.Generic;

public class CustomComparer : IComparer<string>
{
	public int Compare(string x, string y)
	{
		if (!x.Contains("spoiler") || !y.Contains("spoiler"))
		{
			return x.CompareTo(y);
		}
		string[] array = x.Split(new string[1] { "spoiler" }, StringSplitOptions.None);
		array = array[1].Split('.');
		string[] array2 = y.Split(new string[1] { "spoiler" }, StringSplitOptions.None);
		array2 = array2[1].Split('.');
		return int.Parse(array[0]).CompareTo(int.Parse(array2[0]));
	}
}
